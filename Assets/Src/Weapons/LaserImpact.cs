using UnityEngine;
using System.Collections;

public class LaserImpact : MonoBehaviour {
    public float duration;
    private float startTime;
    private LaserController controller;


    public void Spawn(LaserController controller, Vector3 location) {
        startTime = Time.realtimeSinceStartup;
        this.controller = controller;
        transform.position = location;
    }

    public void Update() {
        if (Time.realtimeSinceStartup - startTime > duration) {
            controller.DespawnImpact(this);
        }
    }
}
