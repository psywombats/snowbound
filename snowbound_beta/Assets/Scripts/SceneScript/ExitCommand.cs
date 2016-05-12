using UnityEngine;
using System.Collections;
using System;

public class ExitCommand : SceneCommand {

    private const float hiccupTime = 0.1f;

    private string charaTag;

    // chara name is the name of a character, ie "max"
    public ExitCommand(string charaTag) {
        this.charaTag = charaTag;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        // let's do things in a separate coroutine so that reading is not blocking by this animation
        player.StartCoroutine(ParallelAction(player));
        yield return new WaitForSeconds(hiccupTime);
    }

    private IEnumerator ParallelAction(ScenePlayer player) {
        CharaData chara = player.GetChara(charaTag);
        TachiComponent portrait = player.portraits.GetPortraitByChara(chara);

        // fade 'em out!
        yield return portrait.FadeOut();
    }
}
