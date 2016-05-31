using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class ScenePlayer : MonoBehaviour, InputListener {

    private const string DialogSceneName = "DialogScene";
    private const float hiddenTextModeFadeoutSeconds = 0.6f;

    public TextAsset firstSceneFile;
    public Canvas canvas;
    public TextboxComponent textbox;
    public TextboxComponent paragraphBox;
    public PortraitGroupComponent portraits;
    public BackgroundComponent background;
    public CharaIndexData charas;
    public BackgroundIndexData backgrounds;
    public TransitionIndexData transitions;
    public TransitionImageEffect transition;
    public UnityEngine.UI.Text debugBox;
    
    private SceneScript currentScript;
    private IEnumerator playingRoutine;
    private bool suspended;
    private bool wasHurried;
    private bool hiddenTextMode;

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
                if (hiddenTextMode) {
                    SetHiddenTextMode(false);
                } else {
                    wasHurried = true;
                }
                break;
            case InputManager.Command.Menu:
                suspended = true;
                StartCoroutine(PauseRoutine());
                break;
            case InputManager.Command.Skip:
                SkipMode = !SkipMode;
                break;
            case InputManager.Command.Click:
                if (hiddenTextMode) {
                    SetHiddenTextMode(false);
                } else {
                    Global.Instance().input.SimulateAdvance();
                }
                break;
            case InputManager.Command.Rightclick:
                SetHiddenTextMode(!hiddenTextMode);
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

    public FadeComponent GetFade() {
        return FindObjectOfType<FadeComponent>();
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

    public BackgroundData GetBackground(string tag) {
        return backgrounds.GetBackground(tag);
    }

    public ScreenMemory ToMemory() {
        ScreenMemory memory = new ScreenMemory();
        currentScript.PopulateMemory(memory);
        if (AwaitingInputFromCommand) {
            memory.commandNumber -= 1;
        }
        portraits.PopulateMemory(memory);
        background.PopuateMemory(memory);
        return memory;
    }

    public void PopulateFromMemory(ScreenMemory memory) {
        if (playingRoutine != null) {
            StopCoroutine(playingRoutine);
        }
        currentScript = new SceneScript(memory);
        portraits.PopulateFromMemory(memory);
        background.PopulateFromMemory(memory);
    }

    public IEnumerator ResumeRoutine() {
        yield return Utils.RunParallel(new[] {
            textbox.FadeIn(PauseMenuComponent.FadeoutSeconds),
            paragraphBox.FadeIn(PauseMenuComponent.FadeoutSeconds)
        }, this);
        suspended = false;
    }

    private void SetHiddenTextMode(bool hidden) {
        StartCoroutine(SetHiddenTextModeRoutine(hidden));
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

    private IEnumerator SetHiddenTextModeRoutine(bool hidden) {
        Global.Instance().input.DisableListener(this);

        if (hidden) {
            yield return paragraphBox.FadeOut(hiddenTextModeFadeoutSeconds);
            yield return textbox.FadeOut(hiddenTextModeFadeoutSeconds);
        } else {
            yield return paragraphBox.FadeIn(hiddenTextModeFadeoutSeconds);
            yield return textbox.FadeIn(hiddenTextModeFadeoutSeconds);
        }

        hiddenTextMode = hidden;
        Global.Instance().input.EnableListener(this);
    }
}
