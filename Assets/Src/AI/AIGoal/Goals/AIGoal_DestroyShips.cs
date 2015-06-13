using UnityEngine;
using System.Collections.Generic;

public class AIGoal_DestroyShips : AIGoal {
    public string[] shipNames;
    public List<string> remainingShipNames;

    //todo -- maybe this should be DestroyEntities to be more generic
    public AIGoal_DestroyShips(AIGoalDescriptor descriptor, string[] shipNames)
        : base(descriptor) {
        this.aiState = AIStates.Attack;
        this.shipNames = shipNames;
        this.remainingShipNames = new List<string>(shipNames);
    }

    public override AIGoalStatus UpdateGoalProgress() {
        for (int i = 0; i < remainingShipNames.Count; i++) {
            //Infamy.MissionLog.IsShipDestroyed(remainingShipNames[i])
        }
        return AIGoalStatus.Pending;
    }

    public void SetTargetPrioritizer() {
        //take a function that can be called to pick new targets
    }

}
