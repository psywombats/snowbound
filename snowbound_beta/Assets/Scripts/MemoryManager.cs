using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class MemoryManager : MonoBehaviour {

    private const string SystemMemoryName = "system.sav";
    private const float ScreenshotScaleFactor = 6.0f;
    private const int MaxMessages = 200;

    private Dictionary<string, int> variables;
    private Dictionary<string, int> maxSeenCommands;
    private List<LogItem> messageHistory;
    private float lastSystemSavedTimestamp;

    // this thing will be read by the dialog scene when spawning
    // if non-null, it'll be loaded automatically
    public Memory ActiveMemory { get; set; }

    // global state, unrelated to playthroughs and things like that
    // things like total play time
    public SystemMemory SystemMemory { get; set; }

    public void Awake() {
        variables = new Dictionary<string, int>();
        messageHistory = new List<LogItem>();
        lastSystemSavedTimestamp = Time.realtimeSinceStartup;
        LoadOrCreateSystemMemory();
    }

    public void AppendLogItem(LogItem item) {
        messageHistory.Add(item);
        if (messageHistory.Count > MaxMessages) {
            messageHistory.RemoveAt(0);
        }
    }

    public List<LogItem> GetMessageHistory() {
        return messageHistory;
    }

    public void WriteJsonToFile(object toSerialize, string fileName) {
        FileStream file = File.Open(fileName, FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        writer.Write(JsonUtility.ToJson(toSerialize));
        writer.Flush();
        writer.Close();
        file.Close();
    }

    public T ReadJsonFromFile<T>(string fileName) {
        string json = File.ReadAllText(fileName);
        T result = JsonUtility.FromJson<T>(json);
        return result;
    }

    public Memory ToMemory() {
        Memory memory = new Memory();
        ScenePlayer player = FindObjectOfType<ScenePlayer>();
        memory.screen = player.ToMemory();

        foreach (string key in variables.Keys) {
            memory.variableKeys.Add(key);
        }
        foreach (int value in variables.Values) {
            memory.variableValues.Add(value);
        }

        // screenshot time, disable the save menu from appearing first
        SaveMenuComponent saveMenu = FindObjectOfType<SaveMenuComponent>();
        ScenePlayer scenePlayer = FindObjectOfType<ScenePlayer>();
        if (saveMenu != null) {
            saveMenu.Alpha = 0.0f;
        }
        if (scenePlayer != null) {
            scenePlayer.textbox.Alpha = 1.0f;
            scenePlayer.paragraphBox.Alpha = 1.0f;
        }
        AttachScreenshotToMemory(memory);
        if (saveMenu != null) {
            saveMenu.Alpha = 1.0f;
        }
        if (scenePlayer != null) {
            scenePlayer.textbox.Alpha = 0.0f;
            scenePlayer.paragraphBox.Alpha = 0.0f;
        }

        return memory;
    }

    public void PopulateFromMemory(Memory memory) {
        variables.Clear();
        for (int i = 0; i < memory.variableKeys.Count; i += 1) {
            variables[memory.variableKeys[i]] = memory.variableValues[i];
        }

        ScenePlayer player = FindObjectOfType<ScenePlayer>();
        player.PopulateFromMemory(memory.screen);
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

    public bool HasSeenCommand(string sceneName, int commandIndex) {
        if (maxSeenCommands.ContainsKey(sceneName)) {
            return maxSeenCommands[sceneName] >= commandIndex;
        } else {
            return false;
        }
    }

    public void AcknowledgeCommand(string sceneName, int commandIndex) {
        if (maxSeenCommands.ContainsKey(sceneName)) {
            maxSeenCommands[sceneName] = Math.Max(maxSeenCommands[sceneName], commandIndex);
        } else {
            maxSeenCommands[sceneName] = commandIndex;
        }
    }

    public void SaveSystemMemory() {

        // constants we keep track of
        float currentTimestamp = Time.realtimeSinceStartup;
        float deltaSeconds = currentTimestamp - lastSystemSavedTimestamp;
        lastSystemSavedTimestamp = currentTimestamp;
        SystemMemory.totalPlaySeconds += (int)Math.Round(deltaSeconds);

        // seen history
        foreach (string key in maxSeenCommands.Keys) {
            SystemMemory.maxSeenCommandsKeys.Add(key);
        }
        foreach (int value in maxSeenCommands.Values) {
            SystemMemory.maxSeenCommandsValues.Add(value);
        }

        // other garbage in other managers
        SystemMemory.settings = Global.Instance().settings.ToMemory();

        WriteJsonToFile(SystemMemory, GetSystemMemoryFilepath());
    }

    public Sprite SpriteFromBase64(string encodedString) {
        int width = (int)(Screen.width / ScreenshotScaleFactor);
        int height = (int)(Screen.height / ScreenshotScaleFactor);
        byte[] pngBytes = Convert.FromBase64String(encodedString);
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.LoadImage(pngBytes);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0, 0));
    }

    private void AttachScreenshotToMemory(Memory memory) {
        int width = (int)(Screen.width / ScreenshotScaleFactor);
        int height = (int)(Screen.height / ScreenshotScaleFactor);
        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);

        List<Camera> cameras = new List<Camera>(Camera.allCameras);
        cameras.Sort((Camera c1, Camera c2) => {
            return c2.transform.GetSiblingIndex().CompareTo(c1.transform.GetSiblingIndex());
        });
        foreach (Camera camera in cameras) {
            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = null;
        }

        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] pngBytes = screenshot.EncodeToPNG();
        memory.base64ScreenshotPNG = Convert.ToBase64String(pngBytes);
    }

    private void LoadOrCreateSystemMemory() {
        string path = GetSystemMemoryFilepath();
        if (File.Exists(path)) {
            SystemMemory = ReadJsonFromFile<SystemMemory>(path);
        } else {
            SystemMemory = new SystemMemory();
            Global.Instance().settings.LoadDefaults();
        }

        // reconstruct dictionaries
        maxSeenCommands = new Dictionary<string, int>();
        for (int i = 0; i < SystemMemory.maxSeenCommandsKeys.Count; i += 1) {
            maxSeenCommands[SystemMemory.maxSeenCommandsKeys[i]] = SystemMemory.maxSeenCommandsValues[i];
        }

        // ferry info to other managers
        if (SystemMemory.settings != null) {
            Global.Instance().settings.PopulateFromMemory(SystemMemory.settings);
        }
    }

    private string GetSystemMemoryFilepath() {
        return Application.persistentDataPath + "/" + SystemMemoryName;
    }
}
