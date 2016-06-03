using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SettingsMemory {

    public List<string> floatKeys;
    public List<float> floatValues;

    public SettingsMemory() {
        floatKeys = new List<string>();
        floatValues = new List<float>();
    }
}
