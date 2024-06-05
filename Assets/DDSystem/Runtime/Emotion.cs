namespace Doublsb.Dialog
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Emotion
    {
        //================================================
        //Public Variable
        //================================================
        private Dictionary<string, Sprite> _data;
        public Dictionary<string, Sprite> Data
        {
            get
            {
                if (_data == null) _init_emotionList();
                return _data;
            }
        }

        public string[] _emotion = new string[] { "Normal" };
        public Sprite[] _sprite;

        //================================================
        //Private Method
        //================================================
        private void _init_emotionList()
        {
            _data = new Dictionary<string, Sprite>();

            if (_emotion.Length != _sprite.Length)
                Debug.LogError("Emotion and Sprite have different lengths");

            for (int i = 0; i < _emotion.Length; i++)
                _data.Add(_emotion[i], _sprite[i]);
        }
    }
}