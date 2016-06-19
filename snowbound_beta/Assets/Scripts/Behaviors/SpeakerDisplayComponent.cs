using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FadingUIComponent))]
public class SpeakerDisplayComponent : MonoBehaviour {

    public CharaData chara;
    public Image portraitImage;
    public Text nametag;

    public void SetChara(CharaData chara) {
        this.chara = chara;
        if (chara != null) {
            portraitImage.sprite = chara.portrait;
            nametag.text = chara.name;
        }
    }
}
