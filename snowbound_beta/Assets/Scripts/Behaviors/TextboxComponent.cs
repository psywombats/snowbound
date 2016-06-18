using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

[RequireComponent(typeof(TransitionComponent))]
[RequireComponent(typeof(FadingUIComponent))]
public class TextboxComponent : MonoBehaviour {

    private const float CharacterDelayMax = (1.0f / 20.0f);
    private const float CharacterDelayMin = (1.0f / 160.0f);
    private const float AdvancePromptFadeOutSeconds = 0.15f;
    private const float AdvancePromptFadeInSeconds = 0.3f;
    private const float FastModeHiccupSeconds = 0.05f;

    public Shader shader;
    public Image backer;
    public Text textbox;
    public Image advancePrompt;
    public SpeakerComponent speaker;

    private Setting<float> characterSpeedSetting;
    private string fullText;

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

    public void OnEnable() {
        SetAlpha(0.0f);
        advancePrompt.gameObject.SetActive(false);
        Clear();
    }

    public void Clear() {
        textbox.text = "";
    }

    public void SetAlpha(float alpha) {
        GetComponent<FadingUIComponent>().SetAlpha(alpha);
    }

    public void FadeAdvancePrompt(bool fadeIn) {
        advancePrompt.CrossFadeAlpha(fadeIn? 1.0f : 0.0f, AdvancePromptFadeOutSeconds, false);
    }

    public IEnumerator ShowText(ScenePlayer player, string text) {
        fullText = text;
        FadeAdvancePrompt(false);
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
        FadeAdvancePrompt(true);
    }

    public IEnumerator FadeInRoutine(float durationSeconds) {
        yield return StartCoroutine(GetComponent<FadingUIComponent>().FadeInRoutine(durationSeconds));
    }

    public IEnumerator FadeOutRoutine(float durationSeconds) {
        yield return StartCoroutine(GetComponent<FadingUIComponent>().FadeOutRoutine(durationSeconds));
    }

    public IEnumerator Activate(ScenePlayer player) {
        if (speaker != null && speaker.HasChara()) {
            yield return player.StartCoroutine(Utils.RunParallel(new[] {
                GetComponent<FadingUIComponent>().Activate(player),
                speaker.Activate(player)
            }, player));
        } else {
            yield return player.StartCoroutine(GetComponent<FadingUIComponent>().Activate(player));
        }
    }

    public IEnumerator Deactivate(ScenePlayer player) {
        if (speaker != null) {
            yield return player.StartCoroutine(Utils.RunParallel(new[] {
                GetComponent<FadingUIComponent>().Deactivate(player),
                speaker.Deactivate(player)
            }, player));
        } else {
            yield return player.StartCoroutine(GetComponent<FadingUIComponent>().Deactivate(player));
        }
    }

    private float GetCharacterDelay() {
        return CharacterDelayMax + ((CharacterDelayMin - CharacterDelayMax) * characterSpeedSetting.Value);
    }
}
