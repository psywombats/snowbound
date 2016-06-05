using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuComponent : MonoBehaviour {

    private const string PrefabName = "Prefabs/SettingsMenu";
    private const float FadeoutSeconds = 0.2f;

    public SettingSliderComponent[] sliders;
    public Button cancelButton;
    public Button applyButton;

    private Action onFinish;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public void Awake() {
        cancelButton.onClick.AddListener(() => {
            StartCoroutine(FadeOutRoutine());
        });
        applyButton.onClick.AddListener(() => {
            Apply();
            StartCoroutine(FadeOutRoutine());
        });
    }

    public static GameObject Spawn(GameObject parent, Action onFinish) {
        GameObject menuObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(PrefabName));
        menuObject.GetComponent<SettingsMenuComponent>().onFinish = onFinish;
        Utils.AttachAndCenter(parent, menuObject);
        return menuObject;
    }

    public IEnumerator FadeInRoutine() {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        Alpha = 1.0f;
    }

    public IEnumerator FadeOutRoutine() {
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        Alpha = 0.0f;

        if (onFinish != null) {
            onFinish();
        }
        Destroy(gameObject);
    }

    private void Apply() {
        foreach (SettingSliderComponent slider in sliders) {
            slider.Apply();
        }
    }
}
