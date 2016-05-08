using UnityEngine;
using System.Collections;

public class SceneParser : MonoBehaviour {

    public TextAsset firstSceneFile;
    public TextboxComponent textbox;
    public TextboxComponent paragraphBox;

    public void Start() {
        textbox.gameObject.SetActive(false);
        paragraphBox.gameObject.SetActive(false);
        SceneScript firstScript = new SceneScript(firstSceneFile);
        StartCoroutine(firstScript.PerformActions(this));
    }
}
