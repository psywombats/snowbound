using UnityEngine;
using System.Collections;
using System;

public interface SceneCommand {

    void PerformAction(SceneManagerComponent sceneManager, Action onFinish);

}
