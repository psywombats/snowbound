using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class PauseMenuComponent : MonoBehaviour, InputListener {

    private const float fadeoutSeconds = 0.2f;
    private const string prefabName = "Prefabs/PauseMenu";

    public Button saveButton;
    public Button loadButton;
    public Button resumeButton;
    public Button closeButton;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public static GameObject Spawn(GameObject parent) {
        GameObject menuObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(prefabName));
        Utils.AttachAndCenter(parent, menuObject);
        return menuObject;
    }

    public void Awake() {
        saveButton.onClick.AddListener(() => {
            StartCoroutine(SaveRoutine());
        });

        loadButton.onClick.AddListener(() => {
            
        });

        resumeButton.onClick.AddListener(() => {
            StartCoroutine(ResumeRoutine());
        });

        closeButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }

    public void Start() {
        Global.Instance().input.PushListener(this);
    }

    public void OnEnter() {
        // nothing
    }

    public void OnEscape() {
        StartCoroutine(ResumeRoutine());
    }

    public IEnumerator FadeIn() {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / fadeoutSeconds;
            yield return null;
        }
        Alpha = 1.0f;
    }

    public IEnumerator FadeOut() {
        CanvasGroup group = gameObject.GetComponent<CanvasGroup>();
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / fadeoutSeconds;
            yield return null;
        }
        group.alpha = 0.0f;
    }

    private IEnumerator ResumeRoutine() {
        yield return StartCoroutine(FadeOut());
        Global.Instance().activeScenePlayer.Suspended = false;
        Global.Instance().input.RemoveListener(this);
        Destroy(this);
    }

    private IEnumerator SaveRoutine() {
        yield return StartCoroutine(FadeOut());
        GameObject saveMenuObject = SaveMenuComponent.Spawn(this);
        saveMenuObject.GetComponent<SaveMenuComponent>().Alpha = 0.0f;
        yield return StartCoroutine(saveMenuObject.GetComponent<SaveMenuComponent>().FadeIn());
    }
}
