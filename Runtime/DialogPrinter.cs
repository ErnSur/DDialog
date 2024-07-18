using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Doublsb.Dialog
{
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine.Events;

    [RequireComponent(typeof(ICommandFactory))]
    public class DialogPrinter : MonoBehaviour
    {
        public UnityEvent<string> actorLineStarted;
        public UnityEvent<string> actorLineFinished;
        public DialogCommandSet CurrentDialogCommandSet { get; private set; }

        private bool _initialized;

        private int? _selectedOptionIndex;

        private IPrinter _dialogView;
        private IDialogMenuView _dialogMenu;
        private State _state;

        private CancellationTokenSource _fastForwardTokenSource;
        private CancellationTokenSource _cancelCommandChainTokenSource;
        private ICommandFactory _commandFactory;
        private List<Command> _currentCommands = new List<Command>();

        private void Awake()
        {
            OneTimeInitialize();
        }

        private void OneTimeInitialize()
        {
            if (_initialized)
                return;
            _commandFactory = GetComponent<ICommandFactory>();
            _dialogView = GetComponent<IPrinter>();
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
            _cancelCommandChainTokenSource?.Dispose();
            _cancelCommandChainTokenSource = new CancellationTokenSource();
            RunCommandSet(_cancelCommandChainTokenSource.Token).Forget();
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
            _cancelCommandChainTokenSource?.Cancel();

            foreach (var command in _currentCommands)
            {
                command.Dispose();
            }

            _currentCommands.Clear();

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

        private async UniTask RunCommandSet(CancellationToken cancellationToken)
        {
            Setup();
            _state = State.RunningCommands;

            // todo: optimize so that xml deserialization is not don in runtime but serialized
            
            Debug.Log($"Parese: {CurrentDialogCommandSet.Script}");
            _currentCommands = CommandParser.Parse(CurrentDialogCommandSet.Script, _commandFactory);
            foreach (var command in _currentCommands)
            {
                //Debug.Log("Running: " + command.GetType().Name);
                await command.Execute(_fastForwardTokenSource.Token);
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