using UnityEngine;
using System.Collections;
using System;

public class SpokenLineCommand : TextCommand {

    public SpokenLineCommand(string text) : base(text) {

    }

    protected override TextboxComponent PrimaryBox(SceneParser parser) {
        return parser.textbox;
    }

    protected override TextboxComponent SecondaryBox(SceneParser parser) {
        return parser.paragraphBox;
    }
}
