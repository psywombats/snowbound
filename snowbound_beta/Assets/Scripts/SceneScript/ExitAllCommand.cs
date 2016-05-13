using UnityEngine;
using System.Collections;
using System;

public class ExitAllCommand : StageDirectionCommand {

    public bool ClosesTextboxes { get; set; }

    public override IEnumerator PerformAction(ScenePlayer player) {
        if (ClosesTextboxes) {
            yield return Utils.RunParallel(new[] {
                    player.textbox.FadeOut(),
                    player.paragraphBox.FadeOut()
            }, player);
        }

        if (!player.portraits.AnyVisible()) {
            yield return null;
        } else {
            if (synchronous) {
                yield return player.StartCoroutine(player.portraits.FadeOutAll());
            } else {
                yield return null;
                player.StartCoroutine(player.portraits.FadeOutAll());
            }
        }
    }
}
