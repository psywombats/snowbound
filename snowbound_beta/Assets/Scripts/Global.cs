using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour {

    private static Global instance;
    
    public InputManager input;
    public MemoryManager memory;
    public SettingsCollection settings;

    public static Global Instance() {
        if (instance == null) {
            GameObject globalObject = new GameObject();
            globalObject.hideFlags = HideFlags.HideAndDontSave;
            instance = globalObject.AddComponent<Global>();
            instance.InstantiateManagers();
        }
        return instance;
    }

    public void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void Update() {
        SetFullscreenMode();
    }

    private void InstantiateManagers() {
        settings = gameObject.AddComponent<SettingsCollection>();
        input = gameObject.AddComponent<InputManager>();
        memory = gameObject.AddComponent<MemoryManager>();
    }

    private void SetFullscreenMode() {
        // not sure if this "check" is necessary
        // actually performing this her is kind of a hack
        if (Screen.fullScreen != settings.GetBoolSetting(SettingsConstants.Fullscreen).Value) {
            Screen.fullScreen = settings.GetBoolSetting(SettingsConstants.Fullscreen).Value;
        }
    }
}
