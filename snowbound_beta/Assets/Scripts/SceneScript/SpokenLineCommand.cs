using UnityEngine;
using System.Collections;
using System;

public class SpokenLineCommand : TextCommand {

    private CharaData chara;

    // we expect text in the format MAX: "Some stuff!"
    // or else for narration lines, just Some stuff happened. is fine
    // breaking it down into character tag and stuff is done internally
    public SpokenLineCommand(ScenePlayer player, string text) : base(text) {
        if (SceneScript.StartsWithName(text)) {
            string tag = text.Substring(0, text.IndexOf(':'));
            this.text = text.Substring(text.IndexOf(':') + 2);
            chara = player.portraits.charas.GetData(tag);
        }
    }

    protected override TextboxComponent PrimaryBox(ScenePlayer parser) {
        return parser.textbox;
    }

    protected override TextboxComponent SecondaryBox(ScenePlayer parser) {
        return parser.paragraphBox;
    }
}
