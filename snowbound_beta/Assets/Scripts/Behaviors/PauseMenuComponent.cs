using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class PauseMenuComponent : MonoBehaviour, InputListener {

    public const float FadeoutSeconds = 0.2f;
    private const string PrefabName = "Prefabs/PauseMenu";
    private const string TitleSceneName = "TitleScene";

    public Button saveButton;
    public Button loadButton;
    public Button resumeButton;
    public Button closeButton;
    public Button titleButton;

    private ScenePlayer player;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public static GameObject Spawn(GameObject parent, ScenePlayer player) {
        GameObject menuObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(PrefabName));
        menuObject.GetComponent<PauseMenuComponent>().player = player;
        Utils.AttachAndCenter(parent, menuObject);
        return menuObject;
    }

    public void Awake() {
        saveButton.onClick.AddListener(() => {
            StartCoroutine(SaveRoutine());
        });
        loadButton.onClick.AddListener(() => {
            StartCoroutine(LoadRoutine());
        });
        resumeButton.onClick.AddListener(() => {
            StartCoroutine(ResumeRoutine());
        });
        closeButton.onClick.AddListener(() => {
            StartCoroutine(QuitRoutine());
        });
        titleButton.onClick.AddListener(() => {
            StartCoroutine(TitleRoutine());
        });
    }

    public void Start() {
        Global.Instance().input.PushListener(this);
    }

    public void OnCommand(InputManager.Command command) {
        switch (command) {
            case InputManager.Command.Menu:
                StartCoroutine(ResumeRoutine());
                break;
            default:
                break;
        }
    }

    public IEnumerator FadeIn() {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        Alpha = 1.0f;
        Global.Instance().input.EnableListener(this);
        SetButtonsEnabled(true);
    }

    public IEnumerator FadeOut() {
        Global.Instance().input.DisableListener(this);
        SetButtonsEnabled(false);
        CanvasGroup group = gameObject.GetComponent<CanvasGroup>();
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        group.alpha = 0.0f;
    }

    private void SetButtonsEnabled(bool enabled) {
        saveButton.interactable = enabled;
        loadButton.interactable = enabled;
        resumeButton.interactable = enabled;
        closeButton.interactable = enabled;
        titleButton.interactable = enabled;
    }

    private IEnumerator ResumeRoutine() {
        yield return StartCoroutine(FadeOut());
        yield return player.ResumeRoutine();
        Global.Instance().input.RemoveListener(this);
        Destroy(gameObject);
    }

    private IEnumerator SaveRoutine() {
        yield return StartCoroutine(FadeOut());
        GameObject saveMenuObject = SaveMenuComponent.Spawn(gameObject.transform.parent.gameObject, SaveMenuComponent.SaveMenuMode.Save, () => {
            StartCoroutine(FadeIn());
        });
        saveMenuObject.GetComponent<SaveMenuComponent>().Alpha = 0.0f;
        yield return StartCoroutine(saveMenuObject.GetComponent<SaveMenuComponent>().FadeIn());
    }

    private IEnumerator LoadRoutine() {
        yield return StartCoroutine(FadeOut());
        GameObject saveMenuObject = SaveMenuComponent.Spawn(gameObject.transform.parent.gameObject, SaveMenuComponent.SaveMenuMode.Load, () => {
            StartCoroutine(FadeIn());
        });
        saveMenuObject.GetComponent<SaveMenuComponent>().Alpha = 0.0f;
        yield return StartCoroutine(saveMenuObject.GetComponent<SaveMenuComponent>().FadeIn());
    }

    private IEnumerator TitleRoutine() {
        Global.Instance().input.RemoveListener(this);
        FadeComponent fader = FindObjectOfType<FadeComponent>();
        yield return fader.FadeToBlackRoutine();
        SceneManager.LoadScene(TitleSceneName);
    }

    private IEnumerator QuitRoutine() {
        FadeComponent fader = FindObjectOfType<FadeComponent>();
        yield return fader.FadeToBlackRoutine();
        Application.Quit();
    }
}
