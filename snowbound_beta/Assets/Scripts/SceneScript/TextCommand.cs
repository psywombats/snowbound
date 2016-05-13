using UnityEngine;
using System.Collections;
using System;

public abstract class TextCommand : SceneCommand {

    private string text;

    public TextCommand(string text) {
        this.text = text;
    }

    public IEnumerator PerformAction(ScenePlayer player) {
        TextboxComponent primaryBox = PrimaryBox(player);
        TextboxComponent secondaryBox = SecondaryBox(player);

        // fade the textboxes in or out
        yield return player.StartCoroutine(Utils.RunParallel(new[] {
                primaryBox.FadeIn(),
                secondaryBox.FadeOut()
        }, player));

        // type the text
        yield return player.StartCoroutine(primaryBox.ShowText(text));

        // await input
        yield return Global.Instance().input.AwaitHurry();
    }

    protected abstract TextboxComponent PrimaryBox(ScenePlayer parser);

    protected abstract TextboxComponent SecondaryBox(ScenePlayer parser);
}
