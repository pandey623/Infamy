using UnityEngine;

public interface IWeapon {
    void Fire(WeaponSpawner spawner, WeaponFiringParameters firingParameters);
    bool CanFire(WeaponFiringParameters firingParameters);
   
    GameObject gameObject { get; }
    Transform transform { get; }
    string Name { get; }
}