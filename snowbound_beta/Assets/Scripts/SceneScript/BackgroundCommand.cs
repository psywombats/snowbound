using UnityEngine;
using System.Collections;
using System;

public class BackgroundCommand : SceneCommand {

    private string backgroundTag;

    public BackgroundCommand(string backgroundTag) {
        this.backgroundTag = backgroundTag;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        player.background.SetBackground(backgroundTag);
        yield return null;
    }
}
