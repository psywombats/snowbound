using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class MemoryManager : MonoBehaviour {

    private Dictionary<string, int> variables;

    // this thing will be read by the dialog scene when spawning
    // if non-null, it'll be loaded automatically
    public Memory ActiveMemory { get; set; }

    public void Awake() {
        variables = new Dictionary<string, int>();
    }

    public Memory ToMemory() {
        Memory memory = new Memory();
        memory.screen = Global.Instance().activeScenePlayer.ToMemory();

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

        Global.Instance().activeScenePlayer.PopulateFromMemory(memory.screen);
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

    public Sprite SpriteFromBase64(string encodedString) {
        byte[] pngBytes = Convert.FromBase64String(encodedString);
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.LoadImage(pngBytes);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, Screen.width, Screen.height), new Vector2(0, 0));
    }

    private void AttachScreenshotToMemory(Memory memory) {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

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
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;
        Destroy(renderTexture);

        byte[] pngBytes = screenshot.EncodeToPNG();

        FileStream file = File.Open("test.png", FileMode.Create);
        file.Write(pngBytes, 0, pngBytes.Length);
        file.Close();

        memory.base64ScreenshotPNG = Convert.ToBase64String(pngBytes);
    }
}
