using UnityEngine;
using System.Collections.Generic;

public class SettingsCollection {

    private const string DefaultsFileName = "Settings/SettingsDefaults";

    private Dictionary<string, Setting<float>> floatSettings;

    public SettingsCollection() {
        floatSettings = new Dictionary<string, Setting<float>>();
    }

    public void LoadDefaults() {
        SettingsDefaults defaults = Resources.Load<SettingsDefaults>(DefaultsFileName);
        AddFloatConstant(SettingsConstants.TextSpeed, defaults.textSpeed);
        AddFloatConstant(SettingsConstants.BGMVolume, defaults.bgmVolume);
    }

    public float GetFloatSetting(string tag) {
        return floatSettings[tag].Value;
    }

    private void AddFloatConstant(string tag, float value) {
        floatSettings.Add(tag, new Setting<float>(tag, value));
    }
}
