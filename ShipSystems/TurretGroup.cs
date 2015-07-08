using UnityEngine;

[System.Serializable]
public class TurretGroup { //Note WeaponGroup is basically identical, refactor into base class
    public string groupId;
    public string weaponId;
    public WeaponData weaponData;

    public TurretGroup(string name) {
        this.groupId = name;
    }

    public void SetWeaponData(string weaponId) {
        WeaponData data = WeaponDatabase.GetWeaponData(weaponId);
        if (data != null) {
            this.weaponId = weaponId;
            this.weaponData = data;
        } else {
            Debug.Log("Cannot find weapon data for: " + weaponId);
        }
    }

    public void Reset() {
        SetWeaponData(weaponId);
    }

    public WeaponData WeaponData {
        get { return weaponData; }
    }
    
    float BaseHullDamage {
        get { return weaponData.hullDamage; }
    }
    
    float BaseShieldDamage {
        get { return weaponData.hullDamage; }
    }
    
    float Range {
        get { return weaponData.range; }
    }

    float AspectFOV {
        get { return weaponData.aspectFOV; }
    }

    float AspectRange {
        get { return weaponData.aspectRange; }
    }

    bool AspectSeeking {
        get { return weaponData.aspectSeeking; }
    }

    float Accuracy {
        get { return weaponData.accuracy; }
    }

    bool Linkable {
        get { return false; }
    }

    public const string DefaultId = "default";
}