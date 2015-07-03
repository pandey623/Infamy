using UnityEngine;

public class AIGoal_GoTo : AIGoal {
    public Vector3 destination;
    public float arrivalDistance;
    public bool arrive;

    public AIGoal_GoTo(AIGoalDescriptor descriptor, Vector3 destination, float arrivalDistance, bool arrive = false)
        : base(descriptor) {
        this.destination = destination;
        this.arrivalDistance = arrivalDistance;
        this.arrive = arrive;
        this.aiState = "GoTo"; //todo replace with constant
    }

    public void OnActivate() {
      //  AIState().SetParameter();
    }

    public override AIGoalStatus UpdateGoalProgress() {
        float distanceSqr = (destination - entity.transform.position).sqrMagnitude;
        if (distanceSqr <= arrivalDistance * arrivalDistance) {
            return AIGoalStatus.Completed;
        } else {
            return AIGoalStatus.Pending;
        }
    }
}