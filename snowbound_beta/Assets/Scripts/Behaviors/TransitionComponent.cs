using UnityEngine;
using System.Collections;

public class TransitionComponent : MonoBehaviour {

    public Shader shader;
    public float transitionDurationSeconds;
    public float softTransitionPercent;

    public float out1, out2;

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
            if (GetComponent<Camera>() == null) {
                AssignCommonShaderVariables();
            }
            elapsedSeconds += Time.deltaTime;
            if (elapsedSeconds > transitionDurationSeconds) {
                elapsedSeconds = transitionDurationSeconds;
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

    public Material GetMaterial() {
        return material;
    }

    public bool IsTransitioning() {
        return active;
    }

    public void Hurry() {
        if (this.elapsedSeconds > 0.0f) {
            this.elapsedSeconds = transitionDurationSeconds;
        }
        AssignCommonShaderVariables();
        active = false;
    }

    public void Transition(Texture2D mask, bool invert = false) {
        this.mask = mask;
        this.invert = invert;
        elapsedSeconds = 0.0f;
        active = true;
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
        AssignCommonShaderVariables();
    }

    private void AssignCommonShaderVariables() {
        material.SetTexture("_MaskTexture", mask);
        material.SetFloat("_Elapsed", elapsedSeconds / transitionDurationSeconds);
        material.SetFloat("_SoftFudge", softTransitionPercent);
        material.SetInt("_Invert", invert ? 1 : 0);
        out1 = elapsedSeconds / transitionDurationSeconds;
    }
}
