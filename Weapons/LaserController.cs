using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class LaserController : MonoBehaviour, IWeaponController {
    public string weaponName;

    [Header("Assets")]
    public GameObject impact;
    public GameObject projectile;

    [Header("Pool")]
    public int initialSize;
    public int maximumSize = -1;

    public LayerMask impactLayer;

    private GameObjectPool weaponPool;
    private GameObjectPool impactPool;
    private IWeapon weapon;

    void Awake() {
        weaponPool = new GameObjectPool(projectile, maximumSize, initialSize, transform);
        impactPool = new GameObjectPool(impact, -1, initialSize / 2, transform);
        WeaponManager.Register(this);
        weapon = projectile.GetComponent<IWeapon>();
    }

    public bool CanFire {
        get { return true; }
    }

    public string WeaponName {
        get { return "Laser"; }
    }

    public float Range {
        get { return 500f ; }
    }

    public IWeapon Spawn(Transform gunpoint, Transform target) {
        Laser laser = weaponPool.Spawn(transform).GetComponent<Laser>();
        laser.Fire(gunpoint, target, this);
        return laser;
    }

    public void Impact(Laser laser, RaycastHit hit) {
        GameObject impactObject = impactPool.Spawn();
        LaserImpact impact = impactObject.GetComponent<LaserImpact>();
        impact.Spawn(this, hit.point + hit.normal * 0.2f);
        weaponPool.Despawn(laser.gameObject);

        //apply damage
        GameObject target = hit.transform.gameObject;
        Entity entity = target.GetComponent<Entity>();
        if (entity == null) return;
//        entity.ApplyDamage(laser.damage, hit);
    }

    public void Despawn(IWeapon weapon) {
        weaponPool.Despawn(((Laser)weapon).gameObject);
    }

    public void DespawnImpact(LaserImpact impact) {
        impactPool.Despawn(impact.gameObject);
    }
}
