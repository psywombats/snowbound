using UnityEngine;
using System.Collections;
using System;

public class SwitchToCommand : SceneCommand {

    private SpriteEffectComponent effect;

    private string targetCharaKey;
    private string newBackgroundTag;
    private bool flip;

    public SwitchToCommand(string targetCharaKey, string newBackgroundTag) {
        this.targetCharaKey = targetCharaKey;
        this.newBackgroundTag = newBackgroundTag;
        flip = newBackgroundTag.ToLower().Equals("eric");
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        effect = player.GetEffect();

        yield return player.StartCoroutine(player.paragraphBox.Deactivate(player));
        yield return player.StartCoroutine(player.textbox.Deactivate(player));

        yield return player.ExecuteTransition("whiteout_in", () => {
            player.StartCoroutine(effect.StartWhiteoutRoutine(0.0f));
        });

        yield return new WaitForSeconds(1.5f);
        TachiComponent tachi = player.portraits.GetPortraitBySlot(flip ? "D": "B");
        tachi.SetChara(targetCharaKey);

        yield return player.ExecuteTransition("whiteout_out", () => {
            player.StartCoroutine(effect.StopWhiteoutRoutine(0.0f));
            if (newBackgroundTag != null) {
                player.background.SetBackground(newBackgroundTag);
            }
        });
    }
}
