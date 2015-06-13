using UnityEngine;
using System.Collections.Generic;

public class DotTest : MonoBehaviour {

    public Transform target;

    public float forward;
    public float right;
    public float up;

    void Update() {
        var toTarget = (target.position - transform.position).normalized;

        forward = Vector3.Dot(toTarget, transform.forward);
        right = Vector3.Dot(toTarget, transform.right);
        up = Vector3.Dot(toTarget, transform.up);

        Debug.DrawLine(transform.position, toTarget * 10, Color.red);
    }
}
