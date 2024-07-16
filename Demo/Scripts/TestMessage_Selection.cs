using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TestMessage_Selection : MonoBehaviour
{
    public DialogPrinter dialogPrinter;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        var Text1 = new DialogData("What is 2 times 5?");
        Text1.SelectList.Add(new MenuOption("10", OnCorrectOption));
        Text1.SelectList.Add(new MenuOption("8", OnWrongOption));
        Text1.SelectList.Add(new MenuOption("Why should I care?", OnOtherOption));
        
        dialogTexts.Add(Text1);

        dialogPrinter.Show(dialogTexts);
    }

    private void OnOtherOption()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("Right. You don't have to get the answer."));

        dialogPrinter.Show(dialogTexts);
    }

    private void OnWrongOption()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("You are wrong."));

        dialogPrinter.Show(dialogTexts);
    }

    private void OnCorrectOption()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("You are right."));

        dialogPrinter.Show(dialogTexts);
    }
}
