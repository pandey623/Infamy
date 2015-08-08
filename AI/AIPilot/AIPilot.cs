using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Assert = UnityEngine.Assertions.Assert;

public class AIPilot : MonoBehaviour {

    public Transform[] waypoints;
    public Vector3 goToPosition;
    public bool arriveAtPosition;
    public float goToDistanceThreshold;

    [HideInInspector]
    public Entity entity;

    private EngineSystem engines;
    private WeaponSystem weaponSystem;
    private SensorSystem sensorSystem;
    private CommunicationSystem commSystem;
    private NavigationSystem navSystem;
    private FlightControls controls;

    public void Start() {
        this.entity = GetComponent<Entity>();
        this.engines = GetComponentInChildren<EngineSystem>();
        this.controls = engines.FlightControls;
        this.sensorSystem = entity.sensorSystem;
        this.weaponSystem = entity.weaponSystem;
        this.commSystem = entity.commSystem;
        this.navSystem = entity.navSystem;
        Assert.IsNotNull(this.entity, "AIPilot needs an entity " + transform.name);
        Assert.IsNotNull(this.engines, "AIPilot needs an engine system");
        Assert.IsNotNull(this.sensorSystem, "AIPilot needs a sensor system");
    }

    public void SetGoTo(Vector3 goTo, bool arrive = false) {
        this.goToPosition = goTo;
        this.arriveAtPosition = arrive;
    }

    public FlightControls FlightControls {
        get { return controls; }
    }
 
}

