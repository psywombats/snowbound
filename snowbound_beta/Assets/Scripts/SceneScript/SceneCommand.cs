using UnityEngine;
using System.Collections;
using System;

public interface SceneCommand {

    void PerformAction(SceneParser sceneManager, Action onFinish);

}
