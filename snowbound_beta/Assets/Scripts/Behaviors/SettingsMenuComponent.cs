using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuComponent : MenuComponent {

    private const string PrefabName = "Prefabs/SettingsMenu";
    private const float FadeoutSeconds = 0.2f;

    public SettingSliderComponent[] sliders;
    public Button cancelButton;
    public Button applyButton;

    public void Awake() {
        cancelButton.onClick.AddListener(() => {
            StartCoroutine(ResumeRoutine());
        });
        applyButton.onClick.AddListener(() => {
            Apply();
            StartCoroutine(ResumeRoutine());
        });
    }

    public static GameObject Spawn(GameObject parent, Action onFinish) {
        return Spawn(parent, PrefabName, onFinish);
    }

    private void Apply() {
        foreach (SettingSliderComponent slider in sliders) {
            slider.Apply();
        }
    }
}
