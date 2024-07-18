namespace Doublsb.Dialog
{
    using System;
    using System.Globalization;
    using UnityEngine;

    /// <summary>
    /// Describes how to interpret a <see cref="FontSize"/> value.
    /// Adjusts the font size. Specify the new size in pixels, font units, or percentage. Pixel adjustments can be absolute (such as 5px) or relative (such as +1 or -1). Relative sizes are based on the original font size, so they’re not cumulative.
    /// </summary>
    public enum FontSizeUnit
    {
        /// <summary>
        /// Interprets size as pixel.
        /// </summary>
        Pixel,
        RelativePixel,

        /// <summary>
        /// Interprets size as a percentage, with 100 representing 100%.
        /// The value is not constrained and can range from negative numbers to values greater than 100.
        /// </summary>
        Percent,
        FontUnit,
    }

    /// <summary>
    /// Represents a distance value.
    /// </summary>
    [Serializable]
    public struct FontSize : IEquatable<FontSize>
    {
        internal const float MaxValue = 8388608.0f;

        /// <summary>
        /// Creates a percentage <see cref="FontSize"/> from a float.
        /// </summary>
        /// <returns>The created size.</returns>
        public static FontSize Percent(float value)
        {
            return new FontSize(value, FontSizeUnit.Percent);
        }

        /// <summary>
        /// Creates a Initial <see cref="FontSize"/>.
        /// </summary>
        /// <returns>Initial size.</returns>
        public static FontSize Initial()
        {
            return new FontSize(0f, FontSizeUnit.RelativePixel);
        }

        /// <summary>
        /// The size value.
        /// </summary>
        public float Value
        {
            get => value;

            // Clamp values to prevent floating point calculation inaccuracies in Yoga.
            set => this.value = Mathf.Clamp(value, -MaxValue, MaxValue);
        }

        /// <summary>
        /// The unit of the value property.
        /// </summary>
        public FontSizeUnit Unit
        {
            get => unit;
            set => unit = value;
        }


        /// <inheritdoc/>
        public FontSize(float value) : this(value, FontSizeUnit.Pixel)
        {
        }

        /// <summary>
        /// Creates from a float and an optional <see cref="FontSizeUnit"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="FontSizeUnit.Pixel"/> is the default unit.
        /// </remarks>
        private FontSize(float value, FontSizeUnit unit) : this()
        {
            Value = value;
            this.unit = unit;
        }

        [SerializeField]
        private float value;

        [SerializeField]
        private FontSizeUnit unit;

        /// <undoc/>
        public static implicit operator FontSize(float value)
        {
            return new FontSize(value, FontSizeUnit.Pixel);
        }

        /// <undoc/>
        public static bool operator ==(FontSize lhs, FontSize rhs)
        {
            return lhs.value == rhs.value && lhs.unit == rhs.unit;
        }

        /// <undoc/>
        public static bool operator !=(FontSize lhs, FontSize rhs)
        {
            return !(lhs == rhs);
        }

        /// <undoc/>
        public bool Equals(FontSize other)
        {
            return other == this;
        }

        /// <undoc/>
        public override bool Equals(object obj)
        {
            return obj is FontSize other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (value.GetHashCode() * 397) ^ (int)unit;
            }
        }

        public override string ToString()
        {
            var valueStr = Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
            var unitStr = string.Empty;
            var prefix = string.Empty;
            switch (unit)
            {
                case FontSizeUnit.Pixel:
                    if (!Mathf.Approximately(0, Value))
                        unitStr = "px";
                    break;
                case FontSizeUnit.Percent:
                    unitStr = "%";
                    break;
                case FontSizeUnit.RelativePixel:
                    if (Value > 0)
                        prefix = "+";
                    break;
                case FontSizeUnit.FontUnit:
                default:
                    break;
            }

            return $"{prefix}{valueStr}{unitStr}";
        }

        public static FontSize ParseString(string str, FontSize defaultValue = default)
        {
            if (string.IsNullOrEmpty(str))
                return defaultValue;

            str = str.ToLowerInvariant().Trim();

            FontSize result;
            
            // Find unit index
            int numberStrtIndex = 0;
            int unitIndex = -1;

            if (str[0] is '-' or '+')
            {
                numberStrtIndex = 1;
            }

            int numberEndIndex = numberStrtIndex;
            for (int i = numberStrtIndex; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsNumber(c) || c == '.')
                {
                    ++numberEndIndex;
                }
                else if (char.IsLetter(c) || c == '%')
                {
                    unitIndex = i;
                    break;
                }
                else
                {
                    // Invalid format
                    return defaultValue;
                }
            }

            var floatStr = str.Substring(0, numberEndIndex);
            string unitStr;
            if (unitIndex > 0)
                unitStr = str.Substring(unitIndex, str.Length - unitIndex);
            else
                unitStr = "px";

            float value = defaultValue.value;
            FontSizeUnit unit = defaultValue.unit;

            if (float.TryParse(floatStr,NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out var v))
                value = v;

            switch (unitStr)
            {
                case "px" when str[0] is '+' or '-':
                    unit = FontSizeUnit.RelativePixel;
                    break;
                case "px":
                    unit = FontSizeUnit.Pixel;
                    break;
                case "%":
                    unit = FontSizeUnit.Percent;
                    break;
            }

            result = new FontSize(value, unit);

            return result;
        }
        
        public static string CStrig(FontSize size) => size.ToString();
    }
}