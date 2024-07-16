namespace Doublsb.Dialog
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(DialogPrinter))]
    public class DefaultActorManager : MonoBehaviour, ISoundEffectProvider
    {
        [SerializeField]
        private GameObject characters;

        [SerializeField]
        private string defaultEmotionId = "Normal";

        private DialogPrinter _dialogPrinter;

        private void Awake()
        {
            _dialogPrinter = GetComponent<DialogPrinter>();
            _dialogPrinter.actorLineStarted.AddListener(Show);
            _dialogPrinter.actorLineFinished.AddListener(OnActorLineFinished);
        }
        
        private void OnDestroy()
        {
            _dialogPrinter.actorLineStarted.RemoveListener(Show);
            _dialogPrinter.actorLineFinished.RemoveListener(OnActorLineFinished);
        }

        private void OnActorLineFinished(string _) => HideAll();

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
    }
}