using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour, IWeapon {
    public float raycastDistance = 2f; //todo make static and read off base weapon controller for laser
    public float range;
    public float damage;
    public float speed;
    public float spawnOffset = 1.5f;
    private float distanceTravelled = 0f;
    public LayerMask impactLayer;

    [HideInInspector]
    private LaserController controller;

    public void Fire(Transform origin, Transform target) {
        transform.position = origin.position + (origin.forward * spawnOffset);
        transform.rotation = origin.rotation;
        this.distanceTravelled = 0f;
    }

    public void Fire(Transform origin, Transform target, LaserController controller) {
        this.controller = controller;
        Fire(origin, target);
    }

    public void Update() {
        float speedStep = speed * Time.deltaTime;
        distanceTravelled += speedStep;
        transform.position += (transform.forward * speedStep);

        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position - transform.forward * 2f, transform.forward), out hit, raycastDistance, impactLayer)) {
            controller.Impact(this, hit);
        }
        if (distanceTravelled >= range) {
            controller.Despawn(this);
        }
    }

    public float Range {
        get { return range; }
    }
}
