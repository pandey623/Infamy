using UnityEngine;

public class EntityDescriptor : ScriptableObject {
    public EntityStats[] entityStats;
}

//public class EntityIntializer : ScriptableObject {
//    [Header("BaseStats")]
//    private EntityIntializer parent;
//    public float MaxSpeed;
//    public float Agility;
//    public float Health;
//    public float MaxHealth;
//    public float Shield;
//    public FactionId Faction;
//    public float Thrust;

//    [Header("Starting Values")]
//    public string AI_GoalSet;
//    public string Script_HandlerSet;
//    public float Base_Speed;
//    public float Base_Agility;
//    public float Base_Shield;
//    public float Base_Hull;
//    public Vector3 Start_LookAt;
//    public Vector3 Start_Position;
//    public string Start_LookAtEntity;
//}