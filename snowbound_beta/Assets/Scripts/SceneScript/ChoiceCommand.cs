﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

// this is going to be structured a little weirdly as it's a multiline command
// the options are all assigned backwards to the previous choice
// when the [choice] is created, it originally has no options
public class ChoiceCommand : SceneCommand {

    private const string buttonPrefabName = "Prefabs/ChoiceButton";
    private const float buttonSpacingPx = 16.0f;
    private const float buttonFadeDuration = 0.5f;

    private List<ChoiceOption> options;
    private List<GameObject> choiceObjects;

    public ChoiceCommand() {
        options = new List<ChoiceOption>();
    }

    public IEnumerator PerformAction(ScenePlayer player) {

        // set mode
        player.AwaitingInputFromCommand = true;

        // fade out the paragraph box if necessary
        if (player.paragraphBox.gameObject.activeInHierarchy) {
            while(player.paragraphBox.Alpha > 0.0f) {
                player.paragraphBox.Alpha -= Time.deltaTime / buttonFadeDuration;
                yield return null;
            }
        }
        player.paragraphBox.Alpha = 0.0f;
        player.paragraphBox.gameObject.SetActive(false);

        // display the choices
        choiceObjects = new List<GameObject>();
        for (int i = 0; i < options.Count; i += 1) {
            ChoiceOption option = options[i];
            GameObject choiceObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(buttonPrefabName));

            // display/callback
            FormatChoiceObject(choiceObject, option, player);

            // positioning
            RectTransform transform = choiceObject.GetComponent<RectTransform>();
            float buttonHeight = choiceObject.GetComponent<RectTransform>().rect.height;
            float visibleHeight = Screen.height - player.textbox.Height;
            float middleVisibleFraction = (player.textbox.Height + visibleHeight / 2.0f) / Screen.height;

            float totalButtonsHeight = buttonHeight * options.Count;
            totalButtonsHeight += buttonSpacingPx * (options.Count - 1);

            float lowestFraction = middleVisibleFraction - ((totalButtonsHeight - buttonHeight) / 2.0f) / Screen.height;
            float posY = lowestFraction + ((buttonSpacingPx + buttonHeight) / Screen.height) * (options.Count - i - 1);

            Utils.AttachAndCenter(player.canvas.gameObject, choiceObject);
            transform.anchorMin = new Vector2(0.5f, posY);
            transform.anchorMax = transform.anchorMin;
            transform.anchoredPosition = new Vector2(0, 0);
            choiceObjects.Add(choiceObject);
        }

        // fade in the choices
        while (choiceObjects[0].GetComponent<ChoiceButtonComponent>().Alpha < 1.0f) {
            foreach (GameObject choiceObject in choiceObjects) {
                ChoiceButtonComponent choiceComponent = choiceObject.GetComponent<ChoiceButtonComponent>();
                choiceComponent.Alpha = (choiceComponent.Alpha + Time.deltaTime / buttonFadeDuration);
            }
            yield return null;
        }
        foreach (GameObject choiceObject in choiceObjects) {
            choiceObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }
        while (player.AwaitingInputFromCommand) {
            yield return null;
        }
    }

    public void AddOption(ChoiceOption option) {
        options.Add(option);
    }

    private void FormatChoiceObject(GameObject choiceObject, ChoiceOption option, ScenePlayer player) {
        choiceObject.GetComponent<ChoiceButtonComponent>().text.text = option.caption;
        choiceObject.GetComponent<Button>().onClick.AddListener(() => {
            player.StartCoroutine(OnChoiceClickRoutine(player, option));
        });
    }

    private IEnumerator OnChoiceClickRoutine(ScenePlayer player, ChoiceOption option) {

        // state setup
        player.AwaitingInputFromCommand = false;
        foreach (GameObject choiceObject in choiceObjects) {
            choiceObject.GetComponent<Button>().interactable = false;
        }

        // fade out the choices
        while (choiceObjects[0].GetComponent<ChoiceButtonComponent>().Alpha > 0.0f) {
            foreach (GameObject choiceObject in choiceObjects) {
                ChoiceButtonComponent choiceComponent = choiceObject.GetComponent<ChoiceButtonComponent>();
                choiceComponent.Alpha = (choiceComponent.Alpha - Time.deltaTime / buttonFadeDuration);
            }
            yield return null;
        }
        foreach (GameObject choiceObject in choiceObjects) {
            UnityEngine.Object.Destroy(choiceObject);
        }

        // play the next scene
        player.StartCoroutine(player.PlayScriptForScene(option.sceneName));
    }
}
