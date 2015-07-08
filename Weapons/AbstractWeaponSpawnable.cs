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

    public virtual void Spawn(WeaponSpawner spawner, WeaponSpawnerType type, int poolIndex, Vector3 location, Quaternion rotation) {
        spawnTime = Time.time;
        this.poolIndex = poolIndex;
        this.spawner = spawner;
        this.type = type;
        this.transform.position = location;
        this.transform.rotation = rotation;
    }
}