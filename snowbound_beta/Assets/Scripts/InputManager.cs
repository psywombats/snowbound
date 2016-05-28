using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

    public enum Command {
        Advance,
        Menu,
        Skip
    };

    private Dictionary<Command, List<KeyCode>> keybinds;
    private List<KeyCode> fastKeys;

    private List<InputListener> listeners;
    private List<InputListener> disabledListeners;
    private List<InputListener> listenersToPush;
    private List<InputListener> listenersToRemove;

    public void Awake() {
        keybinds = new Dictionary<Command, List<KeyCode>>();
        keybinds[Command.Advance] = new List<KeyCode>(new[] { KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Space, KeyCode.Z });
        keybinds[Command.Menu] = new List<KeyCode>(new[] { KeyCode.Escape, KeyCode.C, KeyCode.Backspace });
        keybinds[Command.Skip] = new List<KeyCode>(new[] { KeyCode.S });
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
            foreach (Command command in System.Enum.GetValues(typeof(Command))) {
                foreach (KeyCode code in keybinds[command]) {
                    if (Input.GetKeyDown(code)) {
                        listener.OnCommand(command);
                    }
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
            foreach (KeyCode code in keybinds[Command.Advance]) {
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
