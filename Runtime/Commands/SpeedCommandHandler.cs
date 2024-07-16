namespace Doublsb.Dialog
{
    using System;
    using System.Collections;
    using System.Globalization;
    using UnityEngine;

    [RequireComponent(typeof(DialogManager))]
    public class SpeedCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public float defaultDelay = 0.02f;

        private DialogManager _dialogManager;

        private float CurrentDelay
        {
            get => _dialogManager.delay;
            set => _dialogManager.delay = value;
        }
        
        public string Identifier => "speed";

        private void Awake()
        {
            _dialogManager = GetComponent<DialogManager>();
            _dialogManager.actorLineStarted.AddListener(OnActorLineStart);
        }

        IEnumerator IDialogCommandHandler.PerformAction(string context, DialogData dialogData)
        {
            SetSpeed(context);
            yield break;
        }

        private void OnActorLineStart(string _)
        {
            CurrentDelay = defaultDelay;
        }

        private void SetSpeed(string speed)
        {
            switch (speed)
            {
                case "up":
                    CurrentDelay -= 0.25f;
                    if (CurrentDelay <= 0)
                        CurrentDelay = 0.001f;
                    break;

                case "down":
                    CurrentDelay += 0.25f;
                    break;

                case "init" or "end":
                    CurrentDelay = defaultDelay;
                    break;

                default:
                    if (float.TryParse(speed, NumberStyles.Any,
                            CultureInfo.InvariantCulture, out var parsedSpeed))
                        CurrentDelay = parsedSpeed;
                    else
                        throw new Exception($"Cannot parse float number: {speed}");
                    break;
            }
        }
    }
}