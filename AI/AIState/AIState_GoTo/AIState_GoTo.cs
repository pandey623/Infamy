using UnityEngine;
using System.Collections.Generic;

public class AIState_GoTo : AIState {
    private Vector3 destination;
    private FlightControls controls;
    private bool arrive;

    public AIState_GoTo(AIPilot pilot)
        : base("GoTo", pilot) {
        controls = pilot.FlightControls;
    }

    public override void OnEnter() {
        destination = (pilot.activeGoal as AIGoal_GoTo).destination;
        arrive = (pilot.activeGoal as AIGoal_GoTo).arrive;
    }

    public string entityToArriveAt = null;

    public override void Update() {
       // if(entityToArriveAt != null) controls.GoTo();
        controls.GoTo(destination, 1f);
    }
}