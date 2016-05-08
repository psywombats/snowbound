using UnityEngine;
using System.Collections;
using System;

public interface SceneCommand {

    IEnumerator PerformAction(ScenePlayer sceneManager);

}
