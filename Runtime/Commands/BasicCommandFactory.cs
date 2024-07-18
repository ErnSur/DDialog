namespace Doublsb.Dialog
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(IPrinter))]
    public class BasicCommandFactory : MonoBehaviour, ICommandFactory
    {
        private IPrinter _printer;

        [SerializeField]
        private UnityDictionary<string, AudioClip> sounds;

        [SerializeField]
        private AudioSource audioSource;

        private DefaultActorManager _actorManager;

        private void Awake()
        {
            _printer = GetComponent<IPrinter>();
            _actorManager = GetComponent<DefaultActorManager>();
        }

        bool ICommandFactory.TryGetCommand(string commandId, string[] args, ICommand parent, out ICommand command)
        {
            var arg1 = args.FirstOrDefault();
            switch (commandId)
            {
                case "actor":
                    // make it write to actor  manager
                    command = new ActorCommand(_actorManager, arg1);
                    return true;
                case "print":
                    command = new PrintCommand(_printer, arg1);
                    return true;
                case "size":
                    command = new SizeCommand(_printer, arg1);
                    return true;
                case "color":
                    command = new ColorCommand(_printer, arg1);
                    return true;
                case "wait":
                    command = new WaitCommand(arg1);
                    return true;
                case "speed":
                    command = new SpeedCommand(_printer, arg1);
                    return true;
                case "sound" when arg1 != null && sounds.TryGetValue(arg1, out var clip):
                    command = new SoundCommand(clip, audioSource);
                    return true;
                case "emote":
                    if (parent.Root is ActorCommand actor)
                    {
                        command = new FuncCommand(() => _actorManager.Emote(actor.ActorId, arg1));
                        return true;
                    }
                    command = null;
                    return false;
                case "click":
                    command = new FuncCommand(async ct =>
                    {
                        while (!Input.GetMouseButtonDown(0) && !ct.IsCancellationRequested)
                            await UniTask.NextFrame(ct);
                    });
                    return true;
                default:
                    command = null;
                    return false;
            }
        }
    }
}