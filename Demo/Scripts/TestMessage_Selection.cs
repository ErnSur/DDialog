using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doublsb.Dialog;

public class TestMessage_Selection : MonoBehaviour
{
    public DialogManager DialogManager;

    private void Awake()
    {
        var dialogTexts = new List<DialogData>();

        var Text1 = new DialogData("What is 2 times 5?");
        Text1.SelectList.Add(new MenuOption("10", OnCorrectOption));
        Text1.SelectList.Add(new MenuOption("8", OnWrongOption));
        Text1.SelectList.Add(new MenuOption("Why should I care?", OnOtherOption));
        
        dialogTexts.Add(Text1);

        DialogManager.Show(dialogTexts);
    }

    private void OnOtherOption()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("Right. You don't have to get the answer."));

        DialogManager.Show(dialogTexts);
    }

    private void OnWrongOption()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("You are wrong."));

        DialogManager.Show(dialogTexts);
    }

    private void OnCorrectOption()
    {
        var dialogTexts = new List<DialogData>();

        dialogTexts.Add(new DialogData("You are right."));

        DialogManager.Show(dialogTexts);
    }
}
