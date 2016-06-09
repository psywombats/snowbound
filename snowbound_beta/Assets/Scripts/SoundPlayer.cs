using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour {

    private AudioSource source;
    private Setting<float> volumeSetting;
    private ScenePlayer player;

    public void Awake() {
        source = GetComponent<AudioSource>();
        player = FindObjectOfType<ScenePlayer>();
        volumeSetting = Global.Instance().settings.GetFloatSetting(SettingsConstants.SoundEffectVolume);
    }

    public void Update() {
        source.volume = GetMaxVolume();
    }

    public void PlaySound(string soundTag) {
        source.clip = player.GetSoundEffect(soundTag).clip;
        source.Play();
    }

    private float GetMaxVolume() {
        return volumeSetting.Value;
    }
}
