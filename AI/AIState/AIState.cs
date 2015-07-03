using UnityEngine;
using System.Collections.Generic;

public abstract class AIState {
    public AIGoal activeGoal; // set externally when entering / exiting. 
    public string name;
    public AIPilot pilot;
    protected float startTime;
    protected float substateStartTime;
    protected AISubstate activeSubstate;
    protected AISubstate[] substates;
    protected Transform transform;
    protected Entity entity;

    protected SensorSystem sensorSystem;
    protected EngineSystem engineSystem;
    protected WeaponSystem weaponSystem;
    protected CommunicationSystem commSystem;
    protected NavigationSystem navSystem;

    public AIState(string name, AIPilot pilot) {
        this.name = name;
        this.pilot = pilot;
        this.entity = pilot.entity;
        this.transform = entity.transform;
        this.sensorSystem = entity.sensorSystem;
        this.engineSystem = entity.engineSystem;
        this.weaponSystem = entity.weaponSystem;
        this.commSystem = entity.commSystem;
        this.navSystem = entity.navSystem;
    }

    public virtual void OnEnter() { }

    public virtual void OnExit() { }

    public virtual void OnEnterSubstate() { }

    public virtual void OnExitSubstate() { }

    public virtual void Update() { }

    public virtual void OnDamageTaken() { }

    public virtual void OnCollision(Entity other) { }

    public virtual void OnCollisionPossible(Entity other) { }

    public virtual void OnIncomingWarhead(Entity other) { }

    public virtual void ManageShields() { }

    public virtual bool CanEvade() {
        return true;
    }

    public void SetSubstate(string substateName) {
        if (activeSubstate != null) {
            activeSubstate.OnExit();
        }

        for (int i = 0; i < substates.Length; i++) {
            if (substates[i].name == substateName) {
                activeSubstate.Enter();
                return;
            }
        }
        throw new System.Exception("Invalid substate selected " + substateName);
    }

    public void SetGoal(AIGoal goal) {
        this.activeGoal = goal;
        if (activeSubstate != null) {
         //   activeSubstate.OnGoalChanged(goal);
        }
    }

    protected virtual void UpdateActiveSubstate() {
//        if (activeGoal.aiSubstate != null) return;
//        AISubstate oldState = activeSubstate;
//        AISubstate newState = activeSubstate;
//
//        float maxScore = -1f;
//        for (int i = 0; i < substates.Length; i++) {
//            float score = substates[i].GetScore();
//            if (score > maxScore) {
//                newState = substates[i];
//                maxScore = score;
//            }
//        }
//
//        //todo give a slight bias to currently active state if it returns a score > 0.75
//
//        if (oldState != newState) {
//            if (oldState != null) oldState.OnExit();
//            newState.OnEnter(activeGoal);
//        }
    }
}

public class AIStates {
    public static string Attack = "Attack";
    public static string Idle = "Idle";

    public static string[] StateList = {
        Attack, Idle
    };
}