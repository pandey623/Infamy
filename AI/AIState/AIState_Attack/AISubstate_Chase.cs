using System.Collections.Generic;
using UnityEngine;

class AISubstate_Chase : AISubstate {

    public AISubstate_Chase(AIState parentState)
        : base(parentState) { }

    public override SubstateStatus OnUpdate() {
        if (sensorSystem.DistanceToTarget <= 30f) {
            return SubstateStatus.Invalid;
        }
        pilot.FlightControls.GoTo(sensorSystem.TargetPosition, 1f);
        return SubstateStatus.Running;
    }
}