using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BackgroundIndexData", menuName = "Data/BackgroundIndexData")]
public class BackgroundIndexData : ScriptableObject {

    public BackgroundData[] backgrounds;

    private Dictionary<string, BackgroundData> tagToBackground;

    public void OnEnable() {
        tagToBackground = new Dictionary<string, BackgroundData>();
        foreach (BackgroundData background in backgrounds) {
            tagToBackground[background.backgroundTag] = background;
        }
    }

    public BackgroundData GetBackground(string tag) {
        return tagToBackground[tag.ToLower()];
    }
}
