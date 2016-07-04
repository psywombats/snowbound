using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "FadeData", menuName = "Data/FadeData")]
public class FadeData : ScriptableObject {

    public Texture2D transitionMask;
    public bool invert;
    public float delay;
    [Range(0.0f, 1.0f)] public float softEdgePercent;

}
