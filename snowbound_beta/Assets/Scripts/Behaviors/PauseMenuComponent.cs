using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenuComponent : MonoBehaviour {

    private const float fadeoutSeconds = 0.3f;
    private const string prefabName = "Prefabs/PauseMenu";

    public Button saveButton;
    public Button loadButton;
    public Button resumeButton;
    public Button closeButton;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public static GameObject Spawn() {
        return UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(prefabName));
    }

    public void Awake() {
        saveButton.onClick.AddListener(() => {
            
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
        Destroy(this);
    }
}
