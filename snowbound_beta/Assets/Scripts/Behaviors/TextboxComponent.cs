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
    
    public float alpha {
        get { return gameObject.GetComponent<CanvasRenderer>().GetAlpha(); }
        set { gameObject.GetComponent<CanvasRenderer>().SetAlpha(value); }
    }

    public IEnumerator ShowText(string text) {
        fullText = text;
        for (int i = 0; i <= fullText.Length; i += 1) {
            if (Global.Instance().inputManager.WasHurried()) {
                Global.Instance().inputManager.AcknowledgeHurried();
                textbox.text = fullText;
                break;
            }
            textbox.text = fullText.Substring(0, i);
            textbox.text += "<color=#00000000>";
            textbox.text += fullText.Substring(i);
            textbox.text += "</color>";
            yield return new WaitForSeconds(characterDelay);
        }
    }

    public void Clear() {
        textbox.text = "";
    }
}
