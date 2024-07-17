namespace Doublsb.Dialog
{
    using UnityEngine;

    public class CommandCollection : MonoBehaviour
    {
        [SerializeReference]
        private ICommand[] commandHandlers = new ICommand[]
        {
            new PrintCommand(),
            new SizeCommand()
        };
    }
}