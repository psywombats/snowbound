﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

[RequireComponent(typeof(TransitionComponent))]
public class TextboxComponent : MonoBehaviour {

    private const float characterDelay = (1 / 32f);
    private const float textboxFadeSeconds = 0.5f;
    private const float fastModeHiccupSeconds = 0.05f;
    private const float fastModeFadeSeconds = 0.15f;
    private const float advancePromptFadeOutSeconds = 0.15f;
    private const float advancePromptFadeInSeconds = 0.3f;

    public Shader shader;
    public Image backer;
    public Text textbox;
    public Texture2D fadeInTexture;
    public Texture2D fadeOutTexture;
    public Image advancePrompt;

    private string fullText;
    
    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public float Height {
        get { return GetComponent<RectTransform>().rect.height; }
    }

    private float AdvancePromptAlpha {
        get { return advancePrompt.GetComponent<CanvasRenderer>().GetAlpha(); }
        set { advancePrompt.GetComponent<CanvasRenderer>().SetAlpha(value); }
    }

    public void Start() {
        backer.material = GetComponent<TransitionComponent>().GetMaterial();
    }

    public void OnEnable() {
        Alpha = 0.0f;
        advancePrompt.gameObject.SetActive(false);
    }

    public IEnumerator ShowText(ScenePlayer player, string text) {
        fullText = text;
        advancePrompt.CrossFadeAlpha(0.0f, advancePromptFadeOutSeconds, false);
        for (int i = 0; i <= fullText.Length; i += 1) {
            if (player.IsSuspended()) {
                yield return null;
            }
            if (player.WasHurried()) {
                player.AcknowledgeHurried();
                textbox.text = fullText;
                break;
            }
            if (player.ShouldUseFastMode()) {
                break;
            }
            textbox.text = fullText.Substring(0, i);
            textbox.text += "<color=#00000000>";
            textbox.text += fullText.Substring(i);
            textbox.text += "</color>";
            yield return new WaitForSeconds(characterDelay);
        }
        textbox.text = fullText;
        if (player.ShouldUseFastMode()) {
            yield return new WaitForSeconds(fastModeHiccupSeconds);
        }

        advancePrompt.gameObject.SetActive(true);
        AdvancePromptAlpha = 0.0f;
        advancePrompt.CrossFadeAlpha(1.0f, advancePromptFadeInSeconds, false);
    }

    public IEnumerator FadeIn(float durationSeconds) {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / durationSeconds;
            yield return null;
        }
        Alpha = 1.0f;
    }

    public IEnumerator FadeOut(float durationSeconds) {
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / durationSeconds;
            yield return null;
        }
        Alpha = 0.0f;
    }

    public IEnumerator Activate(ScenePlayer player) {
        gameObject.SetActive(true);
        advancePrompt.gameObject.SetActive(false);
        Clear();
        if (fadeInTexture != null) {
            TransitionComponent transition = GetComponent<TransitionComponent>();
            if (Alpha < 1.0f) {
                transition.transitionDurationSeconds = GetFadeSeconds(player);
                StartCoroutine(transition.TransitionRoutine(fadeInTexture, true));
                yield return null;
                Alpha = 1.0f;
                while (transition.IsTransitioning()) {
                    if (player.WasHurried()) {
                        transition.Hurry();
                    }
                    yield return null;
                }
            }
        } else {
            while (Alpha < 1.0f) {
                if (player.WasHurried()) {
                    break;
                }
                Alpha += Time.deltaTime / GetFadeSeconds(player);
                yield return null;
            }
        }
        Alpha = 1.0f;
    }

    public IEnumerator Deactivate(ScenePlayer player) {
        if (fadeOutTexture != null) {
            TransitionComponent transition = GetComponent<TransitionComponent>();
            if (Alpha > 0.0f) {
                transition.transitionDurationSeconds = GetFadeSeconds(player);
                StartCoroutine(transition.TransitionRoutine(fadeOutTexture, false));
                yield return null;
                while (transition.IsTransitioning()) {
                    if (player.WasHurried()) {
                        transition.Hurry();
                    }
                    yield return null;
                }
            }
        } else {
            while (Alpha > 0.0f) {
                if (player.WasHurried()) {
                    break;
                }
                Alpha -= Time.deltaTime / GetFadeSeconds(player);
                yield return null;
            }
        }
        Alpha = 0.0f;
        gameObject.SetActive(false);
        Clear();
    }

    public void Clear() {
        textbox.text = "";
    }

    private float GetFadeSeconds(ScenePlayer player) {
        if (player.ShouldUseFastMode()) {
            return fastModeFadeSeconds;
        } else {
            return textboxFadeSeconds;
        }
    }
}
