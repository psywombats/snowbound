using UnityEngine;
using System.Collections;
using System;

public class ParagraphCommand : TextCommand {

    public ParagraphCommand(string text) : base(text) {

    }

    protected override TextboxComponent PrimaryBox(ScenePlayer parser) {
        return parser.paragraphBox;
    }

    protected override TextboxComponent SecondaryBox(ScenePlayer parser) {
        return parser.textbox;
    }
}
