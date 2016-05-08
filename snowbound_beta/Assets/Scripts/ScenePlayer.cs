using UnityEngine;
using System.Collections;

public class ScenePlayer : MonoBehaviour {

    private const string scenesDirectory = "SceneScripts";

    public TextAsset firstSceneFile;
    public TextboxComponent textbox;
    public TextboxComponent paragraphBox;

    public void Start() {
        textbox.gameObject.SetActive(false);
        paragraphBox.gameObject.SetActive(false);
        StartCoroutine(PlayScriptForScene(firstSceneFile));
    }

    public IEnumerator PlayScriptForScene(string sceneName) {
        TextAsset file = Resources.Load<TextAsset>(scenesDirectory + "/" + sceneName);
        yield return StartCoroutine(PlayScriptForScene(file));
    }

    public IEnumerator PlayScriptForScene(TextAsset sceneFile) {
        SceneScript script = new SceneScript(sceneFile);
        yield return StartCoroutine(script.PerformActions(this));
    }
}
