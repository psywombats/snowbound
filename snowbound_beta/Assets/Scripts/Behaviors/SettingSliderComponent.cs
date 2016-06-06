using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Scrollbar))]
public class SettingSliderComponent : MonoBehaviour {

    public string settingName;
    public Text label;
    public bool percentDisplayMode;
    public string[] gradiatedLabels;
    
    private Scrollbar scrollbar;
    private Setting<float> setting;
    private Vector2 originalLabelLocation;
    private bool dirty;

    public void Awake() {
        scrollbar = GetComponent<Scrollbar>();
        setting = Global.Instance().settings.GetFloatSetting(settingName);
        originalLabelLocation = label.transform.localPosition;
        MatchDisplayToSetting();
        dirty = false;
    }

    public void Update() {
        MatchLabelDisplayToScrollbar();
    }

    public void Apply() {
        dirty = false;
        MatchSettingToDisplay();
    }

    public bool IsDirty() {
        return Math.Abs(setting.Value - scrollbar.value) > 0.02;
    }

    private void MatchDisplayToSetting() {
        scrollbar.value = setting.Value;
        MatchLabelDisplayToScrollbar();
    }

    private void MatchSettingToDisplay() {
        setting.Value = scrollbar.value;
    }

    private void MatchLabelDisplayToScrollbar() {
        if (percentDisplayMode) {
            label.text = Mathf.Round(scrollbar.value * 100.0f) + "%";
        } else {
            int index = Mathf.RoundToInt((float)(gradiatedLabels.Length - 1) * scrollbar.value);
            label.text = gradiatedLabels[index];
        }

        float maximum = scrollbar.GetComponent<RectTransform>().rect.width - (originalLabelLocation.x * 2.0f);
        label.transform.localPosition = new Vector2(originalLabelLocation.x + maximum * scrollbar.value, originalLabelLocation.y);
    }
}
