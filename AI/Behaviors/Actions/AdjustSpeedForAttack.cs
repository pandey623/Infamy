using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AdjustSpeedForAttack : Action {

    public AIPilot pilot;
  
    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
    }

    public override TaskStatus OnUpdate() {
        pilot.FlightControls.throttle = 1f;
        return TaskStatus.Success;
    }
}