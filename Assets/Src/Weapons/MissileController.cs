using UnityEngine;

//Projectile class is free to make a lot of descisions here. 
//Unity Update vs manual OnUpdate, OnDespawn handler, spawn other effects from weapon vs controller
//per missile properties vs shared from controller. flexibility is key here
public class MissileController : MonoBehaviour, IWeaponController {
    private GameObjectPool weaponPool;

    [Header("Pool")]
    public int maximumPoolSize = -1;
    public int initialPoolSize;

    public GameObject prefab;
    
    [Header("Data")] 
    public float range;

    void Start() {
        weaponPool = new GameObjectPool(prefab, maximumPoolSize, initialPoolSize);
        WeaponManager.Register(this);
    }

    public IWeapon Spawn(Transform gunpoint, Transform target) {
        GameObject missileObject = weaponPool.Spawn();
        if (missileObject == null) return null;
        Missile missile = missileObject.GetComponent<Missile>();
        if (missile == null) return null;
        missile.Fire(gunpoint, missileObject.transform);
        return missile;
    }

    public void Despawn(IWeapon weapon) {
        Missile missile = weapon as Missile;
        weaponPool.Despawn(missile.gameObject);
    }

    public bool CanFire {
        get { return true; }
    }

    public string WeaponName {
        get { return "Missile"; }
    }

    public float Range {
        get { return range; }
    }
}