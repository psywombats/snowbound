using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class TextboxComponent : MonoBehaviour {

    private const float characterDelay = (1 / 32f);

    public Image backer;
    public Text textbox;

    private string fullText;
    private Action textCompleteDelegate;

    public void Start() {

    }

    public void Awake() {
        ShowText("It was the best of of best of the times of the gthe game manager this is a typing test speed which I'm going to take immeidately afte this better span more than one line", null);
    }

    public void ShowText(string text, Action textCompleteDelegate) {
        this.fullText = text;
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText() {
        for (int i = 0; i < fullText.Length; i += 1) {
            textbox.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(characterDelay);
        }
        if (this.textCompleteDelegate != null) {
            Action toCall = this.textCompleteDelegate;
            this.textCompleteDelegate = null;
            toCall();
        }
    }
}
