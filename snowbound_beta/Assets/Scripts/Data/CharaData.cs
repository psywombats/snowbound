using UnityEngine;

[CreateAssetMenu(fileName = "CharaData", menuName = "Data/CharaData", order = 1)]
public class CharaData : ScriptableObject {

    public string tag;
    public string displayName;
    public Sprite portrait;

}
