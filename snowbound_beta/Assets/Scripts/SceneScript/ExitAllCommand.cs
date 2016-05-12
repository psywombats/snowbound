using UnityEngine;
using System.Collections;
using System;

public class ExitAllCommand : StageDirectionCommand {

    public override IEnumerator PerformAction(ScenePlayer player) {
        if (!player.portraits.AnyVisible()) {
            yield return null;
        }
        if (synchronous) {
            yield return player.StartCoroutine(player.portraits.FadeOutAll());
        } else {
            player.StartCoroutine(player.portraits.FadeOutAll());
            yield return null;
        }
    }
}
