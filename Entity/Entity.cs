using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[SelectionBase] //todo ensure some kind of collider exists, maybe create it but dont use require component if possilbe
public partial class Entity : MonoBehaviour {
    public FactionId factionId;
    public EntitySize size = EntitySize.Small;
    public EntityType type = EntityType.Fighter;
    public string variation;

    [HideInInspector]
    public WeaponSystem weaponSystem;
    [HideInInspector]
    public SensorSystem sensorSystem;
    [HideInInspector]
    public CommunicationSystem commSystem;
    [HideInInspector]
    public NavigationSystem navSystem;
    [HideInInspector]
    public EngineSystem engineSystem;
    [HideInInspector]
    public TurretSystem turretSystem;

    public void Awake() {
        IntersectionModule_Intialize();
        weaponSystem = GetComponentInChildren<WeaponSystem>();
        sensorSystem = GetComponentInChildren<SensorSystem>();
        engineSystem = GetComponentInChildren<EngineSystem>();
        turretSystem = GetComponentInChildren<TurretSystem>();

        if (gameObject.layer != LayerMask.NameToLayer("Entity")) {
            gameObject.layer = LayerMask.NameToLayer("Entity");
        }
    }

    void Start() {
        EntityDatabase.Add(this);
        FactionManager.AddEntity(this);
    }

    public void Update() {
        IntersectionModule_Update();
    }
}
