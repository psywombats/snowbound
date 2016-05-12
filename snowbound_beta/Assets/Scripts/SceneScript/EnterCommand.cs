using UnityEngine;
using System.Collections;
using System;

public class EnterCommand : SceneCommand {

    private const float hiccupTime = 0.1f;

    private string charaTag;
    private string slotLetter;

    // chara name is the name of a character, ie "max"
    // slot letter is the name of the screen slot to position, from A to E, far left to far right
    public EnterCommand(string charaTag, string slotLetter) {
        this.charaTag = charaTag;
        this.slotLetter = slotLetter;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        // let's do things in a separate coroutine so that reading is not blocking by this animation
        player.StartCoroutine(ParallelAction(player));
        yield return new WaitForSeconds(hiccupTime);
    }

    private IEnumerator ParallelAction(ScenePlayer player) {
        CharaData chara = player.GetChara(charaTag);
        TachiComponent portrait = player.portraits.GetPortraitBySlot(slotLetter);

        // fade out if someone's there already
        if (portrait.gameObject.activeSelf) {
            yield return portrait.FadeOut();
        }

        // fade in the referenced chara
        portrait.SetChara(player.charas.GetChara(charaTag));
        yield return portrait.FadeIn();
    }
}
