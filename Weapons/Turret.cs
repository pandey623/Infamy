using UnityEngine;
using System.Collections.Generic;

public class Turret : MonoBehaviour {

    public Transform target; // todo this should be an entity or subsystem

    public bool locked;
    [HideInInspector]
    public bool multiPart;
    public string weaponId;
    public string weaponVariant;
    public string turretGroupId;
    public float rotationSpeed = 45f;
    public float loftSpeed = 45f;
    [HideInInspector]
    public Transform barrel;
    [HideInInspector]
    public Vector3 turretNormal;
    [HideInInspector]
    public Vector3 barrelNormal;
    [HideInInspector]
    public Vector3[] firepoints;
    private float normalSign;

    private WeaponSpawner weaponSpawner;
    private WeaponFiringParameters firingParameters;
    private Entity entity;

    void Awake() {
        turretNormal = Vector3.Cross(turretNormal, transform.right);
        normalSign = (Vector3.Dot(turretNormal, transform.forward) < 0) ? 1 : -1;
        multiPart = barrel != null;
        if (Inverted && barrel != null) {
            for (int i = 0; i < firepoints.Length; i++) {
                Vector3 fp = firepoints[i];
                fp.z = -fp.z;
                firepoints[i] = fp;
            }
        }
    }

    void Start() {
        entity = GetComponentInParent<Entity>();
        if (entity.turretSystem) {
            SetTurretGroup(entity.turretSystem.GetGroup(turretGroupId));
        }
    }

    public void AlignTo(Vector3 position) {
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
        //todo implement control modes so ai can take direct control
        if (target == null || firingParameters == null || locked) return;
        AlignTo(target.position); //todo return dot of barrel to location / or 1 or 0 for button in fov
        TryToFire();
    }

    public void SetTurretGroup(TurretGroup group) {
        weaponSpawner = WeaponSpawner.Get(group.weaponId);
        if(firingParameters == null) firingParameters = new WeaponFiringParameters(firepoints, entity, this);
        WeaponData data = group.weaponData;
        firingParameters.range = data.range;
        firingParameters.lifetime = data.lifeTime;
        firingParameters.fireRate = data.fireRate;
        firingParameters.aspectLockTime = data.aspectTime;
        firingParameters.accuracy = data.accuracy;
        firingParameters.speed = data.speed;
    }

    //todo might need to special case some weapon types
    public void TryToFire() {
        if (weaponSpawner == null) return;
        if (weaponSpawner.CanFire(firingParameters)) {
            if (multiPart) {
                Vector3 toTarget = target.position - barrel.position;
                if (Vector3.Dot(barrel.forward * normalSign, toTarget.normalized) > 0.98f) { //todo replace 0.98f with an fov value
                    Quaternion rotation = Quaternion.LookRotation(barrel.forward * normalSign, barrel.up);
                    IWeapon weapon = weaponSpawner.Spawn(firingParameters, barrel.TransformPoint(firingParameters.Fire), rotation);
                    //if (weapon.Type == WeaponType.SweepBeam) {

                    //}
                }
            } else {
                Vector3 toTarget = target.position - transform.position;
                //todo -- get button turret normals 
                //if (Vector3.Dot(barrel.forward * normalSign, toTarget.normalized) > 0.98f) {
                //    Quaternion offset = Quaternion.Euler(UnityEngine.Random.insideUnitSphere);
                //    weaponSpawner.Spawn(firingParameters, barrel.TransformPoint(firingParameters.Fire), offset * barrel.rotation);
                //}
            }
        }
    }

    public bool Inverted {
        get { return normalSign < 0; }
    }
}