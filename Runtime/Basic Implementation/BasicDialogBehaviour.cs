namespace Doublsb.Dialog
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [RequireComponent(typeof(IPrinter))]
    public class BasicDialogBehaviour : MonoBehaviour, ICommandFactory
    {
        private IPrinter _printer;

        [SerializeField]
        private UnityDictionary<string, AudioClip> sounds;

        [SerializeField]
        private AudioSource audioSource;

        private BasicActorManager _actorManager;
        //private ICommandFactory[] _commandFactories;

        private void Awake()
        {
            _printer = GetComponent<IPrinter>();
            _actorManager = GetComponent<BasicActorManager>();
            //_commandFactories = GetComponents<ICommandFactory>().Except(new[]{this}).ToArray();
        }

        bool ICommandFactory.TryGetCommand(string commandId, string[] args, out ICommand command)
        {
            // TODO: would be cool but right now we cannot gurantee that user with get this Command factory first with `GetCoponent<>`
            // if(_commandFactories.Any(factory => factory.TryGetCommand(commandId, args, out command)))
            //     return true;
            var arg1 = args.FirstOrDefault();
            switch (commandId)
            {
                case "actor":
                    // make it write to actor  manager
                    command = new ActorCommand(_printer, _actorManager, arg1);
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
                    command = new FuncCommand(() => _actorManager.Emote(_actorManager.ActiveActorId, arg1));
                    return true;
                case "click":
                    command = new FuncCommand(async ct =>
                    {
                        using var skipCts = BasicPrinter.CreateSkipCts(ct);
                        await UniTask.WaitUntilCanceled(skipCts.Token);
                    });
                    return true;
                default:
                    command = null;
                    return false;
            }
        }
    }
}