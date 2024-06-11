using UnityEngine;
using UnityEngine.UI;

namespace Doublsb.Dialog
{
    [RequireComponent(typeof(Image))]
    public class Character : MonoBehaviour
    {
        public UnityDictionary<string,Sprite> Emotions;
        public UnityDictionary<string,AudioClip> Sounds;
        public AudioClip[] ChatSE;
        public AudioClip[] CallSE;
    }
}