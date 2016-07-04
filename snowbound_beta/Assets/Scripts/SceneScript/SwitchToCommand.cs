using UnityEngine;
using System.Collections;
using System;

public class SwitchToCommand : SceneCommand {

    private SpriteEffectComponent effect;

    private const string TransitionInTag = "whiteout_in";
    private const string TransitionOutTag = "whiteout_out";

    public SwitchToCommand(string targetCharaKey) {

    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        effect = player.GetEffect();

        yield return player.StartCoroutine(player.paragraphBox.Deactivate(player));
        yield return player.StartCoroutine(player.textbox.Deactivate(player));

        yield return player.ExecuteTransition(TransitionInTag, () => {
            player.StartCoroutine(effect.StartWhiteoutRoutine(0.0f));
        });

        yield return new WaitForSeconds(3.0f);

        yield return player.ExecuteTransition(TransitionOutTag, () => {
            player.StartCoroutine(effect.StopWhiteoutRoutine(0.0f));
        });
    }
}
