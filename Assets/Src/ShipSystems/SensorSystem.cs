using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class SensorSystem {

    public Entity Target;
    private Entity entity;
    private Transform transform;
    public DotContainer ToTargetDots;
    public DotContainer FromTargetDots;
    public Vector3 ToTarget;
    public Vector3 FromTarget;
    public Vector3 ToTargetNormalized;
    public Vector3 FromTargetNormalized;
    public float TimeSinceTargetChanged = 0f;
    public float DistanceToTarget = 0f;

    public SensorSystem(Entity entity) {
        this.entity = entity;
        this.transform = entity.transform;
    }

    public List<Entity> GetHostiles() {
        return FactionManager.GetHostile(entity.factionId);
    } 

    public void Update() {
        if (Target != null) {
            TimeSinceTargetChanged += Time.deltaTime;
            ToTargetDots = RightUpForwardDotToEntity(Target);
            FromTargetDots = RightUpForwardDotFromEntity(Target);
            DistanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            ToTarget = (Target.transform.position - transform.position);
            FromTarget = (transform.position - Target.transform.position);
            ToTargetNormalized = Vector3.Normalize(ToTarget);
            FromTargetNormalized = Vector3.Normalize(FromTarget);
        } else {
            ClearTarget();
        }
    }

    public void AquireTarget(Entity target) {
        Assert.IsNotNull(target, "Cannot Aquire a null target");
        this.Target = target;
        TimeSinceTargetChanged = 0f;
        ToTargetDots = RightUpForwardDotToEntity(target);
        FromTargetDots = RightUpForwardDotFromEntity(target);
    }

    public void ClearTarget() {
        ToTargetDots.Clear();
        FromTargetDots.Clear();
        Target = null;
        TimeSinceTargetChanged = 0f;
    }

    public Entity GetNearestHostile() {
        List<Entity> hostiles = FactionManager.GetFaction(entity.factionId).GetHostiles();
        Debug.Log(hostiles.Count);
        float minDistance = float.MaxValue;
        Entity closest = null;

        for (int i = 0; i < hostiles.Count; i++) {
            Vector3 hostilePosition = hostiles[i].transform.position;
            float distance = (hostilePosition - transform.position).sqrMagnitude;
            if (distance < minDistance) {
                closest = hostiles[i];
                minDistance = distance;
            }
        }
        return closest;
    }

    public List<Entity> GetHostilesInRange(float range) {
        range *= range;
        List<Entity> hostiles = FactionManager.GetFaction(entity.factionId).GetHostiles();
        List<Entity> retn = new List<Entity>();
        for (int i = 0; i < hostiles.Count; i++) {
            Vector3 hostilePosition = hostiles[i].transform.position;
            if ((hostilePosition - transform.position).sqrMagnitude < range) {
                retn.Add(hostiles[i]);
            }
        }
        return retn;
    }

    public float ForwardDotToTarget {
        get { return ToTargetDots.forward; }
    }

    public float ForwardDotFromTarget {
        get { return FromTargetDots.forward; }
    }

    public Vector3 TargetPosition {
        get { return (Target == null) ? Vector3.zero : Target.transform.position; }
    }

    public DotContainer RightUpForwardDotFromEntity(Entity entity) {
        Vector3 fromEntity = (transform.position - transform.position).normalized;
        float right = Vector3.Dot(fromEntity, transform.right);
        float up = Vector3.Dot(fromEntity, transform.up);
        float forward = Vector3.Dot(fromEntity, transform.forward);
        return new DotContainer(right, up, forward);
    }

    public DotContainer RightUpForwardDotToEntity(Entity entity) {
        Vector3 toEntity = (entity.transform.position - transform.position).normalized;
        float right = Vector3.Dot(toEntity, transform.right);
        float up = Vector3.Dot(toEntity, transform.up);
        float forward = Vector3.Dot(toEntity, transform.forward);
        return new DotContainer(right, up, forward);
    }

    public DotContainer RightUpForwardDotToVector(Vector3 vec) {
        var toTarget = (vec - transform.position).normalized;
        var right = Vector3.Dot(toTarget, transform.right);
        var up = Vector3.Dot(toTarget, transform.up);
        var forward = Vector3.Dot(toTarget, transform.forward);
        return new DotContainer(right, up, forward);
    }

    public DotContainer RightUpForwardDotFromVector(Vector3 vec) {
        var fromTarget = (transform.position - vec).normalized;
        var right = Vector3.Dot(fromTarget, transform.right);
        var up = Vector3.Dot(fromTarget, transform.up);
        var forward = Vector3.Dot(fromTarget, transform.forward);
        return new DotContainer(right, up, forward);
    }
}
