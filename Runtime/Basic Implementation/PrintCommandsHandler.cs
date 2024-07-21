namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [DefaultExecutionOrder(DefaultExecutionOrder)]
    [RequireComponent(typeof(ICommandRunnerProvider))]
    public class PrintCommandsHandler : MonoBehaviour
    {
        public const int DefaultExecutionOrder = 1000;
        protected IPrinter Printer;
        protected CommandRunner CommandRunner;

        protected virtual void Awake()
        {
            Printer = GetComponent<IPrinter>();
            CommandRunner = GetComponent<ICommandRunnerProvider>().CommandRunner;
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            RegisterActor();
            RegisterPrint();
            RegisterSize();
            RegisterColor();
            RegisterSpeed();
            RegisterWait();
            RegisterClick();
        }

        private void RegisterActor()
        {
            CommandRunner.RegisterCommandCallback("actor", async (args, token) =>
                {
                    Printer.Reset();
                    Printer.SetActive(true);
                },
                async (args, token) =>
                {
                    using var skipCts = BasicPrinter.CreateSkipCts(token);
                    await UniTask.WaitUntilCanceled(skipCts.Token);
                    Printer.SetActive(false);
                    Printer.Reset();
                });
        }

        protected virtual void RegisterClick()
        {
            CommandRunner.RegisterCommandCallback("click", async (args, token) =>
            {
                using var skipCts = BasicPrinter.CreateSkipCts(token);
                await UniTask.WaitUntilCanceled(skipCts.Token);
            });
        }

        protected virtual void RegisterWait()
        {
            CommandRunner.RegisterCommandCallback("wait", async (args, token) =>
            {
                if (!float.TryParse(args.FirstOrDefault(),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var waitTime))
                {
                    Debug.LogError($"Cannot parse float number: {waitTime}");
                    return;
                }
                if (token.IsCancellationRequested)
                    return;
 
                using var skipCts = BasicPrinter.CreateSkipCts(token);
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: skipCts.Token);
            });
        }

        protected virtual void RegisterSpeed()
        {
            var previousDelays = new Stack<float>();
            CommandRunner.RegisterCommandCallback("speed", async (args, token) =>
                {
                    var newSpeed = float.TryParse(args.FirstOrDefault(), out var speed) ? speed : 0.1f;
                    previousDelays.Push(Printer.Delay);
                    Printer.Delay = newSpeed;
                },
                async (args, token) =>
                {
                    Printer.Delay = previousDelays.Pop();
                });
        }

        protected virtual void RegisterColor()
        {
            var previousColors = new Stack<Color>();
            
            CommandRunner.RegisterCommandCallback("color", async (args, token) =>
                {
                    var newColor = ColorUtility.TryParseHtmlString(args.FirstOrDefault(), out var color)
                        ? color
                        : Color.white;
                    previousColors.Push(Printer.TextColor);
                    Printer.TextColor = newColor;
                },
                async (args, token) =>
                {
                    Printer.TextColor = previousColors.Pop();
                });
        }

        protected virtual void RegisterPrint()
        {
            CommandRunner.RegisterCommandCallback("print", async (args, token) =>
            {
                var text = args.FirstOrDefault();
                await Printer.Print(text, token);
            });
        }

        protected virtual void RegisterSize()
        {
            var previousFontSizes = new Stack<FontSize>();
            CommandRunner.RegisterCommandCallback("size", async (args, token) =>
                {
                    var newSize = FontSize.ParseString(args.FirstOrDefault());
                    previousFontSizes.Push(Printer.TextSize);
                    Printer.TextSize = newSize;
                },
                async (args, token) =>
                {
                    Printer.TextSize = previousFontSizes.Pop();
                });
        }
    }
}