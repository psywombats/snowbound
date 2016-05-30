using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TransitionIndexData", menuName = "Data/TransitionIndexData")]
public class TransitionIndexData : ScriptableObject {

    public TransitionData[] transitions;

    private Dictionary<string, TransitionData> tagToTransition;

    public void OnEnable() {
        tagToTransition = new Dictionary<string, TransitionData>();
        foreach (TransitionData transition in transitions) {
            tagToTransition[transition.transitionTag] = transition;
        }
    }

    public TransitionData GetTransition(string tag) {
        return tagToTransition[tag.ToLower()];
    }
}
