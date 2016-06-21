using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(FadingUIComponent))]
public class QuickMenuComponent : MonoBehaviour {

    public Button menuButton;
    public Button saveButton;
    public Button loadButton;
    public Button logButton;
    public Button skipButton;

    public void Awake() {
        FormatButtonForCommand(menuButton, InputManager.Command.Menu);
        FormatButtonForCommand(saveButton, InputManager.Command.Save);
        FormatButtonForCommand(loadButton, InputManager.Command.Load);
        FormatButtonForCommand(logButton, InputManager.Command.Log);
        FormatButtonForCommand(skipButton, InputManager.Command.Skip);
    }

    private void FormatButtonForCommand(Button button, InputManager.Command command) {
        button.onClick.AddListener(() => {
            Global.Instance().input.SimulateCommand(command);
        });
    }

    public IEnumerator FadeInRoutine(float durationSeconds) {
        yield return StartCoroutine(GetComponent<FadingUIComponent>().FadeInRoutine(durationSeconds));
    }

    public IEnumerator FadeOutRoutine(float durationSeconds) {
        yield return StartCoroutine(GetComponent<FadingUIComponent>().FadeOutRoutine(durationSeconds));
    }

    public IEnumerator Activate(ScenePlayer player) {
        yield return player.StartCoroutine(GetComponent<FadingUIComponent>().Activate(player));
    }

    public IEnumerator Deactivate(ScenePlayer player) {
        yield return player.StartCoroutine(GetComponent<FadingUIComponent>().Deactivate(player));
    }
}
