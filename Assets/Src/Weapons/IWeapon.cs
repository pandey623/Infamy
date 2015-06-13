using UnityEngine;
public interface IWeapon {
    void Fire(Transform gunpoint, Transform target);
    float Range { get; }
}
