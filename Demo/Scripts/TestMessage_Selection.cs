using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TestMessage_Selection : MonoBehaviour
{
    public DialogPrinter dialogPrinter;

    private void Awake()
    {
        var dialogTexts = new List<DialogCommandSet>();

        var Text1 = new DialogCommandSet("What is 2 times 5?");
        Text1.SelectList.Add(new MenuOption("10", OnCorrectOption));
        Text1.SelectList.Add(new MenuOption("8", OnWrongOption));
        Text1.SelectList.Add(new MenuOption("Why should I care?", OnOtherOption));
        
        dialogTexts.Add(Text1);

        dialogPrinter.Run(dialogTexts);
    }

    private void OnOtherOption()
    {
        var dialogTexts = new List<DialogCommandSet>();

        dialogTexts.Add(new DialogCommandSet("Right. You don't have to get the answer."));

        dialogPrinter.Run(dialogTexts);
    }

    private void OnWrongOption()
    {
        var dialogTexts = new List<DialogCommandSet>();

        dialogTexts.Add(new DialogCommandSet("You are wrong."));

        dialogPrinter.Run(dialogTexts);
    }

    private void OnCorrectOption()
    {
        var dialogTexts = new List<DialogCommandSet>();

        dialogTexts.Add(new DialogCommandSet("You are right."));

        dialogPrinter.Run(dialogTexts);
    }
}
