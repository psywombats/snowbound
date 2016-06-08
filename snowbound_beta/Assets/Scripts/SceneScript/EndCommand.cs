using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class EndCommand : SceneCommand {

    private const string TitleSceneName = "TitleScene";

    private string endingKey;
    
    public EndCommand(string endingKey) {
        this.endingKey = endingKey;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        yield return player.textbox.FadeOutRoutine(0.5f);
        yield return player.paragraphBox.FadeInRoutine(0.5f);
        yield return player.paragraphBox.ShowText(player, "ENDING " + endingKey);
        yield return Global.Instance().input.AwaitAdvance();

        FadeComponent fade = player.GetFade();
        yield return fade.FadeToBlackRoutine();

        SceneManager.LoadScene(TitleSceneName);
    }
}
