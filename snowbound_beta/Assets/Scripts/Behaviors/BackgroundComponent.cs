using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Sprite))]
public class BackgroundComponent : MonoBehaviour {

    private BackgroundData currentBackground;
    private ScenePlayer player;

    public void Awake() {
        player = FindObjectOfType<ScenePlayer>();
    }

	public void SetBackground(string backgroundTag) {
        currentBackground = player.GetBackground(backgroundTag);
        UpdateDisplay();
    }

    public void PopuateMemory(ScreenMemory memory) {
        if (currentBackground != null) {
            memory.backgroundTag = currentBackground.backgroundTag;
        }
    }

    public void PopulateFromMemory(ScreenMemory memory) {
        currentBackground = player.GetBackground(memory.backgroundTag);
        UpdateDisplay();
    }

    private void UpdateDisplay() {
        GetComponent<SpriteRenderer>().sprite = currentBackground.background;
    }
}
