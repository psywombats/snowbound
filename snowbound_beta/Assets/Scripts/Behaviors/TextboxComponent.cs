using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TextboxComponent : MonoBehaviour {

    private const float characterDelay = (1 / 32f);
    private const float textboxFadeDuration = 0.5f;

    public Image backer;
    public Text textbox;
    private string fullText;
    
    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public float height {
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
            textbox.text = fullText.Substring(0, i);
            textbox.text += "<color=#00000000>";
            textbox.text += fullText.Substring(i);
            textbox.text += "</color>";
            yield return new WaitForSeconds(characterDelay);
        }
    }

    public IEnumerator FadeOut(ScenePlayer player) {
        while (Alpha > 0.0f) {
            if (player.WasHurried()) {
                break;
            }
            Alpha -= Time.deltaTime / textboxFadeDuration;
            yield return null;
        }
        Alpha = 0.0f;
        gameObject.SetActive(false);
        Clear();
    }

    public IEnumerator FadeIn(ScenePlayer player) {
        Clear();
        gameObject.SetActive(true);
        while (Alpha < 1.0f) {
            if (player.WasHurried()) {
                break;
            }
            Alpha += Time.deltaTime / textboxFadeDuration;
            yield return null;
        }
        Alpha = 1.0f;
    }

    public void Clear() {
        textbox.text = "";
    }
}
