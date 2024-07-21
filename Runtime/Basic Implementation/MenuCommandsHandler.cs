namespace Doublsb.Dialog
{
    using System.Linq;
    using UnityEngine;

    public class MenuCommandsHandler : MonoBehaviour
    {
        protected CommandRunner CommandRunner;
        protected IDialogMenuView DialogMenuView;
        
        protected virtual void Awake()
        {
            CommandRunner = GetComponent<ICommandRunnerProvider>().CommandRunner;
            DialogMenuView = GetComponent<IDialogMenuView>();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            RegisterMenu();
        }

        private void RegisterMenu()
        {
            CommandRunner.RegisterCommandCallback("menu", async (args, token) =>
            {
                var options = args.FirstOrDefault()?.Split(',');
                var optionSelected = await DialogMenuView.Open(options);
            });
        }
    }
}