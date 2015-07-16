using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindNextWaypoint : Action {

    private AIPilot pilot;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
    }

    public override TaskStatus OnUpdate() {
        if (pilot.waypoints.Length == pilot.waypointIndex) {
            return TaskStatus.Failure;
        } else {
            pilot.goToPosition = pilot.waypoints[pilot.waypointIndex++].position;
            return TaskStatus.Success;
        }
    }
}