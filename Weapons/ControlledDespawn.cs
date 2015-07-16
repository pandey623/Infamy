using UnityEngine;

public class ControlledDespawn : AbstractWeaponSpawnable {
    public bool shouldDespawn = false;

    public void Update() {
        base.Update();
        if (shouldDespawn) {
            shouldDespawn = false;
            spawner.Despawn(this);
        }
    }
}