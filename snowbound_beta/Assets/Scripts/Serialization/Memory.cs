using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Memory : MonoBehaviour {

    // variables
    public List<string> variableKeys;
    public List<int> variableValues;

    // scene data
    public ScreenMemory screen;

    public Memory() {
        variableKeys = new List<string>();
        variableValues = new List<int>();
    }
}
