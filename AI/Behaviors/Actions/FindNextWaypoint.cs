using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindNextWaypoint : Action {

    private AIPilot pilot;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
    }

    public override TaskStatus OnUpdate() {
        pilot.goToPosition = pilot.waypoints[UnityEngine.Random.Range(0, 5)].position;
        return TaskStatus.Success;
    }
}