using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadingUIComponent : MonoBehaviour {

    public float FadeSeconds = 0.5f;
    public float FastModeFadeSeconds = 0.15f;
    public Texture2D fadeInTexture;
    public Texture2D fadeOutTexture;

    private float fadeDurationSeconds;
    private float targetAlpha;

    private float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public void Update() {
        if (Alpha < targetAlpha) {
            Alpha += Time.deltaTime / fadeDurationSeconds;
            if (Alpha > targetAlpha) Alpha = targetAlpha;
        } else if (Alpha > targetAlpha) {
            Alpha -= Time.deltaTime / fadeDurationSeconds;
            if (Alpha < targetAlpha) Alpha = targetAlpha;
        }
    }

    public void SetAlpha(float alpha) {
        Alpha = alpha;
        targetAlpha = alpha;
    }

    public float GetAlpha() {
        return Alpha;
    }

    public IEnumerator FadeInRoutine(float durationSeconds) {
        if (!gameObject.activeInHierarchy) {
            yield break;
        }
        this.fadeDurationSeconds = durationSeconds;
        this.targetAlpha = 1.0f;
        while (Alpha != targetAlpha) {
            yield return null;
        }
    }

    public IEnumerator FadeOutRoutine(float durationSeconds) {
        if (!gameObject.activeInHierarchy) {
            yield break;
        }
        this.fadeDurationSeconds = durationSeconds;
        this.targetAlpha = 0.0f;
        while (Alpha != targetAlpha) {
            yield return null;
        }
    }

    public IEnumerator Activate(ScenePlayer player = null) {
        if (gameObject.activeInHierarchy) {
            yield break;
        }
        gameObject.SetActive(true);
        if (fadeInTexture != null) {
            TransitionComponent transition = GetComponent<TransitionComponent>();
            if (Alpha < 1.0f) {
                transition.transitionDurationSeconds = GetFadeSeconds(player);
                StartCoroutine(transition.TransitionRoutine(fadeInTexture, true));
                yield return null;
                SetAlpha(1.0f);
                while (transition.IsTransitioning()) {
                    if (player != null && player.WasHurried()) {
                        transition.Hurry();
                    }
                    yield return null;
                }
            }
        } else {
            targetAlpha = 1.0f;
            fadeDurationSeconds = GetFadeSeconds(player);
            while (Alpha != targetAlpha) {
                if (player != null && player.WasHurried()) {
                    break;
                }
                yield return null;
            }
        }
    }

    public IEnumerator Deactivate(ScenePlayer player = null) {
        if (!gameObject.activeInHierarchy) {
            yield break;
        }
        if (fadeOutTexture != null) {
            TransitionComponent transition = GetComponent<TransitionComponent>();
            if (Alpha > 0.0f) {
                transition.transitionDurationSeconds = GetFadeSeconds(player);
                StartCoroutine(transition.TransitionRoutine(fadeOutTexture, false));
                yield return null;
                while (transition.IsTransitioning()) {
                    if (player != null && player.WasHurried()) {
                        transition.Hurry();
                    }
                    yield return null;
                }
            }
        } else {
            targetAlpha = 0.0f;
            fadeDurationSeconds = GetFadeSeconds(player);
            while (Alpha != targetAlpha) {
                if (player != null && player.WasHurried()) {
                    break;
                }
                yield return null;
            }
        }
        gameObject.SetActive(false);
    }

    private float GetFadeSeconds(ScenePlayer player) {
        if (player.ShouldUseFastMode()) {
            return FastModeFadeSeconds;
        } else {
            return FadeSeconds;
        }
    }
}
