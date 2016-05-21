using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TextboxComponent : MonoBehaviour {

    private const float characterDelay = (1 / 32f);
    private const float textboxFadeSeconds = 0.5f;
    private const float fastModeHiccupSeconds = 0.08f;
    private const float fastModeFadeSeconds = 0.15f;

    public Image backer;
    public Text textbox;
    private string fullText;
    
    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public float Height {
        get { return GetComponent<RectTransform>().rect.height; }
    }

    public IEnumerator ShowText(ScenePlayer player, string text) {
        fullText = text;
        for (int i = 0; i <= fullText.Length; i += 1) {
            if (player.WasHurried()) {
                player.AcknowledgeHurried();
                textbox.text = fullText;
                break;
            }
            if (player.IsSuspended()) {
                yield return null;
            }
            if (Global.Instance().input.IsFastKeyDown()) {
                break;
            }
            textbox.text = fullText.Substring(0, i);
            textbox.text += "<color=#00000000>";
            textbox.text += fullText.Substring(i);
            textbox.text += "</color>";
            yield return new WaitForSeconds(characterDelay);
        }
        textbox.text = fullText;
        if (Global.Instance().input.IsFastKeyDown()) {
            yield return new WaitForSeconds(fastModeHiccupSeconds);
        }
    }

    public IEnumerator FadeOut(float durationSeconds) {
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / durationSeconds;
            yield return null;
        }
        Alpha = 0.0f;
    }

    public IEnumerator FadeIn(float durationSeconds) {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / durationSeconds;
            yield return null;
        }
        Alpha = 1.0f;
    }

    public IEnumerator Deactivate(ScenePlayer player) {
        while (Alpha > 0.0f) {
            if (player.WasHurried()) {
                break;
            }
            Alpha -= Time.deltaTime / GetFadeoutSeconds();
            yield return null;
        }
        Alpha = 0.0f;
        gameObject.SetActive(false);
        Clear();
    }

    public IEnumerator Activate(ScenePlayer player) {
        Clear();
        gameObject.SetActive(true);
        while (Alpha < 1.0f) {
            if (player.WasHurried()) {
                break;
            }
            Alpha += Time.deltaTime / GetFadeoutSeconds();
            yield return null;
        }
        Alpha = 1.0f;
    }

    public void Clear() {
        textbox.text = "";
    }

    private float GetFadeoutSeconds() {
        if (Global.Instance().input.IsFastKeyDown()) {
            return fastModeFadeSeconds;
        } else {
            return textboxFadeSeconds;
        }
    }
}
