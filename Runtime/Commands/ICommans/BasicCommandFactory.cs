namespace Doublsb.Dialog
{
    using System.Linq;
    using UnityEngine;

    public class EmoteCommandFactory : MonoBehaviour, ICommandFactory
    {
        public bool TryGetCommand(string commandId, string[] args, out ICommand command)
        {
            throw new System.NotImplementedException();
        }
        
        public string Identifier => "e";
        
        private DefaultActorManager _actorManager;

        private void Awake()
        {
            _actorManager = GetComponent<DefaultActorManager>();
        }

        // attributes could be used for handling commands?
        // [CommandHandler("emote")]
        // But then how do you handle dynamic command identifiers? do such command should exist even?
        // for something like emoji recognition they should though
        public IEnumerator PerformAction(string emoteId, ActorLines actorLines, CancellationToken fastForwardToken)
        {
            _actorManager.Emote(actorLines.ActorId, emoteId);
            yield break;
        }
    }

    [RequireComponent(typeof(IPrinter))]
    public class BasicCommandFactory : MonoBehaviour, ICommandFactory
    {
        private IPrinter _printer;

        [SerializeField]
        private UnityDictionary<string, AudioClip> sounds;

        [SerializeField]
        private AudioSource audioSource;

        private void Awake()
        {
            _printer = GetComponent<IPrinter>();
        }

        public bool TryGetCommand(string commandId, string[] args, out ICommand command)
        {
            var arg1 = args.FirstOrDefault();
            switch (commandId)
            {
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
                default:
                    command = null;
                    return false;
            }
        }
    }
}