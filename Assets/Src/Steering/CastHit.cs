using UnityEngine;
using System.Collections;

public class CastHit {
    public Vector3 point;
    public Vector3 direction;
    public float squareDistance;
    public Color color;
    public bool primary = false;

    public CastHit(Vector3 point, Vector3 direction, float squareDistance, Color color, bool primary = false) {
        this.point = point;
        this.direction = direction;
        this.squareDistance = squareDistance;
        this.color = color;
        this.primary = primary;
    }
}