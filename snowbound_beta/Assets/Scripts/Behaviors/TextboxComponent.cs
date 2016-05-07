using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TextboxComponent : MonoBehaviour {

    private const float characterDelay = (1 / 32f);

    public Image backer;
    public Text textbox;

    private string fullText;
    private Action textCompleteDelegate;

    public void Start() {
        
    }

    public void Awake() {
        StartCoroutine(Utils.RunAfterDelay(0.5f, () => {
            ShowText("It was the best of of best of the times of the gthe game ger this is a typing test speed which I'm going to take immeidately afte this betterrrrrrr span more than one line");
        }));
    }

    public void ShowText(string text, Action textCompleteDelegate=null) {
        fullText = text;
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText() {
        for (int i = 0; i <= fullText.Length; i += 1) {
            textbox.text = fullText.Substring(0, i);
            textbox.text += "<color=#00000000>";
            textbox.text += fullText.Substring(i);
            textbox.text += "</color>";
            yield return new WaitForSeconds(characterDelay);
        }
        if (this.textCompleteDelegate != null) {
            Action toCall = this.textCompleteDelegate;
            this.textCompleteDelegate = null;
            toCall();
        }
    }
}
