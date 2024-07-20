namespace Doublsb.Dialog
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(DialogSystem))]
    public class BasicActorManager : MonoBehaviour, IActorManager, ISoundEffectProvider
    {
        [SerializeField]
        private GameObject characters;

        [SerializeField]
        private string defaultEmotionId = "Normal";

        private string _activeActorId;

        public string ActiveActorId
        {
            get => _activeActorId;
            set
            {
                if (value == null)
                    EndActorLine();
                _activeActorId = value;
                if (value != null)
                    StartActorLine(value);
            }
        }

        public bool TryGetChatSoundEffects(string actorId, out AudioClip[] clips)
        {
            if (TryGetActor(actorId, out var actor) && actor.ChatSE is { Length: > 0 })
            {
                clips = actor.ChatSE;
                return true;
            }

            clips = null;
            return false;
        }

        public void Emote(string actorId, string emotion)
        {
            if (!TryGetActor(actorId, out var actor))
            {
                Debug.LogError($"Actor not found: {actorId}");
                return;
            }

            if (!actor.isActiveAndEnabled)
            {
                Debug.LogError($"Actor {actorId} is not shown. Show the actor before emoting {emotion}.");
                return;
            }

            if (actor.Emotions.TryGetValue(emotion, out var sprite))
                actor.GetComponent<Image>().sprite = sprite;
            else
                Debug.LogError($"Emotion not found: {emotion} for {actorId}");
        }

        private void Show(string actorId)
        {
            if (string.IsNullOrEmpty(actorId))
            {
                Debug.LogError("Trying to show actor without ID.");
                return;
            }

            if (!TryGetActor(actorId, out var actor))
            {
                Debug.LogError($"Actor not found: {actorId}");
                return;
            }

            characters.SetActive(true);
            foreach (Transform item in characters.transform)
                item.gameObject.SetActive(false);
            if (actor != null)
                actor.gameObject.SetActive(true);
            Emote(actorId, defaultEmotionId);
        }

        private bool TryGetActor(string actorId, out Character actor)
        {
            var go = characters.transform.Find(actorId);
            if (go == null)
            {
                actor = null;
                return false;
            }

            actor = go.GetComponent<Character>();
            return actor != null;
        }

        private void HideAll()
        {
            characters.SetActive(false);
        }

        private void StartActorLine(string actorId)
        {
            Debug.Log($"Starting actor line for {actorId}");
            Show(actorId);
        }

        private void EndActorLine()
        {
            HideAll();
            Debug.Log($"Ended actor line {ActiveActorId}");
        }
    }
}