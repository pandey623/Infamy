using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//[CustomPropertyDrawer(typeof (TurretGroup))]
//public class TurretGroupDrawer : PropertyDrawer {
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//        EditorGUI.BeginProperty(position, label, property);
//        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//        Rect weaponIdRect = new Rect(position.x, position.y, 50, position.height);
//        EditorGUI.PropertyField(weaponIdRect, property.FindPropertyRelative("weaponId"));
//        EditorGUI.EndProperty();
//    }
//}

public class TurretSystem : MonoBehaviour {
    public List<Turret> turrets;
    public List<TurretGroup> turretGroups;

    void Start() {
        CollectTurrets();
    }

    public TurretGroup GetGroup(string groupId) {
        for (int i = 0; i < turretGroups.Count; i++) {
            if (turretGroups[i].groupId == groupId) return turretGroups[i];
        }
        return null;
    }

    public void AssignWeapons() {
        for (int i = 0; i < turretGroups.Count; i++) {
            for (int j = 0; j < turrets.Count; j++) {
                if (turrets[j].turretGroupId == turretGroups[i].groupId) {
                    turrets[j].SetTurretGroup(turretGroups[i]);
                }
            }
        }
    }

    public void CollectTurrets() {
        turrets = new List<Turret>(transform.GetComponentsInChildren<Turret>());
        if (turretGroups == null) {
            turretGroups = new List<TurretGroup>();
            turretGroups.Add(new TurretGroup(TurretGroup.DefaultId));
            for (int i = 0; i < turrets.Count; i++) {
                turrets[i].turretGroupId = TurretGroup.DefaultId;
            }
        }
        for (int i = 0; i < turretGroups.Count; i++) {
            if (turretGroups[i].weaponData == null) {
                turretGroups[i].Reset();
            }
        }
    }

    public void LockAllTurrets() {
        if (turrets == null) return;
        for (int i = 0; i < turrets.Count; i++) {
            SetTurretLocked(turrets[i], true);
        }
    }

    public void UnlockAllTurrets() {
        if (turrets == null) return;
        for (int i = 0; i < turrets.Count; i++) {
            SetTurretLocked(turrets[i], false);
        }
    }

    private void SetTurretLocked(Turret turret, bool locked) {

    }
}


