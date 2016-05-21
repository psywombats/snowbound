using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    private List<KeyCode> advanceKeys;
    private List<KeyCode> pauseKeys;
    private List<KeyCode> fastKeys;
    private List<InputListener> listeners;

    public void Awake() {
        advanceKeys = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space, KeyCode.Z });
        pauseKeys = new List<KeyCode>(new[] { KeyCode.Escape, KeyCode.C, KeyCode.Backspace });
        fastKeys = new List<KeyCode>(new[] { KeyCode.LeftControl, KeyCode.RightControl });
        listeners = new List<InputListener>();
    }

    public void Update() {
        if (listeners.Count > 0) {
            InputListener listener = listeners[listeners.Count - 1];
            foreach (KeyCode code in advanceKeys) {
                if (Input.GetKeyDown(code)) {
                    listener.OnEnter();
                }
            }
            foreach (KeyCode code in pauseKeys) {
                if (Input.GetKeyDown(code)) {
                    listener.OnEscape();
                }
            }
        }
    }

    public void PushListener(InputListener listener) {
        listeners.Add(listener);
    }

    public void RemoveListener(InputListener listener) {
        listeners.Remove(listener);
    }

    public bool IsFastKeyDown() {
        foreach (KeyCode code in fastKeys) {
            if (Input.GetKey(code)) {
                return true;
            }
        }
        return false;
    }
}
