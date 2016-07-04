using UnityEngine;
using System.Collections;
using System;

public class TransitionComponent : MonoBehaviour {

    public Shader shader;

    private FadeData currentFade;
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
            if (GetComponent<Camera>() == null) {
                AssignCommonShaderVariables();
            }
            elapsedSeconds += Time.deltaTime;
            if (elapsedSeconds > currentFade.delay) {
                elapsedSeconds = currentFade.delay;
                active = false;
            }
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (GetComponent<Camera>() != null) {
            material.SetTexture("_MainTexture", source);
            AssignCommonShaderVariables();
            Graphics.Blit(source, destination, material);
        }
    }

    public void Clear() {
        active = false;
        elapsedSeconds = 0.0f;
        invert = false;
        AssignCommonShaderVariables();
    }

    public void InstantFade() {
        active = false;
        elapsedSeconds = 1.0f;
        invert = false;
        AssignCommonShaderVariables();
    }

    public Material GetMaterial() {
        return material;
    }

    public bool IsTransitioning() {
        return active;
    }

    public void Hurry() {
        if (this.elapsedSeconds > 0.0f) {
            this.elapsedSeconds = currentFade.delay;
        }
        AssignCommonShaderVariables();
        active = false;
    }

    public IEnumerator TransitionRoutine(TransitionData transition, Action intermediate = null) {
        yield return StartCoroutine(FadeRoutine(transition.fadeOut));
        if (intermediate != null) {
            intermediate();
        }
        yield return StartCoroutine(FadeRoutine(transition.fadeIn, true));
    }

    public IEnumerator FadeRoutine(FadeData fade, bool invert = false) {
        this.currentFade = fade;
        this.invert = invert;
        elapsedSeconds = 0.0f;
        active = true;

        ScenePlayer player = FindObjectOfType<ScenePlayer>();
        while (elapsedSeconds < currentFade.delay) {
            if (player.ShouldUseFastMode()) {
                break;
            }
            yield return null;
        }
        AssignCommonShaderVariables();
    }

    private void AssignCommonShaderVariables() {
        if (currentFade != null) {
            material.SetTexture("_MaskTexture", currentFade.transitionMask);
            material.SetFloat("_Elapsed", elapsedSeconds / currentFade.delay);
            material.SetFloat("_SoftFudge", currentFade.softEdgePercent);
            material.SetInt("_Invert", invert ? 1 : 0);
        }
    }
}
