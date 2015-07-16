using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GoToVector : Action {

    private AIPilot pilot;
    public SensorSystem sensorSystem;
    public Transform target;
    private FlightControls controls;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
        sensorSystem = GetComponent<SensorSystem>();
        controls = pilot.FlightControls;
    }

    public override void OnStart() {
        controls.destination = pilot.goToPosition;
        controls.throttle = 1f;
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