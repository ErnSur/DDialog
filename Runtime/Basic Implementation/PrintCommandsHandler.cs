namespace QuickEye.PeeDialog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine;

    [PublicAPI]
    public class PrintCommandsHandler
    {
        protected IPrinter Printer;
        protected CommandRunner CommandRunner;

        public PrintCommandsHandler(CommandRunner commandRunner, IPrinter printer)
        {
            CommandRunner = commandRunner;
            Printer = printer;
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
            RegisterBold();
        }

        protected virtual void RegisterActor()
        {
            CommandRunner.RegisterCommandCallback("actor",
                                                  async (args, token) =>
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
            CommandRunner.RegisterCommandCallback("click",
                                                  async (args, token) =>
                                                  {
                                                      using var skipCts = BasicPrinter.CreateSkipCts(token);
                                                      await UniTask.WaitUntilCanceled(skipCts.Token);
                                                  });
        }

        protected virtual void RegisterWait()
        {
            CommandRunner.RegisterCommandCallback("wait",
                                                  async (args, token) =>
                                                  {
                                                      if (!float.TryParse(args.FirstOrDefault(),
                                                                          System.Globalization.NumberStyles.Any,
                                                                          System.Globalization.CultureInfo
                                                                              .InvariantCulture,
                                                                          out var waitTime))
                                                      {
                                                          Debug.LogError($"Cannot parse float number: {waitTime}");
                                                          return;
                                                      }

                                                      if (token.IsCancellationRequested)
                                                          return;

                                                      using var skipCts = BasicPrinter.CreateSkipCts(token);
                                                      await UniTask.Delay(TimeSpan.FromSeconds(waitTime),
                                                                          cancellationToken: skipCts.Token);
                                                  });
        }

        protected virtual void RegisterSpeed()
        {
            var previousDelays = new Stack<float>();

            CommandRunner.RegisterCommandCallback("speed", BeginCallback, EndCallback);
            return;

            UniTask BeginCallback(string[] args, CancellationToken token)
            {
                var speedArg = args.FirstOrDefault();

                if (!float.TryParse(speedArg, out var newSpeed))
                {
                    Debug.LogError($"Cannot parse float number: {speedArg}");
                    // Push new value either way because the EndCallback needs to pop it
                    newSpeed = Printer.Delay;
                }

                previousDelays.Push(Printer.Delay);
                Printer.Delay = newSpeed;
                return UniTask.CompletedTask;
            }

            UniTask EndCallback(string[] args, CancellationToken token)
            {
                Printer.Delay = previousDelays.Pop();
                return UniTask.CompletedTask;
            }
        }

        protected virtual void RegisterColor()
        {
            var previousColors = new Stack<Color>();

            CommandRunner.RegisterCommandCallback("color",
                                                  async (args, token) =>
                                                  {
                                                      var newColor =
                                                          ColorUtility.TryParseHtmlString(args.FirstOrDefault(),
                                                              out var color)
                                                              ? color
                                                              : Color.white;

                                                      previousColors.Push(Printer.TextColor);
                                                      Printer.TextColor = newColor;
                                                  },
                                                  async (args, token) => { Printer.TextColor = previousColors.Pop(); });
        }

        protected virtual void RegisterPrint()
        {
            CommandRunner.RegisterCommandCallback("print",
                                                  async (args, token) =>
                                                  {
                                                      var text = args.FirstOrDefault();
                                                      await Printer.Print(text, token);
                                                  });
        }

        protected virtual void RegisterSize()
        {
            var previousFontSizes = new Stack<FontSize>();
            CommandRunner.RegisterCommandCallback("size",
                                                  async (args, token) =>
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

        protected virtual void RegisterBold()
        {
            CommandRunner.RegisterCommandCallback("b",
                                                  async (_, _) => { Printer.Text += "<b>"; },
                                                  async (_, _) => { Printer.Text += "</b>"; });
        }
    }
}