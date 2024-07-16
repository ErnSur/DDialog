using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Doublsb.Dialog
{
    using System.Linq;
    using JetBrains.Annotations;
    using UnityEngine.Events;
    using Random = Random;

    public class DialogManager : MonoBehaviour
    {
        public UnityEvent<string> actorLineStarted;
        public UnityEvent<string> actorLineFinished;

        private SoundManager SEAudio;

        public float delay = 0.02f;

        public State State { get; private set; }

        private ISoundEffectProvider _soundEffectProvider;
        private DialogData _currentData;
        private Coroutine _textingRoutine;
        private Coroutine _printingRoutine;

        public AudioClip[] defaultChatSoundEffects;

        private Dictionary<string, IDialogCommandHandler> _commandHandlers;

        private bool _initialized;

        private IDialogMenuView _dialogMenu;
        private IDialogView _dialogView;

        private int? _selectedOptionIndex;

        private void Awake() => OneTimeInit();

        private void OneTimeInit()
        {
            if (_initialized)
                return;
            SEAudio = gameObject.GetComponent<SoundManager>();
            _soundEffectProvider = GetComponent<ISoundEffectProvider>();
            _commandHandlers = GetComponents<IDialogCommandHandler>().ToDictionary(handler => handler.Identifier);
            _dialogView = GetComponent<IDialogView>();
            _dialogMenu = GetComponent<IDialogMenuView>();
            _dialogMenu.OptionSelected += index =>
            {
                _selectedOptionIndex = index;
                Hide();
            };
            _initialized = true;
        }
        
        private void Initialize()
        {
            OneTimeInit();
            _dialogView.Text = string.Empty;
            _dialogView.SetActive(true);
            actorLineStarted.Invoke(_currentData.ActorId);
        }

        #region Show & Hide

        public void Show(DialogData data)
        {
            _selectedOptionIndex = null;
            _currentData = data;
            _textingRoutine = StartCoroutine(Activate());
        }

        public void Show(List<DialogData> data)
        {
            StartCoroutine(Activate_List(data));
        }

        [UsedImplicitly]
        public void Click_Window()
        {
            switch (State)
            {
                case State.Active:
                    StartCoroutine(Skip());
                    break;

                case State.Wait:
                    if (_currentData.SelectList.Count <= 0)
                        Hide();
                    break;
            }
        }

        public void Hide()
        {
            Debug.Log("Hide");
            if (_textingRoutine != null)
                StopCoroutine(_textingRoutine);

            if (_printingRoutine != null)
                StopCoroutine(_printingRoutine);

            foreach (var item in _currentData.Commands)
            {
                if (_commandHandlers.TryGetValue(item.CommandId.ToString(), out var handler))
                {
                    StartCoroutine(handler.CleanupAction(item.Argument, _currentData));
                }
            }

            _dialogView.SetActive(false);
            actorLineFinished.Invoke(_currentData.ActorId);
            StartCoroutine(_dialogMenu.Close());

            State = State.Deactivate;

            if (_currentData.Callback != null)
            {
                _currentData.Callback.Invoke();
                _currentData.Callback = null;
            }

            if (_selectedOptionIndex.HasValue)
            {
                var selectedOption = _currentData.SelectList[_selectedOptionIndex.Value];
                selectedOption.Callback?.Invoke();
            }
        }

        #endregion

        #region Sound

        // Idea: play a different sound on samogłoska i spółgłoska
        // albo inny dźwięk na każdą literę
        public void Play_ChatSE()
        {
            var clips = defaultChatSoundEffects;
            if (_soundEffectProvider != null &&
                _soundEffectProvider.TryGetChatSoundEffects(_currentData.ActorId, out var actorClips))
                clips = actorClips;
            if (_currentData.ChatSoundEffects is { Length: > 0 })
                clips = _currentData.ChatSoundEffects;
            if (clips == null || clips.Length == 0)
                return;

            var clip = clips[Random.Range(0, clips.Length)];
            if (clip != null)
                SEAudio.PlaySound(clip);
            // SEAudio.clip = clips[Random.Range(0, clips.Length)];
            // if (SEAudio.clip != null)
            //     SEAudio.Play();
        }

        #endregion

        #region Show Text

        private IEnumerator Activate_List(List<DialogData> dataList)
        {
            State = State.Active;

            foreach (var data in dataList)
            {
                Show(data);
                if (data.SelectList.Count > 0)
                {
                    var textOptions = data.SelectList.Select(x => x.Text).ToArray();
                    StartCoroutine(_dialogMenu.Open(textOptions));
                }

                while (State != State.Deactivate)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator Activate()
        {
            Initialize();

            State = State.Active;

            foreach (var item in _currentData.Commands)
            {
                if (_commandHandlers.TryGetValue(item.CommandId.ToString(), out var handler))
                {
                    yield return handler.PerformAction(item.Argument, _currentData);
                    continue;
                }

                switch (item.CommandId)
                {
                    case CommandId.print:
                        yield return _printingRoutine = StartCoroutine(Print(item.Argument));
                        break;

                    case CommandId.click:
                        yield return WaitForMouseClick();
                        break;

                    case CommandId.close:
                        Hide();
                        yield break;

                    case CommandId.wait:
                        if (float.TryParse(item.Argument, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out var waitTime))
                            yield return new WaitForSeconds(waitTime);
                        else
                            throw new System.Exception($"Cannot parse float number: {item.Argument}");
                        break;
                }
            }

            State = State.Wait;
        }

        private IEnumerator Print(string text)
        {
            _currentData.PrintText += _currentData.Format.OpenTagger;

            for (int i = 0; i < text.Length; i++)
            {
                _currentData.PrintText += text[i];
                _dialogView.Text = _currentData.PrintText + _currentData.Format.CloseTagger;

                if (text[i] != ' ')
                    Play_ChatSE();
                if (delay != 0)
                    yield return new WaitForSeconds(delay);
            }

            _currentData.PrintText += _currentData.Format.CloseTagger;
        }

        private IEnumerator Skip()
        {
            if (!_currentData.CanBeSkipped)
                yield break;
            var previousDelay = delay;
            delay = 0;
            while (State != State.Wait)
                yield return null;
            delay = previousDelay;
        }

        private static IEnumerator WaitForMouseClick()
        {
            while (!Input.GetMouseButtonDown(0))
                yield return null;
        }

        #endregion
    }
}