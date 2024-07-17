namespace Doublsb.Dialog
{
    using System;
    using System.Text.RegularExpressions;

    internal class DialogFormat
    {
        public string DefaultSize = "60";
        private string _defaultColor = "white";

        private string _color;
        private string _size;


        //================================================
        //Public Method
        //================================================
        public DialogFormat(string defaultSize = "", string defaultColor = "")
        {
            _color = string.Empty;
            _size = string.Empty;

            if (defaultSize != string.Empty)
                DefaultSize = defaultSize;
            if (defaultColor != string.Empty)
                _defaultColor = defaultColor;
        }

        public string Color
        {
            set
            {
                if (isColorValid(value))
                {
                    _color = value;
                    if (_size == string.Empty)
                        _size = DefaultSize;
                }
            }

            get => _color;
        }

        public string Size
        {
            set
            {
                if (isSizeValid(value))
                {
                    _size = value;
                    if (_color == string.Empty)
                        _color = _defaultColor;
                }
            }

            get => _size;
        }

        public string OpenTagger
        {
            get
            {
                if (isValid)
                    return $"<color={Color}><size={Size}>";
                else
                    return string.Empty;
            }
        }

        public string CloseTagger
        {
            get
            {
                if (isValid)
                    return "</size></color>";
                else
                    return string.Empty;
            }
        }

        public void Resize(string command)
        {
            if (_size == string.Empty)
                Size = DefaultSize;

            switch (command)
            {
                case "up":
                    _size = (int.Parse(_size) + 10).ToString();
                    break;

                case "down":
                    _size = (int.Parse(_size) - 10).ToString();
                    break;

                case "init":
                    _size = DefaultSize;
                    break;

                default:
                    _size = command;
                    break;
            }
        }

        //================================================
        //Private Method
        //================================================
        private bool isValid => _color != string.Empty && _size != string.Empty;

        private bool isColorValid(string Color)
        {
            var hexColor = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$");

            return Enum.TryParse(Color, out TextColor _) || hexColor.Match(Color).Success;
        }

        private bool isSizeValid(string Size)
        {
            return float.TryParse(Size, out _);
        }
    }
}