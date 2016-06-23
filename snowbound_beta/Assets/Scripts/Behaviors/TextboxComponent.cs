using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TextboxComponent : MonoBehaviour {

    private const float CharacterDelayMax = (1.0f / 20.0f);
    private const float CharacterDelayMin = (1.0f / 160.0f);
    private const float AdvancePromptFadeOutSeconds = 0.15f;
    private const float AdvancePromptFadeInSeconds = 0.3f;
    private const float FastModeHiccupSeconds = 0.05f;

    public Shader shader;
    public TextboxBackerComponent backer;
    public Text textbox;
    public Image advancePrompt;
    public SpeakerComponent speaker;
    public QuickMenuComponent quickMenu;

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
        characterSpeedSetting = Global.Instance().settings.GetFloatSetting(SettingsConstants.TextSpeed);
    }

    public void OnEnable() {
        Clear();
        if (speaker != null) speaker.GetComponent<FadingUIComponent>().SetAlpha(0.0f);
        if (backer != null) backer.GetComponent<FadingUIComponent>().SetAlpha(0.0f);
        if (quickMenu != null) quickMenu.GetComponent<FadingUIComponent>().SetAlpha(0.0f);
        AdvancePromptAlpha = 0.0f;
    }

    public void Clear() {
        textbox.text = "";
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

    public IEnumerator FadeInRoutine(ScenePlayer player, float durationSeconds) {
        List<IEnumerator> toRun = new List<IEnumerator>();
        if (speaker != null) toRun.Add(speaker.FadeInRoutine(durationSeconds));
        if (backer != null) toRun.Add(backer.FadeInRoutine(durationSeconds));
        if (quickMenu != null) toRun.Add(quickMenu.FadeInRoutine(durationSeconds));
        yield return player.StartCoroutine(Utils.RunParallel(toRun.ToArray(), player));
    }

    public IEnumerator FadeOutRoutine(ScenePlayer player, float durationSeconds) {
        List<IEnumerator> toRun = new List<IEnumerator>();
        if (speaker != null) toRun.Add(speaker.FadeOutRoutine(durationSeconds));
        if (backer != null) toRun.Add(backer.FadeOutRoutine(durationSeconds));
        if (quickMenu != null) toRun.Add(quickMenu.FadeOutRoutine(durationSeconds));
        yield return player.StartCoroutine(Utils.RunParallel(toRun.ToArray(), player));
    }

    public IEnumerator Activate(ScenePlayer player) {
        if (gameObject.activeInHierarchy) {
            yield break;
        }
        backer.GetComponent<Image>().material = backer.GetComponent<TransitionComponent>().GetMaterial();
        gameObject.SetActive(true);
        List<IEnumerator> toRun = new List<IEnumerator>();
        if (speaker != null) toRun.Add(speaker.Activate(player));
        if (backer != null) toRun.Add(backer.Activate(player));
        if (quickMenu != null) toRun.Add(quickMenu.Activate(player));
        yield return player.StartCoroutine(Utils.RunParallel(toRun.ToArray(), player));
    }

    public IEnumerator Deactivate(ScenePlayer player) {
        if (!gameObject.activeInHierarchy) {
            yield break;
        }
        List<IEnumerator> toRun = new List<IEnumerator>();
        if (speaker != null) toRun.Add(speaker.Deactivate(player));
        if (backer != null) toRun.Add(backer.Deactivate(player));
        if (quickMenu != null) toRun.Add(quickMenu.Deactivate(player));
        yield return player.StartCoroutine(Utils.RunParallel(toRun.ToArray(), player));
        gameObject.SetActive(false);
    }

    private float GetCharacterDelay() {
        return CharacterDelayMax + ((CharacterDelayMin - CharacterDelayMax) * characterSpeedSetting.Value);
    }
}
