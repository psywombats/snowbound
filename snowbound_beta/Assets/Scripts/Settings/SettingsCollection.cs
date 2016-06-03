using UnityEngine;
using System.Collections.Generic;

public class SettingsCollection : MonoBehaviour {

    private const string DefaultsFileName = "Settings/SettingsDefaults";

    private Dictionary<string, Setting<float>> floatSettings;

    public void Awake() {
        floatSettings = new Dictionary<string, Setting<float>>();
    }

    public SettingsMemory ToMemory() {
        SettingsMemory memory = new SettingsMemory();
        foreach (string key in floatSettings.Keys) {
            memory.floatKeys.Add(key);
        }
        foreach (Setting<float> setting in floatSettings.Values) {
            memory.floatValues.Add(setting.Value);
        }
        return memory;
    }

    public void PopulateFromMemory(SettingsMemory memory) {
        floatSettings.Clear();
        for (int i = 0; i < memory.floatKeys.Count; i += 1) {
            AddFloatSetting(memory.floatKeys[i], memory.floatValues[i]);
        }
    }

    public void LoadDefaults() {
        SettingsDefaults defaults = Resources.Load<SettingsDefaults>(DefaultsFileName);
        AddFloatSetting(SettingsConstants.TextSpeed, defaults.textSpeed);
        AddFloatSetting(SettingsConstants.BGMVolume, defaults.bgmVolume);
    }

    public float GetFloatSetting(string tag) {
        return floatSettings[tag].Value;
    }

    private void AddFloatSetting(string tag, float value) {
        floatSettings.Add(tag, new Setting<float>(tag, value));
    }
}
