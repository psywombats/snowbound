using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TransitionImageEffect : MonoBehaviour {

    public Shader shader;
    public float transitionDurationSeconds;

    private Texture2D mask;
    private Material material;
    private float elapsedSeconds;
    private bool invert;
    private bool active;

    public void Awake() {
        material = new Material(shader);
    }

    public void OnDestroy() {
        Destroy(material);
    }

    public void Update() {
        if (active) {
            elapsedSeconds += Time.deltaTime;
            if (elapsedSeconds > transitionDurationSeconds) {
                elapsedSeconds = transitionDurationSeconds;
                active = false;
            }
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination) {
        float elapsedRatio = elapsedSeconds / transitionDurationSeconds;
        if (invert) {
            elapsedRatio = 1.0f - elapsedRatio;
        }

        material.SetTexture("_MainTexture", source);
        material.SetTexture("_MaskTexture", mask);
        material.SetFloat("_Elapsed", elapsedRatio);

        Graphics.Blit(source, destination, material);
    }

    public void Transition(Texture2D mask, bool invert = false) {
        this.mask = mask;
        this.invert = invert;
        elapsedSeconds = 0.0f;
        active = true;
    }
}
