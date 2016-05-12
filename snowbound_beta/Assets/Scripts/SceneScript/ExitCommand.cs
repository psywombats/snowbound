using UnityEngine;
using System.Collections;
using System;

public class ExitCommand : StageDirectionCommand {

    private string charaTag;

    // chara name is the name of a character, ie "max"
    public ExitCommand(string charaTag) {
        this.charaTag = charaTag;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        if (synchronous) {
            yield return player.StartCoroutine(ParallelAction(player));
        } else {
            player.StartCoroutine(ParallelAction(player));
            yield return null;
        }
    }

    private IEnumerator ParallelAction(ScenePlayer player) {
        CharaData chara = player.GetChara(charaTag);
        TachiComponent portrait = player.portraits.GetPortraitByChara(chara);

        // fade 'em out!
        yield return portrait.FadeOut();
    }
}
