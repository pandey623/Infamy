using System.Collections.Generic;
using UnityEngine;

public enum SubstateStatus {
    Running, Completed, Invalid
}

class AISubstate_GetSpace : AISubstate {
    private Timer timer;
    private Timer rollTimer;
    private float desiredDistance;
    private float rollOverride;

    public AISubstate_GetSpace(AIState parentState)
        : base(parentState) {
        timer = new Timer();
        rollTimer = new Timer(3f);
    }

    public override void OnEnter() {
        Reset();
    }

    private void Reset() {
        timer.Reset(Random.Range(6f, 10f));
        rollTimer.Reset(Random.Range(1, 4));
        desiredDistance = Random.Range(200f, 500f) + (sensorSystem.DistanceToTarget * Random.Range(0.75f, 1.5f));
        Debug.Log("Getting Distance, " + desiredDistance);
        rollOverride = 0f;
    }

    public override SubstateStatus OnUpdate() {
        float distance = sensorSystem.DistanceToTarget;
        Entity target = sensorSystem.Target;
        Vector3 escapeDirection;
        //need a better 'is mobile / moving' check

        if ((int) target.size < (int) EntitySize.Large) {
            escapeDirection = Vector3.zero;
        } else {
            escapeDirection = sensorSystem.FromTargetNormalized * 1000f;
        }
        if (timer.Ready || distance >= desiredDistance) {
            return SubstateStatus.Completed;
        }

        if (rollTimer.ReadyWithReset(Random.Range(1, 4))) {
            float random = Random.Range(1, 5);
            if (random <= 1) {
                rollOverride = Random.Range(-65f, 65f);
            } else {
                rollOverride = 0f;
            }
        }

        pilot.FlightControls.GoTo(escapeDirection, 1f, rollOverride);
        return SubstateStatus.Running;
    }

}

