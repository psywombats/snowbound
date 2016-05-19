using UnityEngine;
using System.Collections;
using System;

public class SaveMenuComponent : MonoBehaviour, InputListener {

    private const float fadeoutSeconds = 0.2f;
    private const string prefabName = "Prefabs/SaveMenu";

    public SaveButtonComponent[] slots;

    private PauseMenuComponent pauseMenu;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public static GameObject Spawn(PauseMenuComponent pauseMenu) {
        GameObject menuObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(prefabName));
        menuObject.GetComponent<SaveMenuComponent>().pauseMenu = pauseMenu;
        GameObject parent = pauseMenu.gameObject.transform.parent.gameObject;
        Utils.AttachAndCenter(parent, menuObject);
        return menuObject;
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

    public IEnumerator ResumeRoutine() {
        yield return StartCoroutine(FadeOut());
        yield return pauseMenu.FadeIn();
        Global.Instance().input.RemoveListener(this);
        Destroy(this);
    }
}
