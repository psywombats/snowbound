using UnityEngine;
using System.Collections;

public abstract class StageDirectionCommand : SceneCommand {

    protected bool synchronous;

    public abstract IEnumerator PerformAction(ScenePlayer player);

    public void SetSynchronous() {
        synchronous = true;
    }

}
