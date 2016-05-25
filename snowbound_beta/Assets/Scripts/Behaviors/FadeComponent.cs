using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeComponent : MonoBehaviour {

    public Image image = null;
    public bool autoFadeIn = false;
    public float fadeTime = 0.8f;

    private float Alpha {
        get { return image.color.a; }
        set { image.color = new Color(image.color.r, image.color.g, image.color.b, value); }
    }

    public void Awake() {
        if (autoFadeIn) {
            Alpha = 1.0f;
        }
    }

    public void Start() {
        if (autoFadeIn) {
            RemoveTint();
            autoFadeIn = false;
        }
    }

    public void FadeToBlack() {
        StartCoroutine(FadeToBlackRoutine());
    }

    public void RemoveTint() {
        StartCoroutine(RemoveTintRoutine());
    }
    
    public IEnumerator FadeToBlackRoutine() {
        image.CrossFadeAlpha(1.0f, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);
    }

    public IEnumerator RemoveTintRoutine() {
        image.CrossFadeAlpha(0.0f, fadeTime, false);
        yield return new WaitForSeconds(fadeTime);
    }
}
