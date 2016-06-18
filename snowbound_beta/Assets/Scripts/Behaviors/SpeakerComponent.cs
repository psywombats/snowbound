using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(FadingUIComponent))]
public class SpeakerComponent : MonoBehaviour {

    public CharaData chara;
    public Image portraitImage;
    public Text nametag;

    public void SetChara(CharaData chara) {
        this.chara = chara;
        if (chara == null) {
            GetComponent<FadingUIComponent>().SetAlpha(0.0f);
        } else {
            GetComponent<FadingUIComponent>().SetAlpha(1.0f);
            this.nametag.text = chara.displayName;
            this.portraitImage.sprite = chara.portrait;
        }
    }

    public void TransitionToChara(CharaData chara) {
        SetChara(chara);
    }

    public bool HasChara() {
        return chara != null;
    }

    public void QuickActivate() {
        SetChara(null);
        GetComponent<FadingUIComponent>().SetAlpha(1.0f);
    }

    public IEnumerator FadeInRoutine(float durationSeconds) {
        yield return StartCoroutine(GetComponent<FadingUIComponent>().FadeInRoutine(durationSeconds));
    }

    public IEnumerator FadeOutRoutine(float durationSeconds) {
        yield return StartCoroutine(GetComponent<FadingUIComponent>().FadeOutRoutine(durationSeconds));
    }

    public IEnumerator Activate(ScenePlayer player) {
        yield return player.StartCoroutine(GetComponent<FadingUIComponent>().Activate(player));
    }

    public IEnumerator Deactivate(ScenePlayer player) {
        yield return player.StartCoroutine(GetComponent<FadingUIComponent>().Deactivate(player));
    }
}
