using UnityEngine;
using System.Collections.Generic;

public class AIState_Idle : AIState {
    private FlightControls controls;

    public AIState_Idle(AIPilot pilot) : base("Idle", pilot) {
        this.controls = pilot.FlightControls;
    }

    public override void Update() {
        controls.SetStickInputs(0f, 0f, 0f);
        controls.SetThrottle(0f);
    }

}
