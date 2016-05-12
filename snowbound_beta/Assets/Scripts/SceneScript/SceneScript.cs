﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class SceneScript {
    
    private List<SceneCommand> commands;
    private ChoiceCommand choice;

    public SceneScript(TextAsset asset) {
        ParseCommands(asset.text);
    }

    public IEnumerator PerformActions(ScenePlayer parser) {
        foreach (SceneCommand command in commands) {
            yield return parser.StartCoroutine(command.PerformAction(parser));
        }
    }
    
    private void ParseCommands(string text) {
        choice = null;
        commands = new List<SceneCommand>();
        string[] commandStrings = text.Split(new [] { "\r\n", "\n" }, StringSplitOptions.None);
        bool startsNewParagraph = true;
        foreach (string commandString in commandStrings) {
            SceneCommand command;

            if (commandString.Trim().Length == 0) {
                // newline, no command but we need to keep track
                
                startsNewParagraph = true;
                continue;
            } else if (commandString[0] == '[') {
                // this is a command of some type

                if (commandString.IndexOf(']') == commandString.Length - 1) {
                    // infix command
                    if (commandString.IndexOf(' ') == -1) {
                        // single word command
                        command = ParseCommand(commandString.Substring(1, commandString.Length - 2), new List<string>());
                    } else {
                        // multiword infix command
                        string keyword = commandString.Substring(1, commandString.IndexOf(' ') - 1);
                        string argsString = commandString.Substring(commandString.IndexOf(' ') + 1,
                                commandString.Length - (keyword.Length + 3));
                        string[] args = argsString.Split();
                        command = ParseCommand(keyword, new List<string>(args));
                    }
                } else {
                    // postfix command
                    string keyword = commandString.Substring(1, commandString.Length - 2);
                    string argsString = commandString.Substring(commandString.IndexOf(']'));
                    string[] args = argsString.Split();
                    command = ParseCommand(keyword, new List<string>(args));
                }
            } else {
                // this is a text literal

                if (StartsWithName(commandString)) {
                    startsNewParagraph = false;
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
        switch (command) {
            case "goto":
                return new GotoCommand(args[0]);
            case "choice":
                this.choice = new ChoiceCommand();
                return this.choice;
            case "enter":
                return new EnterCommand(args[0], args[1]);
            case "exit":
                return new ExitCommand(args[0]);
            default:
                if (choice != null) {
                    string choiceString = command + " " + String.Join(" ", args.ToArray());
                    this.choice.AddOption(new ChoiceOption(choiceString));
                }
                //Assert.IsTrue(false, "bad command: " + command);
                return null;
        }
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
