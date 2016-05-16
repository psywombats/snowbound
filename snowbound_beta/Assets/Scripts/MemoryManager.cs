using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class MemoryManager : MonoBehaviour, ISerializationCallbackReceiver {

    private Dictionary<string, int> variables;

    // serialization garbage
    [SerializeField]
    private List<string> keys;
    [SerializeField]
    private List<int> values;

    public void Awake() {
        variables = new Dictionary<string, int>();
    }

    public void OnBeforeSerialize() {
        keys = new List<string>();
        values = new List<int>();
        foreach (string key in variables.Keys) {
            keys.Add(key);
        }
        foreach (int value in variables.Values) {
            values.Add(value);
        }
    }

    public void OnAfterDeserialize() {
        variables = new Dictionary<string, int>();
        for (int i = 0; i < keys.Count; i += 1) {
            variables[keys[i]] = values[i];
        }
    }

    public int GetVariable(string variableName) {
        if (!variables.ContainsKey(variableName)) {
            variables[variableName] = 0;
        }
        return variables[variableName];
    }

    public void IncrementVariable(string variableName) {
        variables[variableName] = GetVariable(variableName) + 1;
    }

    public void DecrementVariable(string variableName) {
        variables[variableName] = GetVariable(variableName) - 1;
    }
}
