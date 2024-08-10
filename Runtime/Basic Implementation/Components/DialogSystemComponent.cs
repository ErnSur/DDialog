namespace QuickEye.PeeDialog
{
    using UnityEngine;

    [RequireComponent(typeof(IPrinter))]
    public class DialogSystemComponent : MonoBehaviour, ICommandRunnerProvider
    {
        public DialogEngine DialogEngine { get; private set; }

        public CommandRunner CommandRunner => DialogEngine.CommandRunner;

        private void Awake()
        {
            DialogEngine = new DialogEngine(GetComponent<IPrinter>());
            if (TryGetComponent(out ISoundManager soundManager))
                DialogEngine.AddSoundSupport(soundManager);

            if (TryGetComponent(out IActorManager actorManager))
                DialogEngine.AddActorSupport(actorManager);
        }
    }
}