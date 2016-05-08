using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    private List<KeyCode> hurryKeys;
    private bool wasHurried;

    public void Start() {
        hurryKeys = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space, KeyCode.Z });
    }

    public void Update() {
        foreach (KeyCode code in hurryKeys) {
            if (Input.GetKeyDown(code)) {
                wasHurried = true;
            }
        }
    }

    public IEnumerator AwaitHurry() {
        while (!WasHurried()) {
            yield return null;
        }
        AcknowledgeHurried();
    }

    public bool WasHurried() {
        return wasHurried;
    }

    public void AcknowledgeHurried() {
        wasHurried = false;
    }
}
