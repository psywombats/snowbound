using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveButtonComponent : MonoBehaviour {

    public Button button;
    public Text dataText;
    public Text captionText;

    public void Populate(int slot, Memory memory) {
        dataText.text = "slot0" + slot;
        if (memory == null) {
            button.enabled = false;
            captionText.text = System.String.Format("{0:g}", memory.savedAt);
        } else {
            captionText.text = "";
        }
    }
}
