using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Doublsb.Dialog
{
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;
    using UnityEngine.Events;
    
    public class DialogPrinter : MonoBehaviour
    {
        public UnityEvent<string> actorLineStarted;
        public UnityEvent<string> actorLineFinished;
        public DialogCommandSet CurrentDialogCommandSet { get; private set; }

        private bool _initialized;

        private Dictionary<string, IDialogCommandHandler> _commandHandlers;
        
        private Coroutine _commandChainRoutine;

        private int? _selectedOptionIndex;
        
        private IDialogView _dialogView;
        private IDialogMenuView _dialogMenu;
        private State _state;

        private CancellationTokenSource _fastForwardTokenSource;

        private void Awake()
        {
            OneTimeInitialize();
        }

        private void OneTimeInitialize()
        {
            if (_initialized)
                return;
            _commandHandlers = GetComponents<IDialogCommandHandler>().ToDictionary(handler => handler.Identifier);
            _dialogView = GetComponent<IDialogView>();
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
            _dialogView.Text = string.Empty;
            _dialogView.SetActive(true);
            actorLineStarted.Invoke(CurrentDialogCommandSet.ActorId);
        }

        public void Run(DialogCommandSet commandSet)
        {
            _selectedOptionIndex = null;
            CurrentDialogCommandSet = commandSet;
            _commandChainRoutine = StartCoroutine(RunCommandSet());
        }

        public void Run(List<DialogCommandSet> data)
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
                    if (CurrentDialogCommandSet.SelectList.Count <= 0)
                        Close();
                    break;
            }
        }

        public void Close()
        {
            if (_commandChainRoutine != null)
                StopCoroutine(_commandChainRoutine);

            foreach (var item in CurrentDialogCommandSet.Commands)
            {
                if (_commandHandlers.TryGetValue(item.CommandId.ToString(), out var handler))
                {
                    StartCoroutine(handler.CleanupAction(item.Argument, CurrentDialogCommandSet));
                }
            }

            _dialogView.SetActive(false);
            actorLineFinished.Invoke(CurrentDialogCommandSet.ActorId);
            StartCoroutine(_dialogMenu.Close());

            _state = State.Deactivate;

            if (CurrentDialogCommandSet.Callback != null)
            {
                CurrentDialogCommandSet.Callback.Invoke();
                CurrentDialogCommandSet.Callback = null;
            }

            if (_selectedOptionIndex.HasValue)
            {
                var selectedOption = CurrentDialogCommandSet.SelectList[_selectedOptionIndex.Value];
                selectedOption.Callback?.Invoke();
            }
        }


        private IEnumerator Activate_List(List<DialogCommandSet> commandSets)
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

        private IEnumerator RunCommandSet()
        {
            Setup();
            _state = State.RunningCommands;

            foreach (var command in CurrentDialogCommandSet.Commands)
            {
                if (_commandHandlers.TryGetValue(command.CommandId.ToString(), out var handler))
                {
                    yield return handler.PerformAction(command.Argument, CurrentDialogCommandSet, _fastForwardTokenSource.Token);
                }
            }

            _state = State.AwaitingClose;
        }

        private void FastForwardCommands()
        {
            if (CurrentDialogCommandSet.CanBeSkipped)
                _fastForwardTokenSource.Cancel();
        }
    }
}