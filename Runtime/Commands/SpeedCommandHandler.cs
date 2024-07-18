namespace Doublsb.Dialog
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Threading;
    using UnityEngine;

    [RequireComponent(typeof(PrintCommandHandler))]
    [RequireComponent(typeof(DialogSystem))]
    public class SpeedCommandHandler : MonoBehaviour, IDialogCommandHandler
    {
        public float defaultDelay = 0.02f;

        private DialogSystem _dialogSystem;
        private PrintCommandHandler _printCommandHandler;

        private float CurrentDelay
        {
            get => _printCommandHandler.Delay;
            set => _printCommandHandler.Delay = value;
        }
        
        public string Identifier => "speed";

        private void Awake()
        {
            _dialogSystem = GetComponent<DialogSystem>();
            _printCommandHandler = GetComponent<PrintCommandHandler>();
            _dialogSystem.actorLineStarted.AddListener(OnActorLineStart);
        }

        IEnumerator IDialogCommandHandler.PerformAction(string context, ActorLines actorLines, CancellationToken fastForwardToken)
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