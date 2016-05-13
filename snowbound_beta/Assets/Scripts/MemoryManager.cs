using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour {

    private Dictionary<string, int> variables;

    public void Awake() {
        variables = new Dictionary<string, int>();
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
