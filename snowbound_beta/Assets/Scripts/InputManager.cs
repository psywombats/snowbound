using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    private List<KeyCode> hurryKeys;
    private List<KeyCode> pauseKeys;
    private bool wasHurried;

    public void Awake() {
        hurryKeys = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space, KeyCode.Z });
        pauseKeys = new List<KeyCode>(new[] { KeyCode.Escape, KeyCode.C, KeyCode.Backspace });
    }

    public void Update() {
        foreach (KeyCode code in hurryKeys) {
            if (Input.GetKeyDown(code)) {
                wasHurried = true;
            }
        }
        foreach (KeyCode code in pauseKeys) {
            if (Input.GetKeyDown(code)) {
                Global.Instance().activeScenePlayer.Pause();
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
