using UnityEngine;
using System.Collections;
using System;

public class EnterCommand : StageDirectionCommand {

    private const float hiccupTime = 0.1f;

    private string charaTag;
    private string slotLetter;

    // chara name is the name of a character, ie "max"
    // slot letter is the name of the screen slot to position, from A to E, far left to far right
    public EnterCommand(string charaTag, string slotLetter) {
        this.charaTag = charaTag;
        this.slotLetter = slotLetter;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        if (synchronous) {
            yield return player.StartCoroutine(ParallelAction(player));
        } else {
            yield return null;
            player.StartCoroutine(ParallelAction(player));
        }
    }

    private IEnumerator ParallelAction(ScenePlayer player) {
        CharaData chara = player.portraits.charas.GetData(charaTag);
        TachiComponent portrait = player.portraits.GetPortraitBySlot(slotLetter);

        // fade out if someone's there already
        if (portrait.gameObject.activeSelf) {
            yield return portrait.FadeOut();
        }

        // fade in the referenced chara
        portrait.SetChara(chara);
        yield return portrait.FadeIn();
    }
}
