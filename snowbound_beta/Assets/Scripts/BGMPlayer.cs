using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour {

    public float fadeSeconds = 0.5f;

    private AudioSource source;
    private ScenePlayer player;
    private BGMData currentTrack;
    private Setting<float> volumeSetting;
    private float preprocessingVolume;

    public void Awake() {
        source = GetComponent<AudioSource>();
        player = FindObjectOfType<ScenePlayer>();
        volumeSetting = Global.Instance().settings.GetFloatSetting(SettingsConstants.BGMVolume);
    }

    public void Update() {
        source.volume = preprocessingVolume * GetMaxVolume();
    }

    public void PopulateMemory(ScreenMemory memory) {
        if (currentTrack != null) {
            memory.bgmTag = currentTrack.tag;
        }
    }

    public void PopulateFromMemory(ScreenMemory memory) {
        if (memory.bgmTag != null && memory.bgmTag.Length > 0) {
            currentTrack = player.GetBGM(memory.bgmTag);
            source.clip = currentTrack.track;
            source.Play();
        }
    }

    public IEnumerator FadeOutRoutine() {
        return FadeOutRoutine(fadeSeconds);
    }

    public IEnumerator FadeOutRoutine(float seconds) {
        while (currentTrack != null && source.volume > 0.0f) {
            preprocessingVolume -= Time.deltaTime / seconds;
            yield return null;
        }
    }

    public IEnumerator CrossfadeRoutine(string bgmTag) {
        return CrossfadeRoutine(bgmTag, fadeSeconds);
    }

    public IEnumerator CrossfadeRoutine(string bgmTag, float seconds) {
        yield return FadeOutRoutine(seconds);
        source.Stop();

        BGMData newTrack = player.GetBGM(bgmTag);
        currentTrack = newTrack;
        source.clip = newTrack.track;
        preprocessingVolume = 1.0f;

        if (newTrack != null) {
            source.Play();
        }
    }

    private float GetMaxVolume() {
        return volumeSetting.Value;
    }
}
