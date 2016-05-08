using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneScript {
    
    private List<SceneCommand> commands;

    public SceneScript(TextAsset asset) {
        ParseCommands(asset.text);
    }

    public IEnumerator PerformActions(SceneParser parser) {
        foreach (SceneCommand command in commands) {
            yield return parser.StartCoroutine(command.PerformAction(parser));
        }
    }
    
    private void ParseCommands(string text) {
        commands = new List<SceneCommand>();
        string[] commandStrings = text.Split(new [] { "\r\n", "\n" }, StringSplitOptions.None);
        bool startsNewParagraph = false;
        foreach (string commandString in commandStrings) {
            SceneCommand command;

            if (commandString.Trim().Length == 0) {
                // newline, no command but we need to keep track
                
                startsNewParagraph = true;
                continue;
            } else if (commandString[0] == '[') {
                // this is a command of some type

                if (commandString.IndexOf(']') == commandString.Length - 1) {
                    // single word command
                    command = ParseCommand(commandString.Substring(1, commandString.Length - 2), new List<string>());
                } else {
                    string keyword = commandString.Substring(1, commandString.IndexOf(' '));
                    string argsString = commandString.Substring(commandString.IndexOf(' '), commandString.Length - 2);
                    string[] args = argsString.Split();
                    command = ParseCommand(keyword, new List<string>(args));
                }
            } else {
                // this is a text literal

                if (StartsWithName(commandString)) {
                    command = new SpokenLineCommand(commandString);
                } else {
                    if (startsNewParagraph) {
                        command = new ParagraphCommand(commandString);
                    } else {
                        // the inner monologue
                        command = new SpokenLineCommand(commandString);
                    }
                }
            }

            if (command != null) {
                commands.Add(command);
            }
            startsNewParagraph = false;
        }
    }

    private SceneCommand ParseCommand(string command, List<string> args) {
        return null;
    }

    private bool StartsWithName(string text) {
        if ((text.IndexOf(' ') == -1) || (text.IndexOf(':') == -1)) {
            return false;
        }
        foreach (char c in text) {
            if (c == ':') {
                return true;
            }
            if (!Char.IsUpper(c)) {
                return false;
            }
        }
        return false;
    }
}
