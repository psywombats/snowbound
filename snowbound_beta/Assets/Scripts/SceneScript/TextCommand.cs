using UnityEngine;
using System.Collections;
using System;

public abstract class TextCommand : SceneCommand {

    private const float textboxFadeDuration = 0.5f;

    private string text;

    public TextCommand(string text) {
        this.text = text;
    }

    public IEnumerator PerformAction(ScenePlayer parser) {
        TextboxComponent primaryBox = PrimaryBox(parser);
        TextboxComponent secondaryBox = SecondaryBox(parser);

        // fade the textboxes in or out
        if (!primaryBox.gameObject.activeInHierarchy) {
            primaryBox.Alpha = 0.0f;
            primaryBox.Clear();
            primaryBox.gameObject.SetActive(true);
        }
        while (primaryBox.Alpha < 1.0f) {
            primaryBox.Alpha += Time.deltaTime / textboxFadeDuration;
            secondaryBox.Alpha -= Time.deltaTime / textboxFadeDuration;
            if (primaryBox.Alpha > 1.0f) {
                primaryBox.Alpha = 1.0f;
            }
            if (secondaryBox.Alpha < 0.0f) {
                secondaryBox.Alpha = 0.0f;
            }
            if (Global.Instance().inputManager.WasHurried()) {
                primaryBox.Alpha = 1.0f;
                secondaryBox.Alpha = 0.0f;
                break;
            }
            yield return null;
        }
        secondaryBox.gameObject.SetActive(false);

        // type the text
        yield return parser.StartCoroutine(primaryBox.ShowText(text));

        // await input
        yield return Global.Instance().inputManager.AwaitHurry();
    }

    protected abstract TextboxComponent PrimaryBox(ScenePlayer parser);

    protected abstract TextboxComponent SecondaryBox(ScenePlayer parser);
}
