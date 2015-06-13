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

    public WeaponSystem weaponSystem;
    public SensorSystem sensorSystem;
    public CommunicationSystem commSystem;
    public NavigationSystem navSystem;
    public EngineSystem engineSystem;

    [HideInInspector] public float radius;

    public void Awake() {
        EntityDatabase.Add(this);
        FactionManager.AddEntity(this);
        IntersectionModule_Intialize();
        Transform systems = transform.FindChild("Systems");
        if (systems) {
            weaponSystem = new WeaponSystem(this);
            sensorSystem = new SensorSystem(this);
            commSystem = new CommunicationSystem(this);
            navSystem = new NavigationSystem(this);
            engineSystem = GetComponent<EngineSystem>();
        }

        if (gameObject.layer != LayerMask.NameToLayer("Entity")) {
            gameObject.layer = LayerMask.NameToLayer("Entity");
        }
        //load initializer by path if present otherwise look up default by entity type
        //otherwise load generic default
    }

    public void Update() {
        IntersectionModule_Update();
    }

    public void ApplyDamage(float damage, RaycastHit hitData) {
        //if we have a 'DamageInterpreter' component delegate to that, else just subtract hull/sheilds as nessessary
    }

}
