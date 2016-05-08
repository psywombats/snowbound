using UnityEngine;
using System.Collections;
using System;

public class ParagraphCommand : SceneCommand {

    private const float textboxFadeDuration = 0.5f;

    private string text;

    public ParagraphCommand(string text) {
        this.text = text;
    }

    public void PerformAction(SceneManagerComponent sceneManager, Action onFinish) {
        sceneManager.StartCoroutine(ActionRoutine(sceneManager, onFinish));
    }

    private IEnumerator ActionRoutine(SceneManagerComponent sceneManager, Action onFinish) {
        TextboxComponent textbox = sceneManager.textbox;
        TextboxComponent paragraph = sceneManager.paragraphBox;

        // fade the textboxes in or out
        if (!paragraph.gameObject.activeInHierarchy) {
            paragraph.alpha = 0.0f;
            paragraph.gameObject.SetActive(true);
        }
        while (paragraph.alpha < 1.0f) {
            textbox.alpha -= Time.deltaTime / textboxFadeDuration;
            paragraph.alpha += Time.deltaTime / textboxFadeDuration;
            if (textbox.alpha < 0.0f) {
                textbox.alpha = 0.0f;
            }
            if (paragraph.alpha > 1.0f) {
                paragraph.alpha = 1.0f;
            }
            yield return null;
        }
        textbox.gameObject.SetActive(false);

        // type the text
        yield return sceneManager.StartCoroutine(paragraph.ShowText(text));

        // done
        onFinish();
    }
}
