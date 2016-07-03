using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeComponent : MonoBehaviour {

    public Image image = null;
    public bool autoFadeIn = false;
    public float fadeTime = 0.8f;
    public float fastModeHiccupTime = 0.1f;

    private BGMPlayer bgm;
    private bool fading;

    private float Alpha {
        get { return image.color.a; }
        set { image.color = new Color(image.color.r, image.color.g, image.color.b, value); }
    }

    public void Awake() {
        bgm = FindObjectOfType<BGMPlayer>();
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

    public bool IsFading() {
        return fading;
    }

    public void Clear() {
        Alpha = 0.0f;
    }

    public void InstantFade() {
        Alpha = 1.0f;
    }
    
    public IEnumerator FadeToBlackRoutine(bool allowFastMode = false, bool fadeBGM = true) {
        ScenePlayer player = FindObjectOfType<ScenePlayer>();
        fading = true;

        gameObject.transform.SetAsLastSibling();

        if (player != null && player.ShouldUseFastMode()) {
            yield return new WaitForSeconds(fastModeHiccupTime);
        } else {
            image.CrossFadeAlpha(1.0f, fadeTime, false);
            if (bgm != null && fadeBGM) {
                StartCoroutine(bgm.FadeOutRoutine(fadeTime));
            }
            yield return new WaitForSeconds(fadeTime);
        }

        fading = false;
    }

    public IEnumerator RemoveTintRoutine(bool allowFastMode = false) {
        ScenePlayer player = FindObjectOfType<ScenePlayer>();
        fading = true;

        gameObject.transform.SetAsLastSibling();

        image.CrossFadeAlpha(1.0f, 0.0f, false);
        if (player != null && player.ShouldUseFastMode()) {
            yield return new WaitForSeconds(fastModeHiccupTime);
        } else {
            image.CrossFadeAlpha(0.0f, fadeTime, false);
            yield return new WaitForSeconds(fadeTime);
        }

        fading = false;
    }
}
