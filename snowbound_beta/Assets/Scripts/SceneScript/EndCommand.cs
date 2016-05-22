using UnityEngine;
using System.Collections;
using System;

public class EndCommand : SceneCommand {

    private string endingKey;
    
    public EndCommand(string endingKey) {
        this.endingKey = endingKey;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        yield return player.textbox.FadeOut(0.5f);
        yield return player.paragraphBox.FadeIn(0.5f);
        yield return player.paragraphBox.ShowText(player, "ENDING " + endingKey);
        yield return Global.Instance().input.AwaitInput();
        Application.Quit();
    }
}
