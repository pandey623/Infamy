using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GoToVector : Action {

    private AIPilot pilot;
    public SensorSystem sensorSystem;
    public Transform target;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
        sensorSystem = GetComponent<SensorSystem>();
    }

    public override void OnStart() {
        pilot.FlightControls.destination = pilot.goToPosition;
        pilot.FlightControls.throttle = 1f;
        pilot.goToDistanceThreshold = 5f;
    }

    public override TaskStatus OnUpdate() {
        if (Vector3.Distance(pilot.goToPosition, pilot.transform.position) < pilot.goToDistanceThreshold) {
            return TaskStatus.Success;
        } else {
            return TaskStatus.Running;
        }
    }
}