using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveButtonComponent : MonoBehaviour {

    public Button button;
    public Text dataText;
    public Text captionText;

    private SaveMenuComponent menu;
    private int slot;

    public void Awake() {
        button.onClick.AddListener(() => {
            menu.SaveOrLoadFromSlot(slot);
        });
    }

    public void Populate(SaveMenuComponent menu, int slot, Memory memory, SaveMenuComponent.SaveMenuMode mode) {
        this.menu = menu;
        this.slot = slot;

        dataText.text = "slot0" + (slot + 1);
        if (memory == null) {
            if (mode == SaveMenuComponent.SaveMenuMode.Load) {
                button.enabled = false;
                dataText.text = "<no data>";
            }
            captionText.text = "";
        } else {
            captionText.text = System.String.Format("{0:g}", memory.savedAt);
        }
    }
}
