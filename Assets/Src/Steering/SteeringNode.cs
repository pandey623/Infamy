using UnityEngine;
using System.Collections;

public class SteeringNode {
    public int index;
    public float score;
    public int x;
    public int y;
    public string name;

    public SteeringNode(int index, int x, int y) {
        this.index = index;
        this.x = x;
        this.y = y;
        this.name = "x: " + x + ", y: " + y;
    }   
}

