﻿using UnityEngine;
using System.Collections;

public class SceneManagerComponent : MonoBehaviour {

    public TextAsset firstSceneFile;
    public TextboxComponent textbox;

    public void Start() {
        SceneScript firstScript = new SceneScript(firstSceneFile);
        firstScript.performActions(this, () => {

        });
    }
}
