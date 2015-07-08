using System;
using System.Collections.Generic;
using UnityEngine;

//todo handle inter weapon group linking
//later todo, handle cross weapon group linking

public class WeaponFiringParameters {
    public Entity entity;
    public Turret turret;
    public Vector3[] firepoints;

    public float lastFireTime;
    public float targetAquiredTime;
    public float totalAspectLockTime;
    public float currentAspectLockTime;
    public float longestAspectLockTime;
    public int currentFirepointIndex;

    public float fireRate;
    public float range;
    public float lifetime;
    public float aspectLockTime;
    public float accuracy;
    public float speed;
    public bool firingBeam;

    public bool invertedFireDirection;

    public WeaponFiringParameters(Vector3[] firepoints, Entity entity, Turret turret) {
        this.firepoints = firepoints;
        this.entity = entity;
        this.turret = turret;
        this.currentFirepointIndex = 0;
        this.lastFireTime = 0f;
        this.targetAquiredTime = 0f;
        this.totalAspectLockTime = 0f;
        this.currentAspectLockTime = 0f;
        this.longestAspectLockTime = 0f;
        this.fireRate = -1f;
        this.range = -1f;
        this.lifetime = -1f;
        this.aspectLockTime = -1f;
        this.accuracy = 1f;
        this.firingBeam = false;
        this.speed = 100f;

        if (turret != null) {
            invertedFireDirection = turret.Inverted;
        }
    }

    public Vector3 NextFirePoint {
        get {
            int idx = currentFirepointIndex + 1;
            if (idx == firepoints.Length) idx = 0;
            return firepoints[idx];
        }
    }

    public Vector3 Fire {
        get {
            currentFirepointIndex++;
            lastFireTime = Time.time;
            if (currentFirepointIndex == firepoints.Length) {
                currentFirepointIndex = 0;
            }
            return firepoints[currentFirepointIndex];
        }
    }

    public void UpdateAspectLockTime(IWeapon weapon) {
        if (turret != null && turret.target != null) {

        } else {
            SensorSystem sensors = entity.sensorSystem;
          //  if (!weapon.AspectSeeking || sensors == null) return;
            //if (sensors.TargetInAspectRange(weapon.AspectRange, weapon.AspectFOV)) {
            //    totalAspectLockTime += Time.deltaTime;
            //    currentAspectLockTime += Time.deltaTime;
            //    if (currentAspectLockTime > longestAspectLockTime) {
            //        longestAspectLockTime = currentAspectLockTime;
            //    }
            //} else {
            // currentAspecLockTime = 0f;
            //}
        }
    }

    public void TargetAquired(Entity target) {
        this.totalAspectLockTime = 0f;
        this.currentAspectLockTime = 0f;
        this.longestAspectLockTime = 0f;
        this.targetAquiredTime = Time.time;
    }

}

