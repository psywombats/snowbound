using UnityEngine;
using System.Collections;
using System;

public class Utils {
    
    public static IEnumerator RunAfterDelay(float delayInSeconds, Action toRun) {
        yield return new WaitForSeconds(delayInSeconds);
        toRun();
    }
}
