using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

[RequireComponent(typeof(TransitionComponent))]
public class TextboxComponent : MonoBehaviour {

    private const float CharacterDelayMax = (1.0f / 20.0f);
    private const float CharacterDelayMin = (1.0f / 160.0f);
    private const float TextboxFadeSeconds = 0.5f;
    private const float FastModeHiccupSeconds = 0.05f;
    private const float FastModeFadeSeconds = 0.15f;
    private const float AdvancePromptFadeOutSeconds = 0.15f;
    private const float AdvancePromptFadeInSeconds = 0.3f;

    public Shader shader;
    public Image backer;
    public Text textbox;
    public Texture2D fadeInTexture;
    public Texture2D fadeOutTexture;
    public Image advancePrompt;

    private Setting<float> characterSpeedSetting;
    private string fullText;
    private float fadeDurationSeconds;
    private float targetAlpha;
    
    private float Alpha {
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

    public void Awake() {
        backer.material = GetComponent<TransitionComponent>().GetMaterial();
        characterSpeedSetting = Global.Instance().settings.GetFloatSetting(SettingsConstants.TextSpeed);
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

    public void OnEnable() {
        SetAlpha(0.0f);
        advancePrompt.gameObject.SetActive(false);
    }

    public IEnumerator FadeInRoutine(float durationSeconds) {
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

    public IEnumerator ShowText(ScenePlayer player, string text) {
        fullText = text;
        advancePrompt.CrossFadeAlpha(0.0f, AdvancePromptFadeOutSeconds, false);
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
            yield return new WaitForSeconds(GetCharacterDelay());
        }
        textbox.text = fullText;
        if (player.ShouldUseFastMode()) {
            yield return new WaitForSeconds(FastModeHiccupSeconds);
        }

        advancePrompt.gameObject.SetActive(true);
        AdvancePromptAlpha = 0.0f;
        advancePrompt.CrossFadeAlpha(1.0f, AdvancePromptFadeInSeconds, false);
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
                SetAlpha(1.0f);
                while (transition.IsTransitioning()) {
                    if (player.WasHurried()) {
                        transition.Hurry();
                    }
                    yield return null;
                }
            }
        } else {
            targetAlpha = 1.0f;
            fadeDurationSeconds = GetFadeSeconds(player);
            while (Alpha != targetAlpha) {
                if (player.WasHurried()) {
                    break;
                }
                yield return null;
            }
        }
        SetAlpha(1.0f);
    }

    public IEnumerator Deactivate(ScenePlayer player) {
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
                    if (player.WasHurried()) {
                        transition.Hurry();
                    }
                    yield return null;
                }
            }
        } else {
            targetAlpha = 0.0f;
            fadeDurationSeconds = GetFadeSeconds(player);
            while (Alpha != targetAlpha) {
                if (player.WasHurried()) {
                    break;
                }
                yield return null;
            }
        }
        SetAlpha(0.0f);
        gameObject.SetActive(false);
        Clear();
    }

    public void Clear() {
        textbox.text = "";
    }

    public void SetAlpha(float alpha) {
        Alpha = alpha;
        targetAlpha = alpha;
    }

    private float GetFadeSeconds(ScenePlayer player) {
        if (player.ShouldUseFastMode()) {
            return FastModeFadeSeconds;
        } else {
            return TextboxFadeSeconds;
        }
    }

    private float GetCharacterDelay() {
        return CharacterDelayMax + ((CharacterDelayMin - CharacterDelayMax) * characterSpeedSetting.Value);
    }
}
