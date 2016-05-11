using UnityEngine;
using System.Collections;
using System;

public class EnterCommand : SceneCommand {

    // chara name is the name of a character, ie "max"
    // slot letter is the name of the screen slot to position, from A to E, far left to far right
    public EnterCommand(string charaName, string slotLetter) {

    }

    public IEnumerator PerformAction(ScenePlayer player) {
        throw new NotImplementedException();
    }
}
