using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TransitionData", menuName = "Data/TransitionData")]
public class TransitionData : GenericDataObject {

    public Texture2D transitionMask;
    public float delay;

}
