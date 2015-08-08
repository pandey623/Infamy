using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class StopEngines : Action {

    private AIPilot pilot;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
    }

    public override void OnStart() {
        pilot.FlightControls.throttle = 0f;
        pilot.goToDistanceThreshold = 5f;
    }

    public override TaskStatus OnUpdate() {
        pilot.goToPosition = pilot.transform.forward * 100f;
        if (GetComponent<Rigidbody>().velocity.magnitude == 0) {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}