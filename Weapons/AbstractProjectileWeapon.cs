using System;
using UnityEngine;

public abstract class StandardProjectileWeapon : AbstractWeapon {
    public float raycastDistance = 2f;
    public float spawnOffset = 1.5f;
    protected float distanceTravelled = 0f;
    protected Vector3 forward;
    protected float range;
    protected float speed;
    public LayerMask impactLayer;

    public override void Fire(WeaponSpawner spawner, WeaponFiringParameters firingParameters) {
        this.spawner = spawner;
        this.firingParameters = firingParameters;
        this.distanceTravelled = 0f;
        this.range = firingParameters.range;
        this.speed = 100f;
        forward = transform.forward;
        transform.position = transform.position + (forward * spawnOffset);
        transform.rotation *= Quaternion.Euler(UnityEngine.Random.insideUnitSphere * firingParameters.accuracy);

        spawner.SpawnMuzzleFlash(transform.position, transform.rotation);
    }

    public void Update() {
        float speedStep = speed * Time.deltaTime;
        distanceTravelled += speedStep;
        transform.position += (forward * speedStep);

        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position - forward * 2f, forward), out hit, raycastDistance, impactLayer)) {
            spawner.SpawnImpact(hit.point + hit.normal * 0.2f, Quaternion.identity);
            spawner.Despawn(gameObject);
        } else if (distanceTravelled >= range) {
            spawner.Despawn(gameObject);
        }
    }
}
