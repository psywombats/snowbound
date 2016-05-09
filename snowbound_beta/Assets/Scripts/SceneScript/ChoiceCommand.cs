using UnityEngine;
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

    public ChoiceCommand() {
        options = new List<ChoiceOption>();
    }

    public IEnumerator PerformAction(ScenePlayer player) {

        // fade out the paragraph box if necessary
        if (player.paragraphBox.gameObject.activeInHierarchy) {
            while(player.paragraphBox.alpha > 0.0f) {
                player.paragraphBox.alpha -= Time.deltaTime / buttonFadeDuration;
                yield return null;
            }
        }
        player.paragraphBox.alpha = 0.0f;
        player.paragraphBox.gameObject.SetActive(false);

        // display the choices
        List<GameObject> choiceObjects = new List<GameObject>();
        for (int i = 0; i < options.Count; i += 1) {
            ChoiceOption choice = options[i];
            GameObject choiceObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(buttonPrefabName));
            RectTransform transform = choiceObject.GetComponent<RectTransform>();

            float buttonHeight = choiceObject.GetComponent<RectTransform>().rect.height;
            float visibleHeight = Screen.height - player.textbox.height;
            float middleVisibleFraction = (player.textbox.height + visibleHeight / 2.0f) / Screen.height;

            float totalButtonsHeight = buttonHeight * options.Count;
            totalButtonsHeight += buttonSpacingPx * (options.Count - 1);

            float lowestFraction = middleVisibleFraction - ((totalButtonsHeight - buttonHeight) / 2.0f) / Screen.height;
            float posY = lowestFraction + ((buttonSpacingPx + buttonHeight) / Screen.height) * i;
            
            choiceObject.transform.SetParent(player.canvas.transform);
            transform.anchorMin = new Vector2(0.5f, posY);
            transform.anchorMax = transform.anchorMin;
            transform.anchoredPosition = new Vector2(0, 0);
            choiceObject.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
            choiceObjects.Add(choiceObject);
        }

        // fade in the choices
        while (choiceObjects[0].GetComponent<CanvasRenderer>().GetAlpha() < 1.0f) {
            foreach (GameObject choiceObject in choiceObjects) {
                CanvasRenderer renderer = choiceObject.GetComponent<CanvasRenderer>();
                renderer.SetAlpha(renderer.GetAlpha() + Time.deltaTime / buttonFadeDuration);
            }
            yield return null;
        }
        foreach (GameObject choiceObject in choiceObjects) {
            choiceObject.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
        }
    }

    public void AddOption(ChoiceOption option) {
        options.Add(option);
    }
}
