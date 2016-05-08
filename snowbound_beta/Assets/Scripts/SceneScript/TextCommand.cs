using UnityEngine;
using System.Collections;
using System;

public abstract class TextCommand : SceneCommand {

    private const float textboxFadeDuration = 0.5f;

    private string text;

    public TextCommand(string text) {
        this.text = text;
    }

    public void PerformAction(SceneParser parser, Action onFinish) {
        parser.StartCoroutine(TextRoutine(parser, onFinish));
    }

    private IEnumerator TextRoutine(SceneParser parser, Action onFinish) {
        TextboxComponent primaryBox = PrimaryBox(parser);
        TextboxComponent secondaryBox = SecondaryBox(parser);

        // fade the textboxes in or out
        if (!primaryBox.gameObject.activeInHierarchy) {
            primaryBox.alpha = 0.0f;
            primaryBox.Clear();
            primaryBox.gameObject.SetActive(true);
        }
        while (primaryBox.alpha < 1.0f) {
            primaryBox.alpha += Time.deltaTime / textboxFadeDuration;
            secondaryBox.alpha -= Time.deltaTime / textboxFadeDuration;
            if (primaryBox.alpha > 1.0f) {
                primaryBox.alpha = 1.0f;
            }
            if (secondaryBox.alpha < 0.0f) {
                secondaryBox.alpha = 0.0f;
            }
            if (Global.Instance().inputManager.WasHurried()) {
                primaryBox.alpha = 1.0f;
                secondaryBox.alpha = 0.0f;
                break;
            }
            yield return null;
        }
        secondaryBox.gameObject.SetActive(false);

        // type the text
        yield return parser.StartCoroutine(primaryBox.ShowText(text));

        // await input
        yield return Global.Instance().inputManager.AwaitHurry();

        // done
        onFinish();
    }

    protected abstract TextboxComponent PrimaryBox(SceneParser parser);

    protected abstract TextboxComponent SecondaryBox(SceneParser parser);
}
