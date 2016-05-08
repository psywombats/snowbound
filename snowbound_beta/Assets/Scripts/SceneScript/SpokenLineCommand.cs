using UnityEngine;
using System.Collections;
using System;

public class SpokenLineCommand : SceneCommand {

    private const float textboxFadeDuration = 0.5f;

    private string text;

    public SpokenLineCommand(string text) {
        this.text = text;
    }

    public void PerformAction(SceneManagerComponent sceneManager, Action onFinish) {
        sceneManager.StartCoroutine(ActionRoutine(sceneManager, onFinish));
    }

    private IEnumerator ActionRoutine(SceneManagerComponent sceneManager, Action onFinish) {
        TextboxComponent textbox = sceneManager.textbox;
        TextboxComponent paragraph = sceneManager.paragraphBox;

        // fade the textboxes in or out
        if (!textbox.gameObject.activeInHierarchy) {
            textbox.alpha = 0.0f;
            textbox.gameObject.SetActive(true);
        }
        while (textbox.alpha < 1.0f) {
            textbox.alpha += Time.deltaTime / textboxFadeDuration;
            paragraph.alpha -= Time.deltaTime / textboxFadeDuration;
            if (textbox.alpha > 1.0f) {
                textbox.alpha = 1.0f;
            }
            if (paragraph.alpha < 0.0f) {
                paragraph.alpha = 0.0f;
            }
            yield return null;
        }
        paragraph.gameObject.SetActive(false);

        // type the text
        yield return sceneManager.StartCoroutine(textbox.ShowText(text));

        // done
        onFinish();
    }
}
