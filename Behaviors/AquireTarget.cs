using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AquireTarget : Action {
    
    private AIPilot pilot;
    public SensorSystem sensorSystem;
    public Transform target;

    public override void OnStart() {
        pilot = GetComponent<AIPilot>();
        sensorSystem = GetComponent<SensorSystem>();
    }

    public override TaskStatus OnUpdate() {
        pilot.FlightControls.destination = target.position;
        return TaskStatus.Success;
    }
}