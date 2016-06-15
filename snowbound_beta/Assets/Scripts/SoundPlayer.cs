using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour {

    public SoundEffectIndexData sounds;

    private AudioSource source;
    private Setting<float> volumeSetting;

    public void Awake() {
        source = GetComponent<AudioSource>();
        volumeSetting = Global.Instance().settings.GetFloatSetting(SettingsConstants.SoundEffectVolume);
    }

    public void Update() {
        source.volume = GetMaxVolume();
    }

    public void PlaySound(string soundTag) {
        source.clip = sounds.GetData(soundTag).clip;
        source.Play();
    }

    private float GetMaxVolume() {
        return volumeSetting.Value;
    }
}
