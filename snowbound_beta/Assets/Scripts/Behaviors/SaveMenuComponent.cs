using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SaveMenuComponent : MonoBehaviour, InputListener {

    private const float FadeoutSeconds = 0.2f;
    private const string PrefabName = "Prefabs/SaveMenu";
    private const string SaveGameSuffix = ".sav";
    private const string SystemSaveName = "system.sav";

    public enum SaveMenuMode {
        Save, Load
    };

    public SaveButtonComponent[] slots;
    
    private SaveMenuMode mode;
    private Action onFinish;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }
    
    public static GameObject Spawn(GameObject parent, SaveMenuMode mode, Action onFinish) {
        GameObject menuObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(PrefabName));
        menuObject.GetComponent<SaveMenuComponent>().onFinish = onFinish;
        menuObject.GetComponent<SaveMenuComponent>().mode = mode;
        Utils.AttachAndCenter(parent, menuObject);
        return menuObject;
    }

    public void Start() {
        Global.Instance().input.PushListener(this);
        RefreshData();
    }

    public void OnEnter() {
        // nothing
    }

    public void OnEscape() {
        StartCoroutine(ResumeRoutine());
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

    public IEnumerator FadeIn() {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        Alpha = 1.0f;
    }

    public IEnumerator FadeOut() {
        CanvasGroup group = gameObject.GetComponent<CanvasGroup>();
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        group.alpha = 0.0f;
    }

    public IEnumerator ResumeRoutine() {
        SetButtonsEnabled(false);
        yield return StartCoroutine(FadeOut());
        Global.Instance().input.RemoveListener(this);
        if (onFinish != null) {
            onFinish();
        }
        Destroy(gameObject);
    }

    private void SetButtonsEnabled(bool enabled) {
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
        SetButtonsEnabled(false);
        yield return StartCoroutine(FadeOut());
        Global.Instance().input.RemoveListener(this);
        Global.Instance().memory.ActiveMemory = memory;
        FadeComponent fade = FindObjectOfType<FadeComponent>();
        yield return fade.FadeToBlackRoutine();
        ScenePlayer.LoadScreen();
        yield return null;
    }
}
