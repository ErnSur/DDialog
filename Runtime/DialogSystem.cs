using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Doublsb.Dialog
{
    using System;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine.Events;

    [RequireComponent(typeof(ICommandFactory))]
    public class DialogSystem : MonoBehaviour
    {
        public UnityEvent<string> actorLineStarted;
        public UnityEvent<string> actorLineFinished;
        public ActorLines CurrentActorLines { get; private set; }

        private bool _initialized;

        private int? _selectedOptionIndex;

        private IPrinter _printer;
        private IDialogMenuView _dialogMenu;
        private State _state;

        private CancellationTokenSource _fastForwardTokenSource;
        private CancellationTokenSource _cancelCommandChainTokenSource;
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
            _printer = GetComponent<IPrinter>();
            _dialogMenu = GetComponent<IDialogMenuView>();
            _dialogMenu.OptionSelected += index =>
            {
                _selectedOptionIndex = index;
                Close();
            };
            _initialized = true;
        }

        /// <summary>
        /// Initialization that runs before every dialog start
        /// </summary>
        private void Setup()
        {
            _fastForwardTokenSource?.Dispose();
            _fastForwardTokenSource = new CancellationTokenSource();

            OneTimeInitialize();
            _printer.Reset();
            _printer.SetActive(true);
            actorLineStarted.Invoke(CurrentActorLines.ActorId);
        }

        public void Run(ActorLines commandSet)
        {
            _selectedOptionIndex = null;
            CurrentActorLines = commandSet;
            _cancelCommandChainTokenSource?.Dispose();
            _cancelCommandChainTokenSource = new CancellationTokenSource();
            RunCommandSet(_cancelCommandChainTokenSource.Token).Forget();
        }

        public void Run(List<ActorLines> data)
        {
            StartCoroutine(Activate_List(data));
        }

        [UsedImplicitly]
        public void SkipOrClose()
        {
            switch (_state)
            {
                case State.RunningCommands:
                    FastForwardCommands();
                    break;

                case State.AwaitingClose:
                    if (CurrentActorLines.SelectList.Count <= 0)
                        Close();
                    break;
            }
        }

        public void Close()
        {
            _cancelCommandChainTokenSource?.Cancel();

            foreach (var command in _rootCommand.OfType<IDisposable>())
            {
                command.Dispose();
            }

            _rootCommand = null;

            _printer.SetActive(false);
            actorLineFinished.Invoke(CurrentActorLines.ActorId);
            StartCoroutine(_dialogMenu.Close());

            _state = State.Deactivate;

            if (CurrentActorLines.Callback != null)
            {
                CurrentActorLines.Callback.Invoke();
                CurrentActorLines.Callback = null;
            }

            if (_selectedOptionIndex.HasValue)
            {
                var selectedOption = CurrentActorLines.SelectList[_selectedOptionIndex.Value];
                selectedOption.Callback?.Invoke();
            }
        }


        private IEnumerator Activate_List(List<ActorLines> commandSets)
        {
            _state = State.RunningCommands;

            foreach (var commandSet in commandSets)
            {
                Run(commandSet);
                if (commandSet.SelectList.Count > 0)
                {
                    var textOptions = commandSet.SelectList.Select(x => x.Text).ToArray();
                    StartCoroutine(_dialogMenu.Open(textOptions));
                }

                while (_state != State.Deactivate)
                {
                    yield return null;
                }
            }
        }

        private async UniTask RunCommandSet(CancellationToken cancellationToken)
        {
            Setup();
            _state = State.RunningCommands;

            _rootCommand = CommandParser.ParseCommands(CurrentActorLines.Script,CurrentActorLines.ActorId, _commandFactory);
            
            foreach (var command in _rootCommand)
                await command.Act(_fastForwardTokenSource.Token);

            _state = State.AwaitingClose;
        }

        private void FastForwardCommands()
        {
            if (CurrentActorLines.CanBeSkipped)
                _fastForwardTokenSource.Cancel();
        }
        
    }
}