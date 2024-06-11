using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Doublsb.Dialog
{
    public class DialogManager : MonoBehaviour
    {
        //================================================
        //Public Variable
        //================================================
        [Header("Game Objects")]
        public GameObject Printer;

        private IDialogActorManager _actorManager;

        [Header("UI Objects")]
        public Text Printer_Text;

        [Header("Audio Objects")]
        public AudioSource SEAudio;

        public AudioSource CallAudio;

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

        //================================================
        //Private Method
        //================================================
        private DialogData _current_Data;

        private float _currentDelay;
        private float _lastDelay;
        private Coroutine _textingRoutine;
        private Coroutine _printingRoutine;

        public AudioClip[] defaultChatSoundEffects;

        private Dictionary<string, IDialogCommandHandler> _commandHandlers = new()
        {
            { "size", new SizeCommandHandler() },
            { "color", new ColorCommandHandler() },
            // {"speed", new SpeedCommandHandler()},
            // {"click", new ClickCommandHandler()},
            // {"close", new CloseCommandHandler()},
            // {"wait", new WaitCommandHandler()}
        };

        // private void Awake()
        // {
        //     _commandHandlers = GetComponents<IDialogCommandHandler>().ToDictionary(handler => handler.Identifier);
        //     _commandHandlers["size"] = new SizeCommandHandler();
        // }

        //================================================
        //Public Method
        //================================================

        #region Show & Hide

        public void Show(DialogData Data)
        {
            _current_Data = Data;
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
                    if (_current_Data.SelectList.Count <= 0)
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

            if (_current_Data.Callback != null)
            {
                _current_Data.Callback.Invoke();
                _current_Data.Callback = null;
            }
        }

        #endregion

        #region Selector

        public void Select(int index)
        {
            Result = _current_Data.SelectList.GetByIndex(index).Key;
            Hide();
        }

        #endregion

        #region Sound

        public void Play_ChatSE()
        {
            var clips = defaultChatSoundEffects;
            if (_actorManager.TryGetChatSoundEffects(out var actorClips))
                clips = actorClips;
            if (_current_Data.ChatSoundEffects is { Length: > 0 })
                clips = _current_Data.ChatSoundEffects;
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
            _actorManager = GetComponent<IDialogActorManager>();
            
            if (!_commandHandlers.ContainsKey("emote"))
                _commandHandlers["emote"] = gameObject.GetComponent<EmoteCommandHandler>();
            if(!_commandHandlers.ContainsKey("sound"))
                _commandHandlers["sound"] = gameObject.GetComponent<SoundCommandHandler>();  
            _currentDelay = Delay;
            _lastDelay = 0.1f;
            Printer_Text.text = string.Empty;

            Printer.SetActive(true);

            _actorManager.Show(_current_Data.Character);
        }

        private void _init_selector()
        {
            _clear_selector();

            if (_current_Data.SelectList.Count > 0)
            {
                Selector.SetActive(true);

                for (int i = 0; i < _current_Data.SelectList.Count; i++)
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
            SelectorItemText.text = _current_Data.SelectList.GetByIndex(index).Value;

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

            foreach (var item in _current_Data.Commands)
            {
                if (_commandHandlers.TryGetValue(item.Command.ToString(), out var handler))
                {
                    yield return handler.PerformAction(item.Context, _current_Data);
                    Debug.Log($"Command {item.Command} Activated!!");
                    continue;
                }

                switch (item.Command)
                {
                    case Command.print:
                        yield return _printingRoutine = StartCoroutine(_print(item.Context));
                        break;

                    case Command.speed:
                        Set_Speed(item.Context);
                        break;

                    case Command.click:
                        yield return _waitInput();
                        break;

                    case Command.close:
                        Hide();
                        yield break;

                    case Command.wait:
                        yield return new WaitForSeconds(float.Parse(item.Context));
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
            _current_Data.PrintText += _current_Data.Format.OpenTagger;

            for (int i = 0; i < Text.Length; i++)
            {
                _current_Data.PrintText += Text[i];
                Printer_Text.text = _current_Data.PrintText + _current_Data.Format.CloseTagger;

                if (Text[i] != ' ')
                    Play_ChatSE();
                if (_currentDelay != 0)
                    yield return new WaitForSeconds(_currentDelay);
            }

            _current_Data.PrintText += _current_Data.Format.CloseTagger;
        }

        private IEnumerator _skip()
        {
            if (_current_Data.isSkippable)
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