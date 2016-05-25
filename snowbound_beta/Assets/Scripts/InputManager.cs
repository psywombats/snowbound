using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    private List<KeyCode> advanceKeys;
    private List<KeyCode> pauseKeys;
    private List<KeyCode> fastKeys;
    private List<InputListener> listeners;
    private List<InputListener> disabledListeners;

    private List<InputListener> listenersToPush;
    private List<InputListener> listenersToRemove;

    public void Awake() {
        advanceKeys = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space, KeyCode.Z });
        pauseKeys = new List<KeyCode>(new[] { KeyCode.Escape, KeyCode.C, KeyCode.Backspace });
        fastKeys = new List<KeyCode>(new[] { KeyCode.LeftControl, KeyCode.RightControl });
        listeners = new List<InputListener>();

        listenersToPush = new List<InputListener>();
        listenersToRemove = new List<InputListener>();
        disabledListeners = new List<InputListener>();
    }

    public void Update() {
        listeners.AddRange(listenersToPush);
        foreach (InputListener listener in listenersToRemove) {
            listeners.Remove(listener);
        }
        listenersToRemove.Clear();
        listenersToPush.Clear();

        if (listeners.Count > 0) {
            InputListener listener = listeners[listeners.Count - 1];
            if (disabledListeners.Contains(listener)) {
                return;
            }
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
        listenersToPush.Add(listener);
    }

    public void RemoveListener(InputListener listener) {
        listenersToRemove.Add(listener);
    }

    public void DisableListener(InputListener listener) {
        disabledListeners.Add(listener);
    }

    public void EnableListener(InputListener listener) {
        if (disabledListeners.Contains(listener)) {
            disabledListeners.Remove(listener);
        }
    }

    public IEnumerator AwaitInput() {
        bool advance = false;
        while (advance == false) {
            foreach (KeyCode code in advanceKeys) {
                if (Input.GetKeyDown(code)) {
                    advance = true;
                }
            }
            yield return null;
        }
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
