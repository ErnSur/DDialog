using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TestMessage : MonoBehaviour
{
    public DialogSystem dialogSystem;

    public GameObject[] Example;

    private void Awake()
    {
        var dialogTexts = new List<ActorLines>();

        //dialogTexts.Add(new ActorLines("You can easily change text <color=red>color</color>, and <size=+30>size</size> like this.", "Li", () => Show_Example(0)));

        //dialogTexts.Add(new ActorLines("Just put the command in the string!", "Li", () => Show_Example(1)));

        //dialogTexts.Add(new ActorLines("You can also change the character's sprite <emote=Sad/>like this, <click/><emote=Happy/>Smile.", "Li",  () => Show_Example(2)));

        //dialogTexts.Add(new ActorLines("If you need an emphasis effect, <wait=0.5/>wait... <click/>or click command.", "Li", () => Show_Example(3)));

        //dialogTexts.Add(new ActorLines("Text can be <speed=down>slow... </speed><speed=up/>or fast.", "Li", () => Show_Example(4)));

        //dialogTexts.Add(new ActorLines("You don't even need to click on the window like this...<speed=0.1/> tada!<close/>", "Li", () => Show_Example(5)));

        //dialogTexts.Add(new ActorLines("<speed=0.1/>AND YOU CAN'T SKIP THIS SENTENCE.", "Li", () => Show_Example(6), false));

        dialogTexts.Add(new ActorLines("And here we go, the haha sound! <click/><sound=haha/>haha.", "Li", null, false));

        dialogTexts.Add(new ActorLines("That's it! Please check the documents. Good luck to you.", "Sa"));

        dialogSystem.Run(dialogTexts);
    }

    private void Show_Example(int index)
    {
        Example[index].SetActive(true);
    }
}
