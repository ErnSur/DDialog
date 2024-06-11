namespace Doublsb.Dialog
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.UI;

    public class DefaultActorManager : MonoBehaviour, IDialogActorManager
    {
        [SerializeField]
        private GameObject characters;

        private Character _speakingActor;

        string IDialogActorManager.SpeakingActor => _speakingActor == null ? string.Empty : _speakingActor.name;

        public bool TryGetActorSound(string soundId, out AudioClip clip)
        {
            return _speakingActor.Sounds.TryGetValue(soundId, out clip);
        }

        public bool TryGetChatSoundEffects(out AudioClip[] clips)
        {
            if (_speakingActor != null && _speakingActor.ChatSE is { Length: > 0 })
            {
                clips = _speakingActor.ChatSE;
                return true;
            }

            clips = null;
            return false;
        }

        [Button]
        public void Show(string actorId)
        {
            if (string.IsNullOrEmpty(actorId))
            {
                Debug.LogError("Trying to show actor without ID.");
                return;
            }

            if(!TryGetActor(actorId, out _speakingActor))
            {
                Debug.LogError($"Actor not found: {actorId}");
                return;
            }

            characters.SetActive(true);
            foreach (Transform item in characters.transform)
                item.gameObject.SetActive(false);
            if (_speakingActor != null)
                _speakingActor.gameObject.SetActive(true);
            Emote("Normal");
        }

        private bool TryGetActor(string actorId, out Character actor)
        {
            var go = characters.transform.Find(actorId);
            if(go == null)
            {
                actor = null;
                return false;
            }
            
            actor = go.GetComponent<Character>();
            return actor != null;
        }

        public void HideAll()
        {
            _speakingActor = null;
            characters.SetActive(false);
        }

        public void Emote(string emotion)
        {
            if (_speakingActor == null)
            {
                Debug.LogError($"No actor to emote {emotion}. Show actor first.");
                return;
            }

            if (_speakingActor.Emotions.TryGetValue(emotion, out var sprite))
                _speakingActor.GetComponent<Image>().sprite = sprite;
            else
                Debug.LogError($"Emotion not found: {emotion} for {_speakingActor}");
        }
    }
}