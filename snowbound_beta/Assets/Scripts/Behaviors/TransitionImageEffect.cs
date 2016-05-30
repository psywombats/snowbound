using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class TransitionImageEffect : MonoBehaviour {

    public Shader shader;
    public float transitionDurationSeconds;
    public float softTransitionPercent;

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
        
        material.SetTexture("_MainTexture", source);
        material.SetTexture("_MaskTexture", mask);
        material.SetFloat("_Elapsed", elapsedRatio);
        material.SetFloat("_SoftFudge", softTransitionPercent);
        material.SetInt("_Invert", invert ? 1 : 0);

        Graphics.Blit(source, destination, material);
    }

    public IEnumerator TransitionRoutine(Texture2D mask, bool invert = false) {
        Transition(mask, invert);
        ScenePlayer player = FindObjectOfType<ScenePlayer>();
        while (elapsedSeconds < transitionDurationSeconds) {
            if (player.ShouldUseFastMode()) {
                break;
            }
            yield return null;
        }
    }

    public void Transition(Texture2D mask, bool invert = false) {
        this.mask = mask;
        this.invert = invert;
        elapsedSeconds = 0.0f;
        active = true;
    }
}
