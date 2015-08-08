using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class Fire : Action {

    public AIPilot pilot;
    public WeaponSystem weapons;
    public SensorSystem sensors;

    public override void OnAwake() {
        pilot = GetComponent<AIPilot>();
        weapons = GetComponent<WeaponSystem>();
        sensors = GetComponent<SensorSystem>();
    }


    public override TaskStatus OnUpdate() {
        //todo replace with distance from weapon firepoint to target

        if (weapons.weaponGroups[0].Fire()) {
            return TaskStatus.Success;
        } else {
            return TaskStatus.Failure;
        }
    }
}