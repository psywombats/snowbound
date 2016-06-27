using UnityEngine;
using System.Collections;
using System;

public class SwitchToCommand : ParagraphCommand {

    public SwitchToCommand(string targetCharaKey) : base("~ NOW PLAYING FROM " + targetCharaKey + "'s PERSPECTIVE ~") {

    }
}
