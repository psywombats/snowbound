using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharaIndexData", menuName = "Data/CharaIndexData")]
public class CharaIndexData : ScriptableObject {

    public CharaData[] charas;

    private Dictionary<string, CharaData> tagToChara;

    public void OnEnable() {
        tagToChara = new Dictionary<string, CharaData>();
        foreach (CharaData chara in charas) {
            tagToChara[chara.tag] = chara;
        }
    }

    public CharaData GetChara(string tag) {
        return tagToChara[tag.ToLower()];
    }
}
