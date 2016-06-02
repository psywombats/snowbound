using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BGMIndexData", menuName = "Data/BGMIndexData")]
public class BGMIndexData : ScriptableObject {

    public BGMData[] bgms;

    private Dictionary<string, BGMData> tagToBGM;

    public void OnEnable() {
        tagToBGM = new Dictionary<string, BGMData>();
        foreach (BGMData bgm in bgms) {
            tagToBGM[bgm.tag] = bgm;
        }
    }

    public BGMData GetBGM(string tag) {
        return tagToBGM[tag.ToLower()];
    }
}
