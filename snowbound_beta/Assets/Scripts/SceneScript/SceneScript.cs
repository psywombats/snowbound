using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class SceneScript {
    
    private List<SceneCommand> commands;
    private ChoiceCommand choice;
    private StageDirectionCommand lastStageDirection;
    private BranchCommand lastBranch;
    private ExitAllCommand lastExitAll;
    private bool holdMode;
    private bool nvlMode;

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
                // newline, this is a command to clear the stage of characters presuming no HOLDs

                if (!holdMode) {
                    this.lastExitAll = new ExitAllCommand();
                    command = this.lastExitAll;
                    if (lastStageDirection != null) {
                        lastStageDirection.SetSynchronous();
                    }
                } else {
                    command = null;
                }
                holdMode = false;
                startsNewParagraph = true;
            } else if (commandString[0] == '[') {
                // this is a command of some type

                startsNewParagraph = false;
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
                    // spoken line
                    command = ParseLine(commandString);
                } else {
                    if (startsNewParagraph) {
                        // text paragraph
                        command = ParseParagraph(commandString);
                    } else {
                        // the inner monologue
                        command = ParseLine(commandString);
                    }
                }
                startsNewParagraph = false;
                if (lastStageDirection != null) {
                    lastStageDirection.SetSynchronous();
                }
            }

            if (command != null) {
                commands.Add(command);
            }
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
                this.lastStageDirection = new EnterCommand(args[0], args[1]);
                return this.lastStageDirection;
            case "exit":
                this.lastStageDirection = new ExitCommand(args[0]);
                return this.lastStageDirection;
            case "clear":
                this.lastStageDirection = new ExitAllCommand();
                return this.lastStageDirection;
            case "hold":
                // don't bother an explicit command here, this is really just a meta-command about parsing
                holdMode = true;
                return null;
            case "increment":
                return new IncrementCommand(args[0], 1);
            case "decrement":
                return new IncrementCommand(args[0], -1);
            case "branch":
                this.lastBranch = new BranchCommand(args[0], args[1], args[2]);
                return this.lastBranch;
            case "true":
                this.lastBranch.TrueSceneName = args[1];
                return null;
            case "false":
                this.lastBranch.FalseSceneName = args[1];
                return null;
            default:
                if (choice != null) {
                    string choiceString = command + " " + String.Join(" ", args.ToArray());
                    this.choice.AddOption(new ChoiceOption(choiceString));
                }
                //Assert.IsTrue(false, "bad command: " + command);
                return null;
        }
    }

    private SceneCommand ParseLine(string commandString) {
        if (nvlMode) {
            if (lastExitAll != null) {
                lastExitAll.ClosesTextboxes = true;
                lastExitAll = null;
            }
        }
        nvlMode = false;
        return new SpokenLineCommand(commandString);
    }

    private SceneCommand ParseParagraph(string commandString) {
        if (!nvlMode) {
            if (lastExitAll != null) {
                lastExitAll.ClosesTextboxes = true;
                lastExitAll = null;
            }
        }
        nvlMode = true;
        return new ParagraphCommand(commandString);
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
