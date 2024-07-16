using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Doublsb.Dialog
{
    using System.Linq;
    using JetBrains.Annotations;
    using UnityEngine.Events;

    public class DialogManager : MonoBehaviour
    {
        public UnityEvent<string> actorLineStarted;
        public UnityEvent<string> actorLineFinished;
        [Header("Game Objects")]
        public GameObject Printer;

        [Header("UI Objects")]
        public Text Printer_Text;

        [Header("Audio Objects")]
        private SoundManager SEAudio;
        
        [Header("Preference")]
        public float Delay = 0.1f;

        [Header("Selector")]
        public GameObject Selector;

        public GameObject SelectorItem;
        public Text SelectorItemText;

        [HideInInspector]
        public string Result;
        
        public State State { get; private set; }

        private ISoundEffectProvider _soundEffectProvider;
        private DialogData _currentData;
        private float _currentDelay;
        private float _lastDelay;
        private Coroutine _textingRoutine;
        private Coroutine _printingRoutine;

        public AudioClip[] defaultChatSoundEffects;

        private Dictionary<string, IDialogCommandHandler> _commandHandlers;

        private bool _initialized;

        private void Awake() => OneTimeInit();

        private void OneTimeInit()
        {
            if (_initialized)
                return;
            SEAudio = gameObject.GetComponent<SoundManager>();
            _soundEffectProvider = GetComponent<ISoundEffectProvider>();
            _commandHandlers = GetComponents<IDialogCommandHandler>().ToDictionary(handler => handler.Identifier);
            _initialized = true;
        }
        //================================================
        //Public Method
        //================================================

        #region Show & Hide

        public void Show(DialogData data)
        {
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
                    StartCoroutine(_skip());
                    break;

                case State.Wait:
                    if (_currentData.SelectList.Count <= 0)
                        Hide();
                    break;
            }
        }

        public void Hide()
        {
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

            Printer.SetActive(false);
            actorLineFinished.Invoke(_currentData.ActorId);
            Selector.SetActive(false);

            State = State.Deactivate;

            if (_currentData.Callback != null)
            {
                _currentData.Callback.Invoke();
                _currentData.Callback = null;
            }
        }

        #endregion

        #region Selector

        public void Select(int index)
        {
            Result = _currentData.SelectList.GetByIndex(index).Key;
            Hide();
        }

        #endregion

        #region Sound
        
        // Idea: play a different sound on samogłoska i spółgłoska
        // albo inny dźwięk na każdą literę
        public void Play_ChatSE()
        {
            var clips = defaultChatSoundEffects;
            if (_soundEffectProvider != null && _soundEffectProvider.TryGetChatSoundEffects(_currentData.ActorId, out var actorClips))
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

        #region Speed

        public void Set_Speed(string speed)
        {
            switch (speed)
            {
                case "up":
                    _currentDelay -= 0.25f;
                    if (_currentDelay <= 0)
                        _currentDelay = 0.001f;
                    break;

                case "down":
                    _currentDelay += 0.25f;
                    break;

                case "init" or "end":
                    _currentDelay = Delay;
                    break;

                default:
                    if (float.TryParse(speed, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var parsedSpeed))
                        _currentDelay = parsedSpeed;
                    else
                        throw new System.Exception($"Cannot parse float number: {speed}");
                    break;
            }

            _lastDelay = _currentDelay;
        }

        #endregion

        //================================================
        //Private Method
        //================================================

        private void _initialize()
        {
            OneTimeInit();
            _currentDelay = Delay;
            _lastDelay = 0.1f;
            Printer_Text.text = string.Empty;

            Printer.SetActive(true);

            actorLineStarted.Invoke(_currentData.ActorId);
        }

        private void _init_selector()
        {
            _clear_selector();

            if (_currentData.SelectList.Count > 0)
            {
                Selector.SetActive(true);

                for (int i = 0; i < _currentData.SelectList.Count; i++)
                {
                    _add_selectorItem(i);
                }
            }

            else
                Selector.SetActive(false);
        }

        private void _clear_selector()
        {
            for (int i = 1; i < Selector.transform.childCount; i++)
            {
                Destroy(Selector.transform.GetChild(i).gameObject);
            }
        }

        private void _add_selectorItem(int index)
        {
            SelectorItemText.text = _currentData.SelectList.GetByIndex(index).Value;

            var newItem = Instantiate(SelectorItem, Selector.transform);
            newItem.GetComponent<Button>().onClick.AddListener(() => Select(index));
            newItem.SetActive(true);
        }

        #region Show Text

        private IEnumerator Activate_List(List<DialogData> dataList)
        {
            State = State.Active;

            foreach (var data in dataList)
            {
                Show(data);
                _init_selector();

                while (State != State.Deactivate)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator Activate()
        {
            _initialize();

            State = State.Active;

            foreach (var item in _currentData.Commands)
            {
                if (_commandHandlers.TryGetValue(item.CommandId.ToString(), out var handler))
                {
                    yield return handler.PerformAction(item.Argument, _currentData);
                    Debug.Log($"Command {item.CommandId} Activated!!");
                    continue;
                }

                switch (item.CommandId)
                {
                    case CommandId.print:
                        yield return _printingRoutine = StartCoroutine(_print(item.Argument));
                        break;

                    case CommandId.speed:
                        Set_Speed(item.Argument);
                        break;

                    case CommandId.click:
                        yield return _waitInput();
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

        private IEnumerator _waitInput()
        {
            while (!Input.GetMouseButtonDown(0))
                yield return null;
            _currentDelay = _lastDelay;
        }

        private IEnumerator _print(string Text)
        {
            _currentData.PrintText += _currentData.Format.OpenTagger;

            for (int i = 0; i < Text.Length; i++)
            {
                _currentData.PrintText += Text[i];
                Printer_Text.text = _currentData.PrintText + _currentData.Format.CloseTagger;

                if (Text[i] != ' ')
                    Play_ChatSE();
                if (_currentDelay != 0)
                    yield return new WaitForSeconds(_currentDelay);
            }

            _currentData.PrintText += _currentData.Format.CloseTagger;
        }

        private IEnumerator _skip()
        {
            if (_currentData.CanBeSkipped)
            {
                _currentDelay = 0;
                while (State != State.Wait)
                    yield return null;
                _currentDelay = Delay;
            }
        }

        #endregion
    }
}