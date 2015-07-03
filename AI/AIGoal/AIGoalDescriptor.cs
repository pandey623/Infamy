using UnityEngine;
using System.Collections.Generic;

public struct AIGoalDescriptor {
    public float priority;
    public float tenacityMultiplier;
    public AIGoalType type;
    public AIGoalStatus status;

    public AIGoalDescriptor(float priority, float tenacityMultiplier = 1.0f, AIGoalType type = AIGoalType.Scripted, AIGoalStatus status = AIGoalStatus.Pending) {
        this.priority = priority;
        this.tenacityMultiplier = tenacityMultiplier;
        this.type = type;
        this.status = status;
    }
}
