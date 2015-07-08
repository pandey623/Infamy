using System;
using UnityEngine;

public abstract class AbstractWeapon : MonoBehaviour, IWeapon {
    protected WeaponFiringParameters firingParameters;
    protected WeaponSpawner spawner;

    public abstract void Fire(WeaponSpawner spawner, WeaponFiringParameters firingParameters);
    
    public virtual bool CanFire(WeaponFiringParameters firingParameters) {
        float lastFireTime = firingParameters.lastFireTime;
        float rechargeOverride = firingParameters.fireRate;
        float rechargeTime = (rechargeOverride == -1) ? 1f : rechargeOverride;
        return Time.time - lastFireTime > rechargeTime;
    }

    public string Name {
        get { return GetType().ToString(); }
    }

}

