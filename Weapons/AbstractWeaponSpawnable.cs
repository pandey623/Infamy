using System;
using UnityEngine;

public abstract class AbstractWeaponSpawnable : MonoBehaviour {
    [HideInInspector]
    public float spawnTime;
    [HideInInspector]
    public int poolIndex;
    [HideInInspector]
    public WeaponSpawner spawner;
    [HideInInspector]
    public WeaponSpawnerType type;
    [HideInInspector]
    public Transform parent;

    public virtual void Spawn(WeaponSpawner spawner, WeaponSpawnerType type, int poolIndex, Vector3 position, Quaternion rotation, Transform parent = null) {
        spawnTime = Time.time;
        this.poolIndex = poolIndex;
        this.spawner = spawner;
        this.type = type;
        this.parent = parent;
        this.transform.position = position;
        this.transform.rotation = rotation;
    }

    public virtual void Spawn(WeaponSpawner spawner, WeaponSpawnerType type, int poolIndex, Transform parent) {
        spawnTime = Time.time;
        this.poolIndex = poolIndex;
        this.spawner = spawner;
        this.type = type;
        this.parent = parent;
        this.transform.position = parent.position;
        this.transform.rotation = parent.rotation;
    }

    public void Update() {
        if (parent != null) {
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }
    }
}