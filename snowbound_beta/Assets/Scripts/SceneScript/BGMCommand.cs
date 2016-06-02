using UnityEngine;
using System.Collections;
using System;

public class BGMCommand : SceneCommand {

    private string tag;

	public BGMCommand(string tag) {
        this.tag = tag;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        BGMPlayer bgm = player.GetBGM();
        bgm.StartCoroutine(bgm.CrossfadeRoutine(tag));
        yield return null;
    }
}
