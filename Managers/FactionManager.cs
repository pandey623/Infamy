using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[Serializable]
public class FactionDispositionMap : SerializableDictionary<string, Disposition> { }

public class FactionManager : MonoBehaviour {

    public static List<Faction> factions;
    private static FactionManager instance;

    [HideInInspector]
    public FactionDispositionMap factionDispositionMap;

    void Awake() {
        if (instance != null) return;
        instance = this;
        factions = new List<Faction>();
        foreach (FactionId factionId in Enum.GetValues(typeof(FactionId))) {
            factions.Add(new Faction(factionId, CreateDispositionMap(factionId)));
        }
        SetFactionsHostile(FactionId.Maas, FactionId.XWA);
    }

    private Dictionary<FactionId, Disposition> CreateDispositionMap(FactionId id) {
        Dictionary<FactionId, Disposition> map = new Dictionary<FactionId, Disposition>();
        foreach(FactionId factionId in Enum.GetValues(typeof(FactionId))) {
            if (factionId == id) continue;
            map[factionId] = factionDispositionMap[id.ToString() + ":" + factionId.ToString()];
        }
        return map;
    }

    public static void AddEntity(Entity entity) {
        for (int i = 0; i < factions.Count; i++) {
            factions[i].AddEntity(entity);
        }
    }

    public static Faction GetFaction(FactionId id) {
        return factions.Find((faction) => faction.id == id);
    }

    public static void SetFactionsHostile(FactionId factionId1, FactionId factionId2) {
        Faction f1 = factions.Find((f) => f.id == factionId1);
        Faction f2 = factions.Find((f) => f.id == factionId2);
        f1.SetFactionHostile(f2.id);
        f2.SetFactionHostile(f1.id);
    }

    public static void SetFactionsFriendly(FactionId factionId1, FactionId factionId2) {
        Faction f1 = factions.Find((f) => f.id == factionId1);
        Faction f2 = factions.Find((f) => f.id == factionId2);
        f1.SetFactionFriendly(f2.id);
        f2.SetFactionFriendly(f1.id);
    }

    public static void SetFactionsNeutral(FactionId factionId1, FactionId factionId2) {
        Faction f1 = factions.Find((f) => f.id == factionId1);
        Faction f2 = factions.Find((f) => f.id == factionId2);
        f1.SetFactionNeutral(f2.id);
        f2.SetFactionNeutral(f1.id);
    }

    public static void RemoveEntityFromFactions(Entity ent) {
        for (var i = 0; i < factions.Count; i++) {
            factions[i].RemoveEntity(ent);
        }
    }

    public static List<Entity> GetHostile(FactionId id) {
        return GetFaction(id).GetHostiles();
    }

    public static int GetHostileCount(FactionId id) {
        return GetFaction(id).HostileCount;
    }
}

[CustomEditor(typeof(FactionManager))]
public class FactionManagerEditor : Editor {

    private static List<FactionId> factionIds;
    private static List<Disposition> dispositions;
    private static string[] dispositionStrings;

    void OnEnable() {
        FactionManager manager = (FactionManager)target;
        if (manager.factionDispositionMap == null) {
            manager.factionDispositionMap = new FactionDispositionMap();
        }
        factionIds = new List<FactionId>((FactionId[])Enum.GetValues(typeof(FactionId)));
        dispositions = new List<Disposition>((Disposition[])Enum.GetValues(typeof(Disposition)));
        dispositionStrings = new string[dispositions.Count];
        for (int i = 0; i < dispositions.Count; i++) {
            dispositionStrings[i] = dispositions[i].ToString();
        }
    }

    public override void OnInspectorGUI() {
        FactionManager manager = (FactionManager)target;
        var dispositionMap = manager.factionDispositionMap;
        EditorGUILayout.Space();
        foreach (FactionId faction in factionIds) {
            EditorGUILayout.LabelField(faction.ToString());
            EditorGUI.indentLevel++;

            for (int i = 0; i < factionIds.Count; i++) {
                if (faction == factionIds[i]) continue;
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 50f;
                DrawFactionDisposition(faction, factionIds[i], true);
                DrawFactionDisposition(factionIds[i], faction, false);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
    }

    private void DrawFactionDisposition(FactionId currentFaction, FactionId otherFaction, bool useLabel) {
        FactionManager manager = (FactionManager)target;
        var dispositionMap = manager.factionDispositionMap;
        var label = useLabel ? otherFaction.ToString() : "";
        string lookup = currentFaction.ToString() + ":" + otherFaction.ToString();
        if (!dispositionMap.ContainsKey(lookup)) {
            dispositionMap[lookup] = Disposition.Neutral;
        }
        var disposition = dispositionMap[lookup];
        int index = dispositions.IndexOf(disposition);
      
        int selection = EditorGUILayout.Popup(label, index, dispositionStrings);
        if (selection != index) {
            dispositionMap[lookup] = dispositions[selection];
            if (Application.isPlaying) {
                FactionManager.GetFaction(currentFaction).SetDisposition(otherFaction, dispositions[selection]);
            }
        }
    }
}