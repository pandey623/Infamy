using UnityEngine;
using System.Collections;

public class HelloWorld : MissionScript {
    private int val = 0;

    void Start() {
        Every(1f).Limit(3).Unless(False).Then(SomeAction);
        WhileTrigger t =  While(True);
        t.SetInterval(1f);
    }

    public void SomeAction() {
        val++;
        Debug.Log(val);
    }

    public void ScaleUp() {
        transform.localScale *= 5f;
    }
 }