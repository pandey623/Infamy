using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {
    private static Dictionary<string, IWeaponController> controllers = new Dictionary<string, IWeaponController>();

    public static void Register(IWeaponController controller) {
        controllers[controller.WeaponName] = controller;
    }

    public static IWeaponController GetWeaponController(string id) {
        IWeaponController controller;
        if (controllers.TryGetValue(id, out controller)) {
            return controller;
        } else {
            throw new System.Exception("No weapon controller for " + id + " is registered");
        }
    }
}
