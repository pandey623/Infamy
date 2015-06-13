using UnityEngine;
using System.Collections.Generic;

public abstract class AISubstate {
    public string name;
    protected AIState parentState;
    protected AIPilot pilot;
    protected Entity entity;
    protected Transform transform;
    protected SensorSystem sensorSystem;
    protected EngineSystem engineSystem;
    protected WeaponSystem weaponSystem;
    protected CommunicationSystem commSystem;
    protected NavigationSystem navSystem;

    public AISubstate(AIState parentState) {

        this.parentState = parentState;
        this.pilot = parentState.pilot;
        this.entity = parentState.pilot.entity;
        this.transform = entity.transform;
        this.sensorSystem = entity.sensorSystem;
        this.engineSystem = entity.engineSystem;
        this.weaponSystem = entity.weaponSystem;
        this.commSystem = entity.commSystem;
        this.navSystem = entity.navSystem;
    }

    public void Enter() {
        OnEnter();

    }

    public SubstateStatus Update() {
        return OnUpdate();
    }

    public void Exit() {
        OnExit();
    }

    public abstract SubstateStatus OnUpdate();
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
//
//    public float ActiveTime {
//        get { return activeTimer.ElapsedTime; }
//    }
//
//    public float InActiveTime {
//        get { return inactiveTimer.ElapsedTime; }
//    }
}