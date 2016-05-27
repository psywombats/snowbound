using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Memory {

    // variables
    public List<string> variableKeys;
    public List<int> variableValues;

    // scene data
    public ScreenMemory screen;

    // meta info
    public double savedAt;
    public string base64ScreenshotPNG;

    public Memory() {
        variableKeys = new List<string>();
        variableValues = new List<int>();
        savedAt = Utils.CurrentTimestamp();
    }
}
