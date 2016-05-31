using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogComponent : MonoBehaviour, InputListener {

    private const string PrefabName = "Prefabs/Log";
    private const float FadeoutSeconds = 0.5f;

    public Text textbox;
    public ScrollRect scroll;

    public float Alpha {
        get { return gameObject.GetComponent<CanvasGroup>().alpha; }
        set { gameObject.GetComponent<CanvasGroup>().alpha = value; }
    }

    private Action onFinish;

    public void Start() {
        PopulateLog(Global.Instance().memory.GetMessageHistory());
        Global.Instance().input.PushListener(this);
    }

    public static GameObject Spawn(GameObject parent, Action onFinish) {
        GameObject logObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(PrefabName));
        logObject.GetComponent<LogComponent>().onFinish = onFinish;
        Utils.AttachAndCenter(parent, logObject);
        return logObject;
    }

    public void OnCommand(InputManager.Command command) {
        switch (command) {
            case InputManager.Command.Menu:
            case InputManager.Command.Rightclick:
                StartCoroutine(ResumeRoutine());
                break;
            default:
                break;
        }
    }

    public IEnumerator FadeIn() {
        while (Alpha < 1.0f) {
            Alpha += Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        Alpha = 1.0f;
        Global.Instance().input.EnableListener(this);
    }

    public IEnumerator FadeOut() {
        Global.Instance().input.DisableListener(this);
        while (Alpha > 0.0f) {
            Alpha -= Time.deltaTime / FadeoutSeconds;
            yield return null;
        }
        Alpha = 0.0f;
    }


    public void PopulateLog(List<LogItem> log) {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        foreach (LogItem logItem in log) {
            builder.AppendLine(logItem.FormatLogLine());
        }
        textbox.text = builder.ToString();
        scroll.normalizedPosition = new Vector2(0, 0);
    }

    private IEnumerator ResumeRoutine() {
        yield return StartCoroutine(FadeOut());
        Global.Instance().input.RemoveListener(this);
        if (onFinish != null) {
            onFinish();
        }
        Destroy(gameObject);
    }
}
