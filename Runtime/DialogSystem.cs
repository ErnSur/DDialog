using System.Collections.Generic;
using UnityEngine;

namespace Doublsb.Dialog
{
    using System;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;


    public class CommandSystemPoC
    {
        private static CommandRunner _commandRunner = new CommandRunner();
        private void Init()
        {
            UserCode_RegisterActorCommand();
        }
        
        private void UserCode_RegisterActorCommand()
        {
            var previousValue = 0;
            var value = 5;
            var newValue = 10;
            _commandRunner.RegisterCommandCallback("actor", async token =>
            {
                previousValue = value;
                value = newValue;
            }, async token =>
            {
                value = previousValue;
            });
        }
    }

    public class CommandRunner
    {
        private Func<CancellationToken, UniTask> callbacks;
        public void RegisterBeginCallback(string commandName, Func<CancellationToken, UniTask> callback, float priority = 0)
        {
        }
        
        public void RegisterCommandCallback(string commandName, Func<CancellationToken, UniTask> beginCallback, Func<CancellationToken, UniTask> endCallback, float priority = 0)
        {
        }

        public async UniTask Execute(string script, CancellationToken cancellationToken = default)
        {
            // TODO: set actor ID in the `WriteSo` method to the actor tag
            var commandTree = CommandParser.ParseCommands(script, _, _);
            await callbacks.Invoke(cancellationToken);
            await commandTree.Act(cancellationToken);
        }

        public void GoTo(string commandLabel)
        {
            // TODO: implement. This method should be used to jump to a specific command in the execution tree. i.e., go to label in renpy
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

        public async UniTask Run(List<ActorLines> data)
        {
            await Activate_List(data);
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
                    var destroyToken = this.GetCancellationTokenOnDestroy();
                    using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, destroyToken);
                    await call(linkedToken.Token);
                }
                catch (Exception e)
                {
                    //Debug.LogError($"Command failed:{e}");
                }
            }

            _state = State.AwaitingClose;
        }
    }
}