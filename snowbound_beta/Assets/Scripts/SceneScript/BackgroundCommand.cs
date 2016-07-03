using UnityEngine;
using System.Collections;
using System;

public class BackgroundCommand : SceneCommand {

    private const string FadeTransitionTag = "fade";

    private string backgroundTag;
    private string transitionTag;

    public BackgroundCommand(string backgroundTag, string transitionTag) {
        this.backgroundTag = backgroundTag;
        this.transitionTag = (transitionTag == null) ? FadeTransitionTag : transitionTag;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        TransitionData data = player.transitions.GetData(transitionTag);
        FadeComponent fade = player.GetFade();

        yield return player.StartCoroutine(player.paragraphBox.Deactivate(player));
        yield return player.StartCoroutine(player.textbox.Deactivate(player));

        yield return player.StartCoroutine(player.ExecuteTransition(data));
        
        player.background.SetBackground(backgroundTag);

        yield return player.StartCoroutine(player.ExecuteTransition(data, true));
    }
}
