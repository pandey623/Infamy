using UnityEngine;
using System.Collections.Generic;

public class CastHitTracker {
    public Entity entity;
    public List<CastHit> hits;
    public int lifeTime = 0;

    public CastHitTracker(Entity e) {
        this.entity = e;
        this.hits = new List<CastHit>();
    }

    public bool UpdateLifetime(int totalLifetime) {
        return (++lifeTime) >= totalLifetime;
    }

    public void AddHitPoint(Vector3 point, Vector3 direction, float squareDistance) {
        this.hits.Add(new CastHit(point, direction, squareDistance, Color.white));
    }
}