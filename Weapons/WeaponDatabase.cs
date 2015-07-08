using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WeaponData {
    public string weaponId;
    public float range;
    public float fireRate;
    public float aspectTime;
    public float lifeTime;
    public float accuracy;
    public float hullDamage;
    public float shieldDamage;
    public float speed;

    public float aspectFOV;
    public float aspectRange;
    public bool aspectSeeking;
    //todo add type, ammo, charge time, impact layer, fire mode, linkable etc

    public WeaponData(string weaponId) {
        this.weaponId = weaponId;
        WeaponDatabase.RegisterWeaponData(this.weaponId, this);
        this.range = 500f;
        this.fireRate = 1f;
        this.aspectTime = -1;
        this.lifeTime = -1;
        this.accuracy = 1f;
        this.hullDamage = 1f;
        this.shieldDamage = 1f;
        this.speed = 100f;
    }

    public static WeaponData Clone(WeaponData data) {
        return data.MemberwiseClone() as WeaponData;
    }
}

public class WeaponDatabase : MonoBehaviour {

    private static Dictionary<string, WeaponData> database;
    private static List<string> weaponList;
    private static string[] rawWeaponList;
    private static bool dirtyWeaponList;

    static WeaponDatabase() {
        database = new Dictionary<string, WeaponData>();
        weaponList = new List<string>();
        dirtyWeaponList = true;
        WeaponData laser = new WeaponData("Laser");
        laser.range = 350f;
        laser.lifeTime = -1;
        laser.hullDamage = 5;
        laser.aspectTime = -1;
        laser.accuracy = 0.9f;
        WeaponData vulcan = new WeaponData("Vulcan");
        WeaponData beam = new WeaponData("Beam");
        beam.lifeTime = 1.5f;
        beam.range = 50f;
        beam.speed = 50f;
    }


    public static string[] GetWeaponList() {
        if (dirtyWeaponList) {
            dirtyWeaponList = false;
            rawWeaponList = weaponList.ToArray();
        }
        return rawWeaponList;
    }

    public static WeaponData GetWeaponData(string weaponId) {
        WeaponData data;
        if (database.TryGetValue(weaponId, out data)) {
            return WeaponData.Clone(data);
        }
        return null;
    }

    public static void RegisterWeaponData(string weaponId, WeaponData data) {
        database[weaponId] = data;
        weaponList.Add(weaponId);
        dirtyWeaponList = true;
    }

    public static int GetWeaponIndex(string weaponId) {
        return weaponList.IndexOf(weaponId);
    }
}