using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using QuickEye.PeeDialog;

public class TestMessage : MonoBehaviour
{
    public DialogSystemComponent dialogSystem;

    public GameObject[] examples;

    private CancellationTokenSource _disableCancellationTokenSource;
    private CancellationToken CancellationToken => _disableCancellationTokenSource.Token;

    private void OnEnable()
    {
        _disableCancellationTokenSource?.Dispose();
        _disableCancellationTokenSource = new CancellationTokenSource();
    }

    [ContextMenu("Run")]
    private void Start()
    {
        RunDialog().Forget();
    }

    private async UniTaskVoid RunDialog()
    {
        
        await Say("Li",
            "<speed=0.2>. . .</speed><color=#7BA6FA> Hello!, <color=orange>I'm</color> Li</color>. I'm here to explain the basic features of the<wait=.3/><emote=Happy/><sound=haha/><wait=1/><size=+50/> Pee Dialog.");
        await Say("Sa", "You can easily change text <color=red>color</color>, and <size=+30>size</size> like this.");
        ShowExample(0);
        await Say("Li", "Just<wait=1/> put the command in the string!");
        ShowExample(1);
        await Say("Li",
            "You can also change the character's sprite <emote=Sad/>like this, <click/><emote=Happy/>Smile.");
        ShowExample(2);
        await Say("Li", "If you need an emphasis effect, <wait=0.5/>wait... <click/>or click command.");
        ShowExample(3);
        await Say("Li", "Text can be <speed=down>slow... </speed><speed=up/>or fast.");
        ShowExample(4);
        await Say("Li", "And here we go, the haha sound! <click/><sound=haha/>haha.");
        await Say("Li", "That's it! Please check the documents. Good luck to you.");
    }

    private void OnDisable()
    {
        _disableCancellationTokenSource.Cancel();
    }

    private async UniTask Say(string actor, string script)
    {
        var scriptWithActor = $"<actor={actor}>{script}</actor>";
        await dialogSystem.DialogEngine.Print(scriptWithActor, CancellationToken);
    }

    private void ShowExample(int index)
    {
        examples[index].SetActive(true);
    }
}