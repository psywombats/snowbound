using UnityEngine;
using System.Collections;
using System;

public class Utils {
    
    public static IEnumerator RunAfterDelay(float delayInSeconds, Action toRun) {
        yield return new WaitForSeconds(delayInSeconds);
        toRun();
    }

    public static IEnumerator RunParallel(IEnumerator[] coroutines, MonoBehaviour runner) {
        int running = coroutines.Length;
        foreach (IEnumerator coroutine in coroutines) {
            runner.StartCoroutine(RunWithCallack(coroutine, runner, () => {
                running -= 1;
            }));
        }
        while (running > 0) {
            yield return null;
        }
    }

    public static IEnumerator RunWithCallack(IEnumerator coroutine, MonoBehaviour runner, Action toRun) {
        yield return runner.StartCoroutine(coroutine);
        toRun();
    }

    public static void AttachAndCenter(GameObject parent, GameObject child) {
        RectTransform parentTransform = parent.GetComponent<RectTransform>();
        RectTransform childTransform = child.GetComponent<RectTransform>();
        childTransform.SetParent(parentTransform);
        childTransform.anchorMin = new Vector2(0.5f, 0.5f);
        childTransform.anchorMax = childTransform.anchorMin;
        childTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        childTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public static DateTime TimestampToDateTime(double timestamp) {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
        return dtDateTime;
    }

    public static double CurrentTimestamp() {
        return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
