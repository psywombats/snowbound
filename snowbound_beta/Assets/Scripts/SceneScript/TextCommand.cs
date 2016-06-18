using UnityEngine;
using System.Collections;
using System;

public abstract class TextCommand : SceneCommand {
    
    protected string text;

    public TextCommand(string text) {
        this.text = text;
    }

    public override IEnumerator PerformAction(ScenePlayer player) {
        TextboxComponent primaryBox = PrimaryBox(player);
        TextboxComponent secondaryBox = SecondaryBox(player);
        
        // fade the textboxes in or out
        if (!primaryBox.gameObject.activeInHierarchy) {
            yield return player.StartCoroutine(Utils.RunParallel(new[] {
                primaryBox.Activate(player),
                secondaryBox.Deactivate(player)
            }, player));
        }

        // type the text
        yield return player.StartCoroutine(primaryBox.ShowText(player, text));

        // await input
        yield return player.AwaitHurry();

        // bookkeeping
        Global.Instance().memory.AppendLogItem(new LogItem(text));
    }

    protected abstract TextboxComponent PrimaryBox(ScenePlayer parser);

    protected abstract TextboxComponent SecondaryBox(ScenePlayer parser);
}
