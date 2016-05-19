using UnityEngine;
using System.Collections;
using System;

public class ScenePlayer : MonoBehaviour, InputListener {

    private const string scenesDirectory = "SceneScripts";

    public TextAsset firstSceneFile;
    public Canvas canvas;
    public TextboxComponent textbox;
    public TextboxComponent paragraphBox;
    public PortraitGroupComponent portraits;
    public CharaIndexData charas;

    public bool Suspended { get; set; }

    private SceneScript currentScript;
    private bool wasHurried;

    public void Start() {
        textbox.gameObject.SetActive(false);
        paragraphBox.gameObject.SetActive(false);
        StartCoroutine(PlayScriptForScene(firstSceneFile));
        portraits.HideAll();
    }

    public void OnEscape() {
        Suspended = true;
        StartCoroutine(PauseRoutine());
    }

    public void OnEnter() {
        wasHurried = true;
    }

    public bool WasHurried() {
        return wasHurried;
    }

    public void AcknowledgeHurried() {
        wasHurried = false;
    }

    public IEnumerator AwaitHurry() {
        while (!WasHurried()) {
            yield return null;
        }
        AcknowledgeHurried();
    }

    public IEnumerator PlayScriptForScene(string sceneName) {
        TextAsset file = Resources.Load<TextAsset>(scenesDirectory + "/" + sceneName);
        yield return StartCoroutine(PlayScriptForScene(file));
    }

    public IEnumerator PlayScriptForScene(TextAsset sceneFile) {
        SceneScript script = new SceneScript(sceneFile);
        currentScript = script;
        Global.Instance().activeScenePlayer = this;
        Global.Instance().input.PushListener(this);
        yield return StartCoroutine(script.PerformActions(this));
    }

    public CharaData GetChara(string tag) {
        return charas.GetChara(tag);
    }

    public ScreenMemory ToMemory() {
        ScreenMemory memory = new ScreenMemory();
        currentScript.PopulateMemory(memory);
        portraits.PopulateMemory(memory);
        return memory;
    }

    private IEnumerator PauseRoutine() {
        GameObject menuObject = PauseMenuComponent.Spawn(canvas.gameObject);
        PauseMenuComponent pauseMenu = menuObject.GetComponent<PauseMenuComponent>();
        pauseMenu.Alpha = 0.0f;
        
        yield return pauseMenu.FadeIn();
    }
}
