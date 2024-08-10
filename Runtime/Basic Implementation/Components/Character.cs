using UnityEngine;
using UnityEngine.UI;

namespace QuickEye.PeeDialog
{
    [RequireComponent(typeof(Image))]
    public class Character : MonoBehaviour
    {
        public UnityDictionary<string, Sprite> Emotions;
        public UnityDictionary<string, AudioClip> Sounds;
        public AudioClip[] ChatSE;
        public AudioClip[] CallSE;
    }
}