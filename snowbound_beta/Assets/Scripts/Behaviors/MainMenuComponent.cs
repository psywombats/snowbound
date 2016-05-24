using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuComponent : MonoBehaviour {

    public Button startButton;
    public Button loadButton;
    public Button quitButton;

    private FadeComponent fade;

    public void Awake() {
        fade = FindObjectOfType<FadeComponent>();

        startButton.onClick.AddListener(() => {

        });
        loadButton.onClick.AddListener(() => {

        });
        quitButton.onClick.AddListener(() => {

        });
    }

    private IEnumerator StartRoutine() {
        yield return null;
    }

    private IEnumerator LoadRoutine() {
        GameObject loadMenuObject = SaveMenuComponent.Spawn(null, SaveMenuComponent.SaveMenuMode.Load);
        loadMenuObject.GetComponent<SaveMenuComponent>().Alpha = 0.0f;
        yield return StartCoroutine(loadMenuObject.GetComponent<SaveMenuComponent>().FadeIn());
    }

    private IEnumerator QuitRoutine() {
        yield return fade.FadeOutRoutine();
        Application.Quit();
    }
}
