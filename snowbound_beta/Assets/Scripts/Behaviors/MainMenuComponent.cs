using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuComponent : MonoBehaviour {

    

    public Button startButton;
    public Button loadButton;
    public Button quitButton;

    private FadeComponent fade;

    public void Awake() {
        fade = FindObjectOfType<FadeComponent>();

        startButton.onClick.AddListener(() => {
            SetButtonsEnabled(false);
            StartCoroutine(StartRoutine());
        });
        loadButton.onClick.AddListener(() => {
            SetButtonsEnabled(false);
            StartCoroutine(LoadRoutine());
        });
        quitButton.onClick.AddListener(() => {
            SetButtonsEnabled(false);
            StartCoroutine(QuitRoutine());
        });
    }

    private void SetButtonsEnabled(bool enabled) {
        startButton.interactable = enabled;
        loadButton.interactable = enabled;
        quitButton.interactable = enabled;
    }

    private IEnumerator StartRoutine() {

        yield return fade.FadeToBlackRoutine();
        ScenePlayer.LoadScreen();
    }

    private IEnumerator LoadRoutine() {
        GameObject loadMenuObject = SaveMenuComponent.Spawn(gameObject.transform.parent.gameObject, SaveMenuComponent.SaveMenuMode.Load, () => {
            SetButtonsEnabled(false);
        });
        loadMenuObject.GetComponent<SaveMenuComponent>().Alpha = 0.0f;
        yield return StartCoroutine(loadMenuObject.GetComponent<SaveMenuComponent>().FadeIn());
    }

    private IEnumerator QuitRoutine() {
        yield return fade.FadeToBlackRoutine();
        Application.Quit();
    }
}
