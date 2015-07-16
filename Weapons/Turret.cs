using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

    public Transform target; // todo this should be an entity or subsystem

    public bool locked;
    [HideInInspector]
    public bool multiPart;
    public string weaponId;
    public string turretGroupId;
    public float rotationSpeed = 45f;
    public float loftSpeed = 45f;
    [HideInInspector]
    public Transform barrel;
    [HideInInspector]
    public Vector3 turretNormal;
    [HideInInspector]
    public Vector3 barrelNormal;

    public Firepoint[] firepoints;
    private float normalSign;

    private WeaponSpawner spawner;
    private WeaponFiringParameters firingParameters;
    private Entity entity;
    private int currentFirepointIndex;

    void Awake() {
        currentFirepointIndex = 0;
        turretNormal = Vector3.Cross(turretNormal, transform.right);
        normalSign = (Vector3.Dot(turretNormal, transform.forward) < 0) ? 1 : -1;
        multiPart = barrel != null;
        if (!multiPart) {
            Vector3 toFirepoint = (firepoints[0].transform.position - transform.position).normalized;
            firepoints[0].transform.rotation = Quaternion.LookRotation(toFirepoint);
        }
        //turret firepoints are created at import time
    }

    void Start() {
        spawner = WeaponSpawner.Get(weaponId);
        entity = GetComponentInParent<Entity>();
        firingParameters = new WeaponFiringParameters(entity);
    }

    public void AddFirepoint(Firepoint firepoint) {
        //firepoints.Add(firepoint);
    }

    public void AlignTo(Vector3 position) {
        if(!multiPart) return;
        Vector3 turretAxis = transform.rotation * turretNormal; //same as transform.TransformDirection(turretNormal)
        Vector3 localTarget = transform.InverseTransformPoint(position);
        Vector3 projectedTarget = Vector3.ProjectOnPlane(localTarget, transform.up);

        float toAlignment = -normalSign * Mathf.Atan2(localTarget.x * normalSign, localTarget.y * normalSign) * Mathf.Rad2Deg;
        float turn = rotationSpeed * Time.deltaTime;
        float diff = Mathf.Clamp(toAlignment, -turn, turn);
        transform.Rotate(turretAxis, diff, Space.World);

        if (barrel == null) return;

        Vector3 barrelAxis = Vector3.Cross(barrel.up, barrel.rotation * barrelNormal);
        Vector3 barrelLocalTarget = barrel.InverseTransformPoint(position);
        Vector3 barrelProjectedTarget = Vector3.ProjectOnPlane(barrelLocalTarget, barrelAxis);

        float toBarrelAlignment = -normalSign * Mathf.Atan2(barrelLocalTarget.y * normalSign, barrelLocalTarget.z * normalSign) * Mathf.Rad2Deg;
        float barrelTurn = loftSpeed * Time.deltaTime;
        float barrelDiff = Mathf.Clamp(toBarrelAlignment, -barrelTurn, barrelTurn);
        barrel.Rotate(barrelAxis, barrelDiff, Space.World);

        float fdot = Vector3.Dot(barrel.forward, transform.up);
        float udot = Vector3.Dot(barrel.forward, transform.forward);

        if (udot < 0) {
            barrel.localRotation = Quaternion.AngleAxis(90f, Vector3.left);
        } else if (fdot < 0) {
            barrel.localRotation = Quaternion.identity;
        }
    }

    void Update() {
        //Debug.DrawLine(transform.position, transform.position + (transform.TransformDirection(turretNormal) * 3f), Color.red);
        //todo implement control modes so ai can take direct control
        //todo implement predictive firing
        if (target == null || firingParameters == null || locked) return;
        AlignTo(target.position); //todo return dot of barrel to location / or 1 or 0 for button in fov
        var toFirepoit = firepoints[0].transform.position - transform.position;
        Fire();
    }

    //todo might need to special case some weapon types
    public bool Fire() {
        if (firepoints.Length == 0 || spawner == null || !spawner.CanFire(firingParameters)) return false;
        if (multiPart) {
            Vector3 toTarget = target.position - barrel.position;
            if (Vector3.Dot(barrel.forward * normalSign, toTarget.normalized) > 0.98f) { //todo replace 0.98f with an fov value
                firingParameters.hardpointTransform = firepoints[currentFirepointIndex].transform;
                currentFirepointIndex = (currentFirepointIndex + 1) % firepoints.Length;
                IWeapon weapon = spawner.Spawn(firingParameters);
                firingParameters.lastFireTime = Time.time;
            }
        } else {
            Vector3 toTarget = (target.position - transform.position).normalized;
            Vector3 toFirepoint = (firepoints[0].transform.position - transform.position).normalized;
            //Debug.Log(Vector3.Dot(toTarget, toFirepoint));
            if (Vector3.Dot(toTarget, toFirepoint) > 0.25f) { 
                firingParameters.hardpointTransform = firepoints[currentFirepointIndex].transform;
                firingParameters.hardpointTransform.rotation = Quaternion.LookRotation(toTarget);
                currentFirepointIndex = (currentFirepointIndex + 1) % firepoints.Length;
                IWeapon weapon = spawner.Spawn(firingParameters);
                firingParameters.lastFireTime = Time.time;
            }
        }
        return true;
    }

    public bool Inverted {
        get { return normalSign < 0; }
    }
}