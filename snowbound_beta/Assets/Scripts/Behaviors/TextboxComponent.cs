﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class TextboxComponent : MonoBehaviour {

    private const float characterDelay = (1 / 32f);

    public Image backer;
    public Text textbox;
    private string fullText;
    
    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    public float height {
        get { return GetComponent<RectTransform>().rect.height; }
    }

    public IEnumerator ShowText(string text) {
        fullText = text;
        for (int i = 0; i <= fullText.Length; i += 1) {
            if (Global.Instance().input.WasHurried()) {
                Global.Instance().input.AcknowledgeHurried();
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
