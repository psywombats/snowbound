using UnityEngine;
using System.Collections;

public class ScenePlayer : MonoBehaviour {

    private const string scenesDirectory = "SceneScripts";

    public TextAsset firstSceneFile;
    public Canvas canvas;
    public TextboxComponent textbox;
    public TextboxComponent paragraphBox;
    public PortraitGroupComponent portraits;
    public CharaIndexData charas;

    public bool Suspended { get; set; }

    private SceneScript currentScript;

    public void Start() {
        textbox.gameObject.SetActive(false);
        paragraphBox.gameObject.SetActive(false);
        StartCoroutine(PlayScriptForScene(firstSceneFile));
        portraits.HideAll();
    }

    public IEnumerator PlayScriptForScene(string sceneName) {
        TextAsset file = Resources.Load<TextAsset>(scenesDirectory + "/" + sceneName);
        yield return StartCoroutine(PlayScriptForScene(file));
    }

    public IEnumerator PlayScriptForScene(TextAsset sceneFile) {
        SceneScript script = new SceneScript(sceneFile);
        currentScript = script;
        Global.Instance().activeScenePlayer = this;
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

    public void Pause() {
        Suspended = true;
        StartCoroutine(PauseRoutine());
    }

    private IEnumerator PauseRoutine() {
        GameObject menuObject = PauseMenuComponent.Spawn();
        PauseMenuComponent pauseMenu = menuObject.GetComponent<PauseMenuComponent>();
        pauseMenu.Alpha = 0.0f;

        RectTransform transform = menuObject.GetComponent<RectTransform>();
        transform.SetParent(canvas.transform);
        transform.anchorMin = new Vector2(0.5f, 0.5f);
        transform.anchorMax = transform.anchorMin;
        transform.anchoredPosition = new Vector2(0, 0);
        yield return pauseMenu.FadeIn();
    }
}
