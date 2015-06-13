using UnityEngine;
public interface IWeaponController {
    IWeapon Spawn(Transform gunpoint, Transform target);
    void Despawn(IWeapon weapon);

    bool CanFire { get; }
    string WeaponName { get; }
    float Range { get; }
}