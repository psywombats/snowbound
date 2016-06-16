using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SaveMenuComponent : MenuComponent {

    private const float FadeoutSeconds = 0.2f;
    private const float LoadDelaySeconds = 1.5f;
    private const string PrefabName = "Prefabs/SaveMenu";
    private const string SaveGameSuffix = ".sav";
    private const string SystemSaveName = "system.sav";

    public enum SaveMenuMode {
        Save, Load
    };

    public SaveButtonComponent[] slots;
    
    private SaveMenuMode mode;
    
    public static GameObject Spawn(GameObject parent, SaveMenuMode mode, Action onFinish) {
        GameObject menuObject = Spawn(parent, PrefabName, onFinish);
        menuObject.GetComponent<SaveMenuComponent>().mode = mode;
        return menuObject;
    }

    public override void Start() {
        base.Start();
        RefreshData();
    }

    public void SaveOrLoadFromSlot(int slot) {
        if (mode == SaveMenuMode.Load) {
            Memory memory = Global.Instance().memory.ReadJsonFromFile<Memory>(FilePathForSlot(slot));
            StartCoroutine(LoadRoutine(memory));
        } else {
            Memory memory = Global.Instance().memory.ToMemory();
            Global.Instance().memory.WriteJsonToFile(memory, FilePathForSlot(slot));
            Global.Instance().memory.SaveSystemMemory();
            RefreshData();
            StartCoroutine(ResumeRoutine());
        }
    }

    protected override void SetInputEnabled(bool enabled) {
        base.SetInputEnabled(enabled);
        foreach (SaveButtonComponent button in slots) {
            button.button.interactable = enabled;
        }
    }

    private void RefreshData() {
        for (int i = 0; i < slots.Length; i += 1) {
            SaveButtonComponent saveButton = slots[i];
            string fileName = FilePathForSlot(i);
            Memory memory = null;
            if (File.Exists(fileName)) {
                memory = Global.Instance().memory.ReadJsonFromFile<Memory>(fileName);
            }
            saveButton.Populate(this, i, memory, mode);
        }
    }

    private string FilePathForSlot(int slot) {
        string fileName = Application.persistentDataPath + "/";
        if (slot < 10) {
            fileName += "betasave0";
        }
        fileName += Convert.ToString(slot);
        fileName += SaveGameSuffix;
        return fileName;
    }

    private IEnumerator LoadRoutine(Memory memory) {
        SetInputEnabled(false);
        yield return StartCoroutine(FadeOutRoutine());
        Global.Instance().input.RemoveListener(this);
        Global.Instance().memory.ActiveMemory = memory;
        FadeComponent fade = FindObjectOfType<FadeComponent>();
        yield return fade.FadeToBlackRoutine();
        yield return new WaitForSeconds(LoadDelaySeconds);
        ScenePlayer.LoadScreen();
        yield return null;
    }
}
