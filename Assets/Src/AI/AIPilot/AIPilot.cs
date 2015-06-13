using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Assert = UnityEngine.Assertions.Assert;

//this is only for small craft, larger craft will need a more custom pilot
public class AIPilot : MonoBehaviour {

    private AIState[] aiStates;
    private FlightControls controls;

    public List<AIGoal> goals;
    public AIGoal activeGoal;
    public AIState activeState;
    [HideInInspector]
    public Entity entity;

    public float tenacity = 0.5f;
    public float aggression = 0.5f;
    public float persistence = 1f;

    private EngineSystem engines;
    private WeaponSystem weaponSystem;
    private SensorSystem sensorSystem;
    private CommunicationSystem commSystem;
    private NavigationSystem navSystem;

    //todo -- probably takes a descriptor to read pilot stats out of
    public void Start () {
        this.entity = GetComponent<Entity>();
        this.engines = GetComponentInChildren<EngineSystem>();
        Assert.IsNotNull(this.entity, "AIPilot needs an entity " + transform.name);
        Assert.IsNotNull(this.engines, "AIPilot needs an engine system");
        this.controls = engines.FlightControls;
        this.sensorSystem = entity.sensorSystem;
        this.weaponSystem = entity.weaponSystem;
        this.commSystem = entity.commSystem;
        this.navSystem = entity.navSystem;

        goals = new List<AIGoal>(5);
        AddGoal(new AIGoal_Idle());

        AIGoalDescriptor desc = new AIGoalDescriptor(0.5f);
        AddGoal(new AIGoal_GoTo(desc, new Vector3(0, 0, 42f), 5f));


        aiStates = CreateAIStates(this);

        SetAIState();
    }

    void OnCollisionEnter(Collision c) {
        Debug.Log("Collided");
    }

    public void OnDrawGizmos() {
        if (sensorSystem != null && sensorSystem.Target != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(sensorSystem.TargetPosition, new Vector3(4, 4, 4));
        }

        if (controls != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(controls.destination, Vector3.one);
            Gizmos.color = Color.cyan;
        }

    }

    public void Update() {
        weaponSystem.Update();
        sensorSystem.Update();
        //maybe some sort of initial 'should be evasive' query here
      //  activeState.Update();
      //  UpdateActiveGoals();
    }

    public void AddGoal(AIGoal goal) {
        goal.entity = entity;
        goals.Add(goal);
        goal.Prioritize();
        goals.Sort();
        if (goals[0] != activeGoal) {
            activeGoal = goals[0];
            SetAIState();
        }
    }

    public void ActivateGoal(AIGoal goal) {
        if (goal.status == AIGoalStatus.Inactive) {
            goal.OnActivate();
        }
    }

    public void DeactivateGoal(AIGoal goal) {
        if (goal.status == AIGoalStatus.Pending) {
            goal.OnDeactivate();
        }
    }

    public void AddDynamicGoal(AIGoal goal) {
        goal.type = AIGoalType.Dynamic;
        AddGoal(goal);
    }

    public void RemoveGoal(AIGoal goal) {
        goal.priority = -1f;
        if (activeGoal == goal) {
            activeGoal = goals[0];
            SetAIState();
        }
    }

    private void UpdateActiveGoals() {
        for (int i = 0; i < goals.Count; i++) {
            AIGoal goal = goals[i];
            if (goal.status != AIGoalStatus.Pending) continue;
            if (goal.UpdateGoalProgress() == AIGoalStatus.Pending) {
                goal.Prioritize();
            } else {
                goal.priority = -1f;
            }
        }
        goals.Sort();
        if (goals[0] != activeGoal) {
            activeGoal = goals[0];
            SetAIState();
        }
    }

    private void SetAIState() {
        if (aiStates == null) return;
        if (activeState != null) activeState.OnExit();
        for (int i = 0; i < aiStates.Length; i++) {
            if (aiStates[i].name == activeGoal.aiState) {
                activeState = aiStates[i];
                activeState.OnEnter();
                return;
            }
        }
        throw new System.Exception("Invalid AIState " + activeGoal.aiState);
    }

    public Vector3 PredictedTargetPosition {
        get { return Vector3.zero; }
    }

    public FlightControls FlightControls {
        get { return controls; }
    }

    public bool CanSelectNewTarget {
        get { return sensorSystem.TimeSinceTargetChanged >= persistence * 30f; }
    }

    private static AIState[] CreateAIStates(AIPilot pilot) {
        return new AIState[] {
            new AIState_Attack(pilot),
            new AIState_GoTo(pilot),
            new AIState_Idle(pilot), 
        };
    }
}