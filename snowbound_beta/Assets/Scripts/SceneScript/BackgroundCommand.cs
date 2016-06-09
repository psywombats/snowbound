using UnityEngine;
using System.Collections;
using System;

public class BackgroundCommand : SceneCommand {

    private const string FadeTransitionTag = "fade";
    private const float TextboxFadeSeconds = 0.6f;

    private string backgroundTag;
    private string transitionTag;

    public BackgroundCommand(string backgroundTag, string transitionTag) {
        this.backgroundTag = backgroundTag;
        this.transitionTag = (transitionTag == null) ? FadeTransitionTag : transitionTag;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        TransitionData data = player.transitions.GetData(transitionTag);
        TransitionComponent transition = player.transition;
        FadeComponent fade = player.GetFade();

        yield return player.StartCoroutine(player.paragraphBox.FadeOutRoutine(TextboxFadeSeconds));
        
        if (data.transitionMask == null) {
            yield return player.StartCoroutine(fade.FadeToBlackRoutine(true, false));
        } else {
            yield return player.StartCoroutine(transition.TransitionRoutine(data.transitionMask, false));
        }
        
        player.background.SetBackground(backgroundTag);

        while (transition.IsTransitioning()) {
            if (player.ShouldUseFastMode()) {
                transition.Hurry();
            }
            yield return null;
        }

        if (data.transitionMask == null) {
            yield return player.StartCoroutine(fade.RemoveTintRoutine(true));
        } else {
            yield return player.StartCoroutine(transition.TransitionRoutine(data.transitionMask, true));
        }
    }
}
