using UnityEngine;
using UnityEditor;

//public class EntityInitializer : MonoBehaviour {
//    public float turnRate = 45f;
//    public float maxSpeed = 20f;
//    public float acceleration = 0.25f;
//    public bool piloted = false;
//    public EntitySize size = EntitySize.Small;
//    public EntityType type = EntityType.Fighter;
//    public FactionId id = FactionId.Maas;
//
//    void Start() {
//        ApplyChanges();
//    }
//
//    public void ApplyChanges() {
//        Entity entity = GetComponent<Entity>();
//        entity.engineSystem.TurnRate = turnRate;
//        entity.engineSystem.MaxSpeed = maxSpeed;
//        entity.engineSystem.AccelerationRate = acceleration;
//        entity.piloted = piloted;
//        entity.size = size;
//        entity.type = type;
//        entity.factionId = id;
//    }
//}
//
//[CustomEditor(typeof(EntityInitializer))]
//public class TempEntityInitializerEditor : Editor {
//    
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//        if (GUILayout.Button("Apply Changes")) {
//            EntityInitializer init = target as EntityInitializer;
//            init.ApplyChanges();
//        }
//    }
//}