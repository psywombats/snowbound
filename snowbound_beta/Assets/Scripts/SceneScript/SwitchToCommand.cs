using UnityEngine;
using System.Collections;
using System;

public class SwitchToCommand : SceneCommand {

    private string key;

    public SwitchToCommand(string targetCharaKey) {
        this.key = targetCharaKey;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        yield return player.textbox.FadeOut(0.5f);
        yield return player.paragraphBox.FadeIn(0.5f);
        yield return player.paragraphBox.ShowText(player, "~~~ NOW PLAYING FROM " + key + "'s PERSPECTIVE ~~~");
        yield return Global.Instance().input.AwaitAdvance();
    }
}
