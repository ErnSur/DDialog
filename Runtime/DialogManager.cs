using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Doublsb.Dialog
{
    using System.Linq;

    public class DialogManager : MonoBehaviour
    {
        [Header("Game Objects")]
        public GameObject Printer;

        [Header("UI Objects")]
        public Text Printer_Text;

        [Header("Audio Objects")]
        public AudioSource SEAudio;

        [Header("Preference")]
        public float Delay = 0.1f;

        [Header("Selector")]
        public GameObject Selector;

        public GameObject SelectorItem;
        public Text SelectorItemText;

        [HideInInspector]
        public State state;

        [HideInInspector]
        public string Result;

        private IDialogActorManager _actorManager;
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
            _actorManager = GetComponent<IDialogActorManager>();
            _commandHandlers = GetComponents<IDialogCommandHandler>().ToDictionary(handler => handler.Identifier);
            _initialized = true;
        }
        //================================================
        //Public Method
        //================================================

        #region Show & Hide

        public void Show(DialogData Data)
        {
            _currentData = Data;
            _textingRoutine = StartCoroutine(Activate());
        }

        public void Show(List<DialogData> Data)
        {
            StartCoroutine(Activate_List(Data));
        }

        public void Click_Window()
        {
            switch (state)
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

            Printer.SetActive(false);
            _actorManager.HideAll();
            Selector.SetActive(false);

            state = State.Deactivate;

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

        public void Play_ChatSE()
        {
            var clips = defaultChatSoundEffects;
            if (_actorManager.TryGetChatSoundEffects(out var actorClips))
                clips = actorClips;
            if (_currentData.ChatSoundEffects is { Length: > 0 })
                clips = _currentData.ChatSoundEffects;
            if (clips == null || clips.Length == 0)
                return;

            SEAudio.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            if (SEAudio.clip != null)
                SEAudio.Play();
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

                case "init":
                    _currentDelay = Delay;
                    break;

                default:
                    _currentDelay = float.Parse(speed);
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

            _actorManager.Show(_currentData.ActorId);
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

            var NewItem = Instantiate(SelectorItem, Selector.transform);
            NewItem.GetComponent<Button>().onClick.AddListener(() => Select(index));
            NewItem.SetActive(true);
        }

        #region Show Text

        private IEnumerator Activate_List(List<DialogData> DataList)
        {
            state = State.Active;

            foreach (var Data in DataList)
            {
                Show(Data);
                _init_selector();

                while (state != State.Deactivate)
                {
                    yield return null;
                }
            }
        }

        private IEnumerator Activate()
        {
            _initialize();

            state = State.Active;

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
                        yield return new WaitForSeconds(float.Parse(item.Argument));
                        break;
                }
            }

            state = State.Wait;
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
                while (state != State.Wait)
                    yield return null;
                _currentDelay = Delay;
            }
        }

        #endregion
    }
}