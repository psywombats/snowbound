using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemMemory {

    public int totalPlaySeconds;
    public List<SceneReadMemory> readScenes;

    public SystemMemory() {
        readScenes = new List<SceneReadMemory>();
    }
}
