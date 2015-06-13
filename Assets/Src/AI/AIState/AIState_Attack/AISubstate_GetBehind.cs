using System;
using System.Collections.Generic;
using UnityEngine;

class AISubstate_GetBehind : AISubstate {

    public AISubstate_GetBehind(AIState parentState)
        : base(parentState) {

    }

    public override SubstateStatus OnUpdate() {
        return SubstateStatus.Running;
    }

    public float GetScore() {
        if ((int)sensorSystem.Target.size > (int)EntitySize.Medium) return 0f;

        float dotToTarget = sensorSystem.ForwardDotToTarget;
        float dotFromTarget = sensorSystem.ForwardDotFromTarget;
        if (dotToTarget > 0.2f && (dotFromTarget > -0.2f && dotFromTarget < 0.1f)) {
            return 1f;
        }

        return 0f;
    }
}

