using UnityEngine;
using System.Collections;
using System;

public class BetaTextCommand : SceneCommand {

    private string text;

    public BetaTextCommand(string text) {
        this.text = text;
    }

    public void performAction(SceneManagerComponent sceneManager, Action onFinish) {
        sceneManager.textbox.ShowText(text, onFinish);
    }
}
