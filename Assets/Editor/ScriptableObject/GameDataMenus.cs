using UnityEngine;
using UnityEditor;
using System.Collections;

public class GameDataMenus : MonoBehaviour {

    [MenuItem("Assets/Create/GameData/FactionList")]
    public static void CreateFactions() {
        ScriptableObjectUtility.CreateAsset<FactionList>();
    }

    [MenuItem("Assets/Create/GameData/StringCollection")]
    public static void CreateWeaponList() {
//        ScriptableObjectUtility.CreateAsset<>();
    }

    [MenuItem("Assets/Create/GameData/ScriptableObject")]
    public static void CreateScriptableObject() {
        ScriptableObjectUtility.CreateAsset<ScriptableObject>();
    }
}
