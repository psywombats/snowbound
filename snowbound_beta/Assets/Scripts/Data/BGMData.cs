using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "BGMData", menuName = "Data/BGMData")]
public class BGMData : ScriptableObject {

    public AudioClip track;
    public string tag;

}
