using UnityEngine;
using System.Collections;

public class SteeringVisualizedNode : MonoBehaviour {
    public Color color;
    public float score;
    public Material material;

    void Start() {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update() {
        material.color = color;
    }
}
