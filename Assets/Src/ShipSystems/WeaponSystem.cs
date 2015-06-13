using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class WeaponSystem {

    public List<Gunbank> gunbanks = new List<Gunbank>();
    public Timer timer;
    private Gunbank currentGunbank;
    private IWeaponController controller;
    private bool fireRequested = false;
    private Transform transform;
    private Entity entity;
    private Vector3 target;

    public WeaponSystem(Entity entity) {
        this.entity = entity;
        this.transform = entity.transform;
        this.gunbanks = new List<Gunbank>(transform.GetComponentsInChildren<Gunbank>());
        this.timer = new Timer(0.20f);
        if (this.gunbanks.Count != 0) {
            this.currentGunbank = gunbanks[0];
            this.controller = WeaponManager.GetWeaponController(gunbanks[0].weaponId);
        }
    }

    public float ActivePrimaryRange  {
        get { return controller.Range; }
    }

    //todo -- fix this to accept a vector or transform and a score of 0,1 denoting accuracy
    public void Fire() {
        fireRequested = true;
    }

    public void Update() {
        if (fireRequested && timer.ReadyWithReset(0.25f)) {
            controller.Spawn(currentGunbank.NextHardpoint, null);
            fireRequested = false;
        }
    }

    public bool InPrimaryRange(Entity target) {
        return (target.transform.position - transform.position).sqrMagnitude <= ActivePrimaryRange * ActivePrimaryRange;
    }

    public Gunbank[] FindGunbanks() {
        return transform.GetComponentsInChildren<Gunbank>();
    }
}

public struct WeaponDescriptor {
    public string name;
    public string type;
}

//[CustomEditor(typeof(WeaponSystem))]
//public class WeaponSystemEditor : Editor {
//    public void OnEnable() {
//        WeaponSystem ws = (WeaponSystem)target;
//    }
//
//    public override void OnInspectorGUI() {
//        List<string> weaponNames = new List<string>();
//        weaponNames.Add("None");
//        weaponNames.Add("Laser");
//        weaponNames.Add("Missile");
//        WeaponSystem weaponSystem = target as WeaponSystem;
//        EditorGUIUtility.labelWidth = 75f;
//        Gunbank[] gunbanks = weaponSystem.GetComponentsInChildren<Gunbank>();
//        for (int i = 0; i < gunbanks.Length; i++) {
//            EditorGUIUtility.labelWidth = 75f;
//
//            Gunbank gunbank = gunbanks[i];
//            int index = weaponNames.IndexOf(gunbank.weaponId);
//            if (index == -1) index = 0;
//            EditorGUILayout.BeginHorizontal();
//            index = EditorGUILayout.Popup("Gunbank " + i, index, weaponNames.ToArray());
//            gunbank.weaponId = weaponNames[index];
//            EditorGUIUtility.labelWidth = 50f;
//            gunbank.ammunition = EditorGUILayout.IntField("Ammo", gunbank.ammunition);
//            EditorGUILayout.Toggle("Linkable", gunbank.linkable);
//            EditorGUILayout.EndHorizontal();
//        }
//    }
//}