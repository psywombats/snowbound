using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuComponent : MonoBehaviour {

    

    public Button startButton;
    public Button loadButton;
    public Button quitButton;

    private FadeComponent fade;
    private bool interactive;

    public void Awake() {
        interactive = true;
        fade = FindObjectOfType<FadeComponent>();

        startButton.onClick.AddListener(() => {
            if (!interactive) {
                return;
            }
            interactive = false;
            StartCoroutine(StartRoutine());
        });
        loadButton.onClick.AddListener(() => {
            if (!interactive) {
                return;
            }
            interactive = false;
            StartCoroutine(LoadRoutine());
        });
        quitButton.onClick.AddListener(() => {
            if (!interactive) {
                return;
            }
            interactive = false;
            StartCoroutine(QuitRoutine());
        });
    }

    private IEnumerator StartRoutine() {
        yield return fade.FadeToBlackRoutine();
        ScenePlayer.LoadScreen();
    }

    private IEnumerator LoadRoutine() {
        GameObject loadMenuObject = SaveMenuComponent.Spawn(gameObject.transform.parent.gameObject, null, SaveMenuComponent.SaveMenuMode.Load);
        loadMenuObject.GetComponent<SaveMenuComponent>().Alpha = 0.0f;
        yield return StartCoroutine(loadMenuObject.GetComponent<SaveMenuComponent>().FadeIn());
    }

    private IEnumerator QuitRoutine() {
        yield return fade.FadeToBlackRoutine();
        Application.Quit();
    }
}
