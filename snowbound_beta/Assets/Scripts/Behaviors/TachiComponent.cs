﻿using UnityEngine;
using System.Collections;

public class TachiComponent : MonoBehaviour {

    private const float spriteFadeTime = 0.5f;

    private CharaData chara;
    private bool fadingOut, fadingIn;

    public void SetChara(CharaData chara) {
        this.chara = chara;
        GetComponent<SpriteRenderer>().sprite = chara.portrait;
    }

    public bool ContainsChara(CharaData chara) {
        if (this.chara == null || fadingOut) {
            return false;
        } else {
            return this.chara.tag.Equals(chara.tag);
        }
    }

    public IEnumerator FadeIn() {
        fadingIn = true;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.0f);
        gameObject.SetActive(true);
        while (renderer.color.a < 1.0f) {
            float delta = Time.deltaTime / spriteFadeTime;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a + delta);
            yield return null;
        }
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1.0f);
        fadingIn = false;
    }

    public IEnumerator FadeOut() {
        fadingOut = true;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1.0f);
        while (renderer.color.a > 0.0f) {
            if (fadingIn) {
                break;
            }
            float delta = Time.deltaTime / spriteFadeTime;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a - delta);
            yield return null;
        }
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.0f);
        gameObject.SetActive(false);
        chara = null;
        fadingOut = false;
    }

    public TachiMemory ToMemory() {
        TachiMemory memory = new TachiMemory();
        if (gameObject.activeSelf && chara != null) {
            memory.charaTag = chara.tag;
            memory.enabled = true;
        } else {
            memory.charaTag = null;
            memory.enabled = false;
        }
        return memory;
    }
}