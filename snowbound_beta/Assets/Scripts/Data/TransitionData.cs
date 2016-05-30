using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TransitionData", menuName = "Data/TransitionData")]
public class TransitionData : ScriptableObject {

    public Texture2D transitionMask;
    public string transitionTag;
    public float delay;

}
