using UnityEngine;
using System.Collections;

public class Formation : MonoBehaviour {

    public Transform leader;

    void Start() {
        transform.position = leader.position;
    }

    void Update() {
        transform.position = leader.position;
    }
}
