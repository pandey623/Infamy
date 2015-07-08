using UnityEngine;
using System.Collections.Generic;

//use actual prefab script for weapon behavior
//use weapon spawner for asset assignment / pool inputs
//use weapon spawner for spacial inputs
//use weapon spawner for creating / pooling audio, particles, lights, muzzle flashes, impacts
//use weapon variants for weapon characteristics (material, damage, speed, lifetime, range, aspect lock time)
//use weapon firing parameters for weapon data inputs

public enum WeaponSpawnerType {
    Projectile, MuzzleFlash, Impact, Audio
}

public class WeaponSpawner : MonoBehaviour {
    public GameObject weaponPrefab;

    protected GameObjectPool weaponPool;
    protected IWeapon referenceWeapon;
    public GameObject[] impactPrefabs;
    public GameObject[] muzzleFlashPrefabs;

    protected GameObjectPool[] impactPools;
    protected GameObjectPool[] muzzlePools;

    //todo these should be per-level configs or computed off of # of ships w/ weapon & fire rate etc
    public int initialPoolSize;
    public int maxPoolSize;
    public int maxImpactPoolSize = 10;
    public int initialImpactPoolSize = 5;
    public int maxMuzzleFlashPoolSize = 10;
    public int initialMuzzleFlashPoolSize = 5;
    protected int impactIdx;
    protected int muzzleIdx;

    public void Awake() {
        referenceWeapon = weaponPrefab.GetComponent<IWeapon>();
        WeaponSpawner.Register(referenceWeapon.Name, this);
        impactPools = new GameObjectPool[impactPrefabs.Length];
        muzzlePools = new GameObjectPool[muzzleFlashPrefabs.Length];
        weaponPool = new GameObjectPool(weaponPrefab, maxPoolSize, initialPoolSize, transform);
        for (int i = 0; i < impactPrefabs.Length; i++) {
            impactPools[i] = new GameObjectPool(impactPrefabs[i], maxImpactPoolSize, initialImpactPoolSize, transform);
        }
        for (int i = 0; i < muzzleFlashPrefabs.Length; i++) {
            muzzlePools[i] = new GameObjectPool(muzzleFlashPrefabs[i], maxMuzzleFlashPoolSize, initialMuzzleFlashPoolSize, transform);
        }
        impactIdx = 0;
        muzzleIdx = 0;
    }

    public IWeapon Spawn(WeaponFiringParameters firingParameters, Vector3 position, Quaternion rotation) {
        GameObject weaponRoot = weaponPool.Spawn();
        IWeapon weapon = weaponRoot.GetComponent<IWeapon>();
        weaponRoot.transform.position = position;
        weaponRoot.transform.rotation = rotation;
        weapon.Fire(this, firingParameters);
        return weapon;
    }

    public void Despawn(AbstractWeaponSpawnable spawnable) {
        Despawn(spawnable.gameObject, spawnable.type, spawnable.poolIndex);
    }

    public void Despawn(GameObject obj, WeaponSpawnerType type = WeaponSpawnerType.Projectile, int poolIndex = -1) {
        switch (type) {
            case WeaponSpawnerType.Impact:
                impactPools[poolIndex].Despawn(obj);
                break;
            case WeaponSpawnerType.MuzzleFlash:
                muzzlePools[poolIndex].Despawn(obj);
                break;
            case WeaponSpawnerType.Projectile:
                weaponPool.Despawn(obj);
                break;
            case WeaponSpawnerType.Audio:
                break;
        }
    }

    public bool CanFire(WeaponFiringParameters firingParameters) {
        return weaponPool.CanSpawn && referenceWeapon.CanFire(firingParameters);
    }

    public GameObject SpawnMuzzleFlash(Vector3 position, Quaternion rotation) {
        GameObject despawnRoot = muzzlePools[muzzleIdx].Spawn();
        AbstractWeaponSpawnable despawner = despawnRoot.GetComponent<AbstractWeaponSpawnable>();
        despawner.Spawn(this, WeaponSpawnerType.MuzzleFlash, muzzleIdx, position, rotation);
        muzzleIdx++;
        if (muzzleIdx == muzzlePools.Length) {
            muzzleIdx = 0;
        }
        return despawnRoot;
    }

    public GameObject SpawnImpact(Vector3 position, Quaternion rotation) {
        GameObject despawnRoot = impactPools[impactIdx].Spawn();
        AbstractWeaponSpawnable despawner = despawnRoot.GetComponent<AbstractWeaponSpawnable>();
        despawner.Spawn(this, WeaponSpawnerType.Impact, impactIdx, position, rotation);
        impactIdx++;
        if (impactIdx == impactPools.Length) {
            impactIdx = 0;
        }
        return despawnRoot;
    }

    private static Dictionary<string, WeaponSpawner> spawners;

    static WeaponSpawner() {
        spawners = new Dictionary<string, WeaponSpawner>();
    }

    public static void Register(string id, WeaponSpawner instance) {
        spawners[id] = instance;
    }

    public static WeaponSpawner Get(string id) {
        WeaponSpawner spawner;
        if (spawners.TryGetValue(id, out spawner)) {
            return spawner;
        }
        return null;
    }
}