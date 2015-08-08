using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TargetTooClose : Conditional {

    public AIPilot pilot;
    public SensorSystem sensors;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
        sensors = GetComponent<SensorSystem>();
    }

    public override TaskStatus OnUpdate() {
        if (sensors.DistanceToTarget < 20f) {
            return TaskStatus.Success;
        } else {
            return TaskStatus.Failure;
        }
    }
}