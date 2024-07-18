namespace Doublsb.Dialog
{
    using System;
    using System.Globalization;

    public class SpeedCommand : TempValueChangeCommand<float>
    {
        private readonly IPrinter _printer;

        protected override float Value
        {
            get => _printer.Delay;
            set => _printer.Delay = value;
        }

        private const float DefaultDelay = 0.02f;

        public SpeedCommand(IPrinter printer, string delay)
        {
            _printer = printer;
            NewValue = ParseSpeed(delay);
        }

        private float ParseSpeed(string text)
        {
            var newValue = Value;
            switch (text)
            {
                case "up":
                    newValue -= 0.25f;
                    if (newValue <= 0)
                        newValue = 0.001f;
                    break;

                case "down":
                    newValue += 0.25f;
                    break;

                case "init" or "end":
                    newValue = DefaultDelay;
                    break;

                default:
                    if (float.TryParse(text, NumberStyles.Any,
                            CultureInfo.InvariantCulture, out var parsedSpeed))
                        newValue = parsedSpeed;
                    else
                        throw new Exception($"Cannot parse float number: {text}");
                    break;
            }

            return newValue;
        }
    }
}