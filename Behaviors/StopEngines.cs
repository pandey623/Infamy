using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class StopEngines : Action {

    private AIPilot pilot;
    private FlightControls controls;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
        controls = pilot.FlightControls;
    }

    public override void OnStart() {
        controls.throttle = 0f;
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