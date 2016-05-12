using UnityEngine;
using System.Collections;

public class TachiComponent : MonoBehaviour {

    private const float spriteFadeTime = 0.5f;

    private CharaData chara;

    public void SetChara(CharaData chara) {
        this.chara = chara;
        GetComponent<SpriteRenderer>().sprite = chara.portrait;
    }

    public bool ContainsChara(CharaData chara) {
        return this.chara.tag.Equals(chara.tag);
    }

    public IEnumerator FadeIn() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.0f);
        gameObject.SetActive(true);
        while (renderer.color.a < 1.0f) {
            float delta = Time.deltaTime / spriteFadeTime;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a + delta);
            yield return null;
        }
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1.0f);
    }

    public IEnumerator FadeOut() {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1.0f);
        while (renderer.color.a > 0.0f) {
            float delta = Time.deltaTime / spriteFadeTime;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a - delta);
            yield return null;
        }
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.0f);
        gameObject.SetActive(false);
        chara = null;
    }
}
