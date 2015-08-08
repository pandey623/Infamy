using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindGetAwayPoint : Action {

    public AIPilot pilot;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
    }

    public override void OnStart() {

    }

    public override TaskStatus OnUpdate() {
        return TaskStatus.Success;
    }
}