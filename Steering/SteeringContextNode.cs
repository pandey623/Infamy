using UnityEngine;
using System.Collections.Generic;

public class SteeringContextNode : MonoBehaviour {
    public Vector2 index;
    public float score;
    public List<string> contributions = new List<string>();
    public List<SteeringContextNode> neighbors = new List<SteeringContextNode>();
    public Color color;
    public Vector3 direction;

    void Start() {
        Reset();
    }

    void Update() {
        GetComponent<MeshRenderer>().material.color = color;
    }

    public void Reset() {
        contributions.Clear();
        score = 0f;
        color = Color.white;
    }
    //todo for next time -- find vectors to danger, saturate them. give bonuses to opposite nodes
    public void Saturate(string contributor, float strength) {
        contributions.Add(contributor + "(" + strength + ")");
        score += strength;
    }
}
