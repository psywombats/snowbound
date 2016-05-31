﻿using UnityEngine;
using System.Collections;
using System;

public class BackgroundCommand : SceneCommand {

    private const string FadeTransitionTag = "fade";
    private const float TextboxFadeSeconds = 0.6f;

    private string backgroundTag;
    private string transitionTag;
    private float delayElapsed;

    public BackgroundCommand(string backgroundTag, string transitionTag) {
        this.backgroundTag = backgroundTag;
        this.transitionTag = (transitionTag == null) ? FadeTransitionTag : transitionTag;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        TransitionData data = player.transitions.GetTransition(transitionTag);
        TransitionComponent transition = player.transition;
        FadeComponent fade = player.GetFade();

        yield return player.StartCoroutine(player.paragraphBox.FadeOut(TextboxFadeSeconds));

        if (data.transitionMask == null) {
            yield return player.StartCoroutine(fade.FadeToBlackRoutine());
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
            yield return player.StartCoroutine(fade.RemoveTintRoutine());
        } else {
            yield return player.StartCoroutine(transition.TransitionRoutine(data.transitionMask, true));
        }
    }
}
