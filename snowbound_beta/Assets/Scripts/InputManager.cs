using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    private List<KeyCode> hurryKeys;
    private List<KeyCode> pauseKeys;
    private List<InputListener> listeners;

    public void Awake() {
        hurryKeys = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space, KeyCode.Z });
        pauseKeys = new List<KeyCode>(new[] { KeyCode.Escape, KeyCode.C, KeyCode.Backspace });
        listeners = new List<InputListener>();
    }

    public void Update() {
        if (listeners.Count > 0) {
            InputListener listener = listeners[listeners.Count - 1];
            foreach (KeyCode code in hurryKeys) {
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
}
