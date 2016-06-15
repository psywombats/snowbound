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
    public BackgroundComponent background;
    public PortraitGroupComponent portraits;
    public TransitionIndexData transitions;
    public TransitionComponent transition;
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

    public bool OnCommand(InputManager.Command command) {
        switch (command) {
            case InputManager.Command.Advance:
                if (hiddenTextMode) {
                    SetHiddenTextMode(false);
                } else {
                    wasHurried = true;
                }
                return true;
            case InputManager.Command.Menu:
                suspended = true;
                StartCoroutine(PauseRoutine());
                return true;
            case InputManager.Command.Skip:
                SkipMode = !SkipMode;
                return true;
            case InputManager.Command.Click:
                if (hiddenTextMode) {
                    SetHiddenTextMode(false);
                } else {
                    Global.Instance().input.SimulateAdvance();
                }
                return true;
            case InputManager.Command.Rightclick:
                SetHiddenTextMode(!hiddenTextMode);
                return true;
            default:
                return false;
        }
    }

    public void SetInputEnabled(bool enabled) {
        if (enabled) {
            currentScript.CurrentCommand.OnFocusGained();
        } else {
            currentScript.CurrentCommand.OnFocusGained();
        }
    }

    public bool WasHurried() {
        return wasHurried;
    }

    public bool IsSuspended() {
        return suspended;
    }

    public bool ShouldUseFastMode() {
        if (currentScript == null) {
            return false;
        } else {
            return currentScript.ShouldUseFastMode(this);
        }
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

    public BGMPlayer GetBGM() {
        return FindObjectOfType<BGMPlayer>();
    }

    public SoundPlayer GetSound() {
        return FindObjectOfType<SoundPlayer>();
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

    public ScreenMemory ToMemory() {
        ScreenMemory memory = new ScreenMemory();
        currentScript.PopulateMemory(memory);
        if (AwaitingInputFromCommand) {
            memory.commandNumber -= 1;
        }
        portraits.PopulateMemory(memory);
        background.PopuateMemory(memory);
        GetBGM().PopulateMemory(memory);
        return memory;
    }

    public void PopulateFromMemory(ScreenMemory memory) {
        if (playingRoutine != null) {
            StopCoroutine(playingRoutine);
        }
        currentScript = new SceneScript(memory);
        portraits.PopulateFromMemory(memory);
        background.PopulateFromMemory(memory);
        GetBGM().PopulateFromMemory(memory);
    }

    public IEnumerator ResumeRoutine() {
        yield return Utils.RunParallel(new[] {
            textbox.FadeInRoutine(PauseMenuComponent.FadeoutSeconds),
            paragraphBox.FadeInRoutine(PauseMenuComponent.FadeoutSeconds)
        }, this);
        suspended = false;
    }

    private void SetHiddenTextMode(bool hidden) {
        StartCoroutine(SetHiddenTextModeRoutine(hidden));
    }

    private IEnumerator PauseRoutine() {
        Global.Instance().memory.RememberScreenshot();

        yield return Utils.RunParallel(new[] {
            textbox.FadeOutRoutine(PauseMenuComponent.FadeoutSeconds),
            paragraphBox.FadeOutRoutine(PauseMenuComponent.FadeoutSeconds)
        }, this);

        GameObject menuObject = PauseMenuComponent.Spawn(canvas.gameObject, () => {
            StartCoroutine(ResumeRoutine());
        });
        PauseMenuComponent pauseMenu = menuObject.GetComponent<PauseMenuComponent>();
        pauseMenu.Alpha = 0.0f;
        
        yield return pauseMenu.FadeInRoutine();
    }

    private IEnumerator PlayCurrentScript() {
        playingRoutine = currentScript.PerformActions(this);
        yield return StartCoroutine(playingRoutine);
    }

    private IEnumerator SetHiddenTextModeRoutine(bool hidden) {
        Global.Instance().input.DisableListener(this);

        if (hidden) {
            yield return paragraphBox.FadeOutRoutine(hiddenTextModeFadeoutSeconds);
            yield return textbox.FadeOutRoutine(hiddenTextModeFadeoutSeconds);
        } else {
            yield return paragraphBox.FadeInRoutine(hiddenTextModeFadeoutSeconds);
            yield return textbox.FadeInRoutine(hiddenTextModeFadeoutSeconds);
        }

        hiddenTextMode = hidden;
        Global.Instance().input.EnableListener(this);
    }
}
