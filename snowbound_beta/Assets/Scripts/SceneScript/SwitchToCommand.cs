using UnityEngine;
using System.Collections;
using System;

public class SwitchToCommand : SceneCommand {

    private SpriteEffectComponent effect;

    private string transitionIntroInTag = "bar_long";
    private string transitionIntroOutTag = "fade_long";
    private string transitionOutroInTag = "fade_long";
    private string transitionOutroOutTag = "bar_long";

    public SwitchToCommand(string targetCharaKey) {

    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        effect = player.GetEffect();
        yield return player.StartCoroutine(effect.StartWhiteoutRoutine(0.0f));
        yield return player.StartCoroutine(player.paragraphBox.Deactivate(player));
        yield return player.StartCoroutine(player.textbox.Deactivate(player));

        yield return player.StartCoroutine(player.ExecuteTransition(player.transitions.GetData(transitionIntroInTag)));
        
        yield return player.StartCoroutine(player.ExecuteTransition(player.transitions.GetData(transitionIntroOutTag), true));

        yield return new WaitForSeconds(3.0f);

        yield return player.StartCoroutine(player.ExecuteTransition(player.transitions.GetData(transitionOutroInTag)));
        yield return player.StartCoroutine(effect.StopWhiteoutRoutine(0.0f));
        yield return player.StartCoroutine(player.ExecuteTransition(player.transitions.GetData(transitionOutroOutTag), true));
    }
}
