using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ScenePlayer : MonoBehaviour, InputListener {

    private const string DialogSceneName = "DialogScene";

    public TextAsset firstSceneFile;
    public Canvas canvas;
    public TextboxComponent textbox;
    public TextboxComponent paragraphBox;
    public PortraitGroupComponent portraits;
    public CharaIndexData charas;
    public UnityEngine.UI.Text debugBox;
    
    private SceneScript currentScript;
    private IEnumerator playingRoutine;
    private bool suspended;
    private bool wasHurried;

    public bool AwaitingInputFromCommand { get; set; }
    public bool SkipMode { get; set; }

    public static void LoadScreen() {
        SceneManager.LoadScene(DialogSceneName);
    }

    public void Start() {
        textbox.gameObject.SetActive(false);
        paragraphBox.gameObject.SetActive(false);
        
        Global.Instance().input.PushListener(this);

        portraits.HideAll();

        if (Global.Instance().memory.ActiveMemory != null) {
            Global.Instance().memory.PopulateFromMemory(Global.Instance().memory.ActiveMemory);
            Global.Instance().memory.ActiveMemory = null;
            ResumeLoadedScene();
        } else {
            PlayFirstScene();
        }
    }

    public void OnCommand(InputManager.Command command) {
        switch (command) {
            case InputManager.Command.Advance:
                wasHurried = true;
                break;
            case InputManager.Command.Menu:
                suspended = true;
                StartCoroutine(PauseRoutine());
                break;
            case InputManager.Command.Skip:
                SkipMode = !SkipMode;
                break;
        }
    }

    public bool WasHurried() {
        return wasHurried;
    }

    public bool IsSuspended() {
        return suspended;
    }

    public bool ShouldUseFastMode() {
        return currentScript.ShouldUseFastMode(this);
    }

    public void AcknowledgeHurried() {
        wasHurried = false;
    }

    public void PlayFirstScene() {
        StartCoroutine(PlayScriptForScene(firstSceneFile));
    }

    public IEnumerator AwaitHurry() {
        while (!WasHurried() && !ShouldUseFastMode()) {
            yield return null;
        }
        AcknowledgeHurried();
    }

    public IEnumerator PlayScriptForScene(string sceneName) {
        TextAsset file = SceneScript.AssetForSceneName(sceneName);
        yield return StartCoroutine(PlayScriptForScene(file));
    }

    public IEnumerator PlayScriptForScene(TextAsset sceneFile) {
        currentScript = new SceneScript(this, sceneFile);
        yield return StartCoroutine(PlayCurrentScript());
    }

    public void ResumeLoadedScene() {
        StartCoroutine(PlayCurrentScript());
    }

    public CharaData GetChara(string tag) {
        return charas.GetChara(tag);
    }

    public ScreenMemory ToMemory() {
        ScreenMemory memory = new ScreenMemory();
        currentScript.PopulateMemory(memory);
        if (AwaitingInputFromCommand) {
            memory.commandNumber -= 1;
        }
        portraits.PopulateMemory(memory);
        return memory;
    }

    public void PopulateFromMemory(ScreenMemory memory) {
        if (playingRoutine != null) {
            StopCoroutine(playingRoutine);
        }
        currentScript = new SceneScript(memory);
        portraits.PopulateFromMemory(memory);
    }

    public IEnumerator ResumeRoutine() {
        yield return Utils.RunParallel(new[] {
            textbox.FadeIn(PauseMenuComponent.FadeoutSeconds),
            paragraphBox.FadeIn(PauseMenuComponent.FadeoutSeconds)
        }, this);
        suspended = false;
    }

    private IEnumerator PauseRoutine() {
        yield return Utils.RunParallel(new[] {
            textbox.FadeOut(PauseMenuComponent.FadeoutSeconds),
            paragraphBox.FadeOut(PauseMenuComponent.FadeoutSeconds)
        }, this);

        GameObject menuObject = PauseMenuComponent.Spawn(canvas.gameObject, this);
        PauseMenuComponent pauseMenu = menuObject.GetComponent<PauseMenuComponent>();
        pauseMenu.Alpha = 0.0f;
        
        yield return pauseMenu.FadeIn();
    }

    private IEnumerator PlayCurrentScript() {
        playingRoutine = currentScript.PerformActions(this);
        yield return StartCoroutine(playingRoutine);
    }
}
