using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "BackgroundData", menuName = "Data/BackgroundData")]
public class BackgroundData : ScriptableObject {

    public Sprite background;
    public string backgroundTag;

}
