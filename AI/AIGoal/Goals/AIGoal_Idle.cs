using UnityEngine;

public class AIGoal_Idle : AIGoal {

    public AIGoal_Idle()
        : base(ConstrutorOverride()) {
        this.aiState = AIStates.Idle;
    }

    public static AIGoalDescriptor ConstrutorOverride() {
        return new AIGoalDescriptor(-0.99f);
    }

    public override AIGoalStatus UpdateGoalProgress() {
        return AIGoalStatus.Pending;
    }

}
