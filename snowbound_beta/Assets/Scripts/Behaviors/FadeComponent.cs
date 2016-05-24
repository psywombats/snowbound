using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeComponent : MonoBehaviour {

    public Image image = null;
    public bool autoFadeIn = false;
    public float fadeTime = 0.8f;

    private float Alpha {
        get { return image.GetComponent<CanvasRenderer>().GetColor().a; }
        set { image.GetComponent<CanvasRenderer>().SetAlpha(value); }
    }

    public void Awake() {
        if (autoFadeIn) {
            Alpha = 1.0f;
        }
    }

    public void Start() {
        if (autoFadeIn) {
            FadeIn();
            autoFadeIn = false;
        }
    }

    public void FadeIn() {
        StartCoroutine(FadeInRoutine());
    }

    public void FadeOut() {
        StartCoroutine(FadeOutRoutine());
    }
    
    public IEnumerator FadeInRoutine() {
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / fadeTime;
            yield return null;
        }
        if (Alpha < 0.0f) {
            Alpha = 0.0f;
        }
    }

    public IEnumerator FadeOutRoutine() {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / fadeTime;
            yield return null;
        }
        if (Alpha > 1.0f) {
            Alpha = 1.0f;
        }
    }
}
