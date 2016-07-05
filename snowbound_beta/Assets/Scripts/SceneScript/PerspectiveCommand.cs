using UnityEngine;
using System.Collections;
using System;

public class PerspectiveCommand : SceneCommand {

    private SpriteEffectComponent effect;

    private string targetCharaKey;
    private string newBackgroundTag;
    private string text;

    public PerspectiveCommand(string targetCharaKey, string newBackgroundTag, string text) {
        this.targetCharaKey = targetCharaKey;
        this.newBackgroundTag = newBackgroundTag;
        this.text = text;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        effect = player.GetEffect();

        yield return player.StartCoroutine(player.paragraphBox.Deactivate(player));
        yield return player.StartCoroutine(player.textbox.Deactivate(player));

        yield return player.ExecuteTransition("whiteout_in", () => {
            player.StartCoroutine(effect.StartWhiteoutRoutine(0.0f));
        });
        yield return new WaitForSeconds(1.0f);

        TachiComponent tachi = player.portraits.GetPortraitBySlot("D");
        yield return player.StartCoroutine(Utils.RunParallel(new[] {
            tachi.FadeCharaIn(targetCharaKey, player.fades.GetData("fade_long")),
            effect.FadeLetterboxesIn()
        }, player));

        effect.HideLetterboxes();
        yield return player.StartCoroutine(effect.FadeLetterboxesIn());

        yield return new WaitForSeconds(10.0f);

        yield return player.ExecuteTransition("whiteout_out", () => {
            effect.HideLetterboxes();
            player.StartCoroutine(effect.StopWhiteoutRoutine(0.0f));
            if (newBackgroundTag != null) {
                player.background.SetBackground(newBackgroundTag);
            }
        });
    }
}
