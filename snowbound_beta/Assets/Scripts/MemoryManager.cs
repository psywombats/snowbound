﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MemoryManager : MonoBehaviour {

    private Dictionary<string, int> variables;

    public void Awake() {
        variables = new Dictionary<string, int>();
    }

    public Memory ToMemory() {
        Memory memory = new Memory();
        
        foreach (string key in variables.Keys) {
            memory.variableKeys.Add(key);
        }
        foreach (int value in variables.Values) {
            memory.variableValues.Add(value);
        }

        memory.screen = Global.Instance().activeScenePlayer.ToMemory();

        return memory;
    }

    public void PopulateFromMemory(Memory memory) {
        variables.Clear();
        for (int i = 0; i < memory.variableKeys.Count; i += 1) {
            variables[memory.variableKeys[i]] = memory.variableValues[i];
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
