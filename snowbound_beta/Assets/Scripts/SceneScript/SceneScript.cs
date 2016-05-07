using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneScript {

    private TextAsset asset;
    private List<SceneCommand> commands;

    public SceneScript(TextAsset asset) {
        this.asset = asset;
        parseCommands(asset.text);
    }

    public void performActions(SceneManagerComponent sceneManager, Action onFinish) {
        if (commands.Count == 0) {
            onFinish();
        } else {
            SceneCommand command = commands[0];
            commands.Remove(command);
            command.performAction(sceneManager, () => {
                performActions(sceneManager, onFinish);
            });
        }
    }
    
    private void parseCommands(string text) {
        commands = new List<SceneCommand>();
        string[] commandStrings = text.Split(new [] { '\n' });
        foreach (string commandString in commandStrings) {
            if (commandString.Trim().Length == 0) {
                continue;
            }
            if (commandString[0] == '[') {

            } else {
                commands.Add(new BetaTextCommand(commandString));
            }
        }
    }
}
