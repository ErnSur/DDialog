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

        private BasicActorManager _actorManager;
        private string _lastActorId;

        private void Awake()
        {
            _printer = GetComponent<IPrinter>();
            _actorManager = GetComponent<BasicActorManager>();
        }

        bool ICommandFactory.TryGetCommand(string commandId, string[] args,  out ICommand command)
        {
            var arg1 = args.FirstOrDefault();
            switch (commandId)
            {
                case "actor":
                    // make it write to actor  manager
                    command = new ActorCommand(_actorManager, arg1);
                    _lastActorId = arg1;
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
                        command = new FuncCommand(() => _actorManager.Emote(_lastActorId, arg1));
                        return true;
                case "click":
                    command = new FuncCommand(async ct =>
                    {
                        while (true)
                        {
                            await UniTask.NextFrame(ct);
                            if (Input.GetMouseButtonDown(0) || ct.IsCancellationRequested)
                                break;
                        }
                    });
                    return true;
                default:
                    command = null;
                    return false;
            }
        }
    }
}