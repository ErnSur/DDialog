using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using QuickEye.PeeDialog;

public class TestMessage_Selection : MonoBehaviour
{
    //public DialogSystem dialogSystem;

    private void Awake()
    {
        // var dialogTexts = new List<ActorLines>();
        //
        // var Text1 = new ActorLines("What is 2 times 5?");
        // Text1.SelectList.Add(new MenuOption("10", OnCorrectOption));
        // Text1.SelectList.Add(new MenuOption("8", OnWrongOption));
        // Text1.SelectList.Add(new MenuOption("Why should I care?", OnOtherOption));
        //
        // dialogTexts.Add(Text1);

        //dialogSystem.Run(dialogTexts).Forget();
    }

    private void OnOtherOption()
    {
        // var dialogTexts = new List<ActorLines>();
        //
        // dialogTexts.Add(new ActorLines("Right. You don't have to get the answer."));

        //dialogSystem.Run(dialogTexts).Forget();
    }

    private void OnWrongOption()
    {
        // var dialogTexts = new List<ActorLines>();
        //
        // dialogTexts.Add(new ActorLines("You are wrong."));

        //dialogSystem.Run(dialogTexts);
    }

    private void OnCorrectOption()
    {
        // var dialogTexts = new List<ActorLines>();
        //
        // dialogTexts.Add(new ActorLines("You are right."));

        //dialogSystem.Run(dialogTexts);
    }
}
