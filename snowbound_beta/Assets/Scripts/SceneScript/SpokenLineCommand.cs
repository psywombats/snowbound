﻿using UnityEngine;
using System.Collections;
using System;

public class SpokenLineCommand : TextCommand {

    public SpokenLineCommand(string text) : base(text) {

    }

    protected override TextboxComponent PrimaryBox(ScenePlayer parser) {
        return parser.textbox;
    }

    protected override TextboxComponent SecondaryBox(ScenePlayer parser) {
        return parser.paragraphBox;
    }
}