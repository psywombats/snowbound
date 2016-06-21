﻿using UnityEngine;
using System.Collections;
using System;

public class SwitchToCommand : SceneCommand {

    private string key;

    public SwitchToCommand(string targetCharaKey) {
        this.key = targetCharaKey;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        yield return player.textbox.FadeOutRoutine(player, 0.5f);
        yield return player.paragraphBox.FadeInRoutine(player, 0.5f);
        yield return player.paragraphBox.ShowText(player, "~~~ NOW PLAYING FROM " + key + "'s PERSPECTIVE ~~~");
        yield return Global.Instance().input.AwaitAdvance();
    }
}
