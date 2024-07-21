using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Doublsb.Dialog;

public class TestMessage : MonoBehaviour
{
    public CommandRunnerComponent dialogSystem;

    public GameObject[] Example;
    
    [ContextMenu("Run")]
    private void OnEnable()
    {
        var dialogTexts = new List<ActorLines>();

        dialogTexts.Add(new ActorLines("You can easily change<color=blue> text <color=red>color</color>, and</color> the <size=+30>size</size> like this.", "Li", () => Show_Example(0)));
        
        dialogTexts.Add(new ActorLines("Just<wait=1/> put the command in the string!", "Li", () => Show_Example(1)));
        
        dialogTexts.Add(new ActorLines("You can also change the character's sprite <emote=Sad/>like this, <click/><emote=Happy/>Smile.", "Li",  () => Show_Example(2)));
        
        dialogTexts.Add(new ActorLines("If you need an emphasis effect, <wait=0.5/>wait... <click/>or click command.", "Li", () => Show_Example(3)));
        
        dialogTexts.Add(new ActorLines("Text can be <speed=down>slow... </speed><speed=up/>or fast.", "Li", () => Show_Example(4)));
        
        dialogTexts.Add(new ActorLines("You don't even need to click on the window like this...<speed=0.1/> tada!<close/>", "Li", () => Show_Example(5)));

        dialogTexts.Add(new ActorLines("<speed=0.25/>AND YOU CAN'T SKIP THIS SENTENCE", "Li", () => Show_Example(6)));

        dialogTexts.Add(new ActorLines("And here we go, the haha sound! <click/><sound=haha/>haha.", "Li"));

        dialogTexts.Add(new ActorLines("That's it! Please check the documents. Good luck to you.", "Sa"));

        Execute(dialogTexts).Forget();
    }

    private async UniTaskVoid Execute(List<ActorLines> dialogTexts)
    {
        foreach (var dialogText in dialogTexts)
        {
            var script = $"<actor={dialogText.ActorId}>{dialogText.Script}</actor>";
            await dialogSystem.CommandRunner.Execute(script, destroyCancellationToken);
        }
    }

    private void Show_Example(int index)
    {
        Example[index].SetActive(true);
    }
}
