using System.Collections.Generic;
using UnityEngine;

namespace Doublsb.Dialog
{
    using System;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;


    public class CommandRunner
    {
        public event Action CommandFinished;

        public async UniTask Execute(ICommand command, CancellationToken cancellationToken = default)
        {
            await command.Act(cancellationToken);
            CommandFinished?.Invoke();
        }
    }

    [RequireComponent(typeof(ICommandFactory))]
    public class DialogSystem : MonoBehaviour
    {
        private ActorLines _currentActorLines;
        private bool _initialized;
        private State _state;
        private CancellationTokenSource _fastForwardTokenSource = new();
        private ICommandFactory _commandFactory;
        private ICommand _rootCommand;

        private void Awake()
        {
            OneTimeInitialize();
        }

        private void OneTimeInitialize()
        {
            if (_initialized)
                return;
            _commandFactory = GetComponent<ICommandFactory>();
            _initialized = true;
        }

        /// <summary>
        /// Initialization that runs before every dialog start
        /// </summary>
        private void Setup()
        {
            OneTimeInitialize();
        }

        private async UniTask Run(ActorLines commandSet)
        {
            _currentActorLines = commandSet;
            await RunCommandSet();
        }

        public void Run(List<ActorLines> data)
        {
            Activate_List(data).Forget();
        }

        [UsedImplicitly]
        public void SkipOrClose()
        {
            switch (_state)
            {
                case State.AwaitingClose:
                    Close();
                    break;
            }
        }

        public void Close()
        {
            _fastForwardTokenSource?.Cancel();

            foreach (var command in _rootCommand.OfType<IDisposable>())
            {
                command.Dispose();
            }

            _rootCommand = null;

            _state = State.Deactivate;

            _currentActorLines.Callback?.Invoke();
        }


        private async UniTask Activate_List(List<ActorLines> commandSets)
        {
            _state = State.RunningCommands;

            foreach (var commandSet in commandSets)
            {
                await Run(commandSet);
            }
        }

        private async UniTask RunCommandSet()
        {
            Setup();
            _state = State.RunningCommands;

            _rootCommand =
                CommandParser.ParseCommands(_currentActorLines.Script, _currentActorLines.ActorId, _commandFactory);
            foreach (var call in _rootCommand.GetExecutionCalls())
            {
                var token = _fastForwardTokenSource.Token;
                if (token.IsCancellationRequested)
                {
                    _fastForwardTokenSource?.Dispose();
                    _fastForwardTokenSource = new CancellationTokenSource();
                    token = _fastForwardTokenSource.Token;
                }

                try
                {
                    // TODO: cancelation token should be used for real cancellation, like destroying the GameObject and stuff
                    // TODO: clicking on chat should not automatically cancel cancellation token, instead the command should receive information that the chat was clicked, possibly from context object / or just from the custom game context
                    var destroyToken = this.GetCancellationTokenOnDestroy();
                    using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, destroyToken);
                    await call(linkedToken.Token);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Command failed:{e}");
                }
            }

            Debug.Log("Command chain finished");
            _state = State.AwaitingClose;
        }
    }
}