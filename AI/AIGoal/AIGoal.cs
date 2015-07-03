using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class AIGoal : IComparable {
    public AIGoalStatus status;
    public AIGoalType type;
    public string aiState;    //Attack
    public string aiSubstate; //Strafe
    public float priority;
    public float tenacityMultiplier; // < 0 means not interruptable
    public Entity entity;
//   public AIGoalPrioritizer prioritizer;       //might be a string lookup
//   public AIGoalTargetSelector targetSelector; //might be a string lookup

    //todo introduce AIGoalStatus.Valid|Invalid.
    //use case would be target list but some have not entered battle yet
    public AIGoal(AIGoalDescriptor descriptor) {
        this.status = descriptor.status;
        this.type = descriptor.type;
        this.priority = descriptor.priority;
        this.tenacityMultiplier = descriptor.tenacityMultiplier;
        if (this.status == AIGoalStatus.Pending) {
            OnActivate();
        }
    }

    public virtual void Prioritize() { }
    public virtual void OnActivate() { }
    public virtual void OnDeactivate() { }
    public abstract AIGoalStatus UpdateGoalProgress();

    public int CompareTo(object obj){
 	    AIGoal other = obj as AIGoal;
        if (other.priority < priority) return -1;
        if(other.priority > priority) return 1;
        return 0;
    }

    public bool Interruptible {
        get { return tenacityMultiplier < 0f; }
    }
}

//Each of these probably has a semi custom inspector and lives on a scriptable object or in a JSON file.
//new AIDestroyShipsGoal([ShipNames], priority = 0.5f, TargetSelector = null, Prioritizer = null)
//new AIDestroy_N_ShipsGoal()
//new AIDestroySubsystemGoal()
//new AIDestroySubsystemTypeGoal()
//new AIDisableGoal()

