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

    private PauseMenuComponent pauseMenu;
    private SaveMenuMode mode;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public static GameObject Spawn(PauseMenuComponent pauseMenu, SaveMenuMode mode) {
        GameObject menuObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(PrefabName));
        menuObject.GetComponent<SaveMenuComponent>().pauseMenu = pauseMenu;
        menuObject.GetComponent<SaveMenuComponent>().mode = mode;
        GameObject parent = pauseMenu.gameObject.transform.parent.gameObject;
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
            Memory memory = ReadJsonFromFile<Memory>(FilePathForSlot(slot));
            Global.Instance().memory.PopulateFromMemory(memory);
            StartCoroutine(LoadRoutine());
        } else {
            Memory memory = Global.Instance().memory.ToMemory();
            WriteJsonToFile(memory, FilePathForSlot(slot));
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
        yield return StartCoroutine(FadeOut());
        yield return pauseMenu.FadeIn();
        Global.Instance().input.RemoveListener(this);
        Destroy(gameObject);
    }

    private void RefreshData() {
        for (int i = 0; i < slots.Length; i += 1) {
            SaveButtonComponent saveButton = slots[i];
            string fileName = FilePathForSlot(i);
            Memory memory = null;
            if (File.Exists(fileName)) {
                memory = ReadJsonFromFile<Memory>(fileName);
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

    private void WriteJsonToFile(object toSerialize, string fileName) {
        FileStream file = File.Open(fileName, FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(JsonUtility.ToJson(toSerialize));
        writer.Flush();
        writer.Close();
        file.Close();
    }

    private T ReadJsonFromFile<T>(string fileName) {
        string json = File.ReadAllText(fileName);
        T result = JsonUtility.FromJson<T>(json);
        return result;
    }

    private IEnumerator LoadRoutine() {
        yield return StartCoroutine(FadeOut());
        Global.Instance().input.RemoveListener(this);
        Global.Instance().input.RemoveListener(pauseMenu);
        Global.Instance().activeScenePlayer.ResumeLoadedScene();
        Destroy(gameObject);
    }
}
