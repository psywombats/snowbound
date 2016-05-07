using UnityEngine;
using System.Collections;
using System;

public interface SceneCommand {

    void performAction(SceneManagerComponent sceneManager, Action onFinish);

}
