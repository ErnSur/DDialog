using UnityEngine;
using UnityEngine.UI;

namespace Doublsb.Dialog
{
    [RequireComponent(typeof(Image))]
    public class Character : MonoBehaviour
    {
        public UnityDictionary<string,Sprite> Emotions;
        public AudioClip[] ChatSE;
        public AudioClip[] CallSE;
    }
}