using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TurnTowards : Action {

    private AIPilot pilot;

    public override void OnStart() {
        pilot = GetComponent<AIPilot>();
    }

    public override TaskStatus OnUpdate() {

        pilot.entity.engineSystem.FlightControls.SetThrottle(1f);
        if (Vector3.Distance(pilot.transform.position, pilot.FlightControls.destination) < 5f) {
            return TaskStatus.Success;
        } else {
            return TaskStatus.Running;
        }
    }
}