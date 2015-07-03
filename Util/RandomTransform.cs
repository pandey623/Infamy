using UnityEngine;
using UnityEditor;
using System.Collections;

public class RandomTransform : MonoBehaviour {
    [Header("Position")]
    public bool randomPosition;
    public float range;
   
    [Header("Rotation")]
    public bool randomRotation;
    public Vector3 minRotation;
    public Vector3 maxRotation;
   
    [Header("Scale")]
    public bool randomScale;
    public Vector3 minScale;
    public Vector3 maxScale;

    private Vector3 origin = Vector3.zero;

    void Start() {
        if (randomPosition) {
            transform.localPosition = RandomPosition(range, transform.localPosition);
        }

        if (randomRotation) {
            transform.localRotation = Random.rotation;
        }
    }

    public void Randomize(float range = 100) {
        Randomize(range, Vector3.zero);
    }

    public void RandomizeScale(Vector3 originScale) {

    }

    public void Randomize(float range, Vector3 origin) {
        var random = new System.Random();
        var x = Random.Range(-range, range);
        var y = Random.Range(-range, range);
        var z = Random.Range(-range, range);
        transform.localPosition = origin + new Vector3(x, y, z);
    }

    public static Vector3 RandomPosition(Vector3 range, Vector3 origin) {
        Vector3 position = new Vector3();
        Vector3 random = Random.insideUnitSphere;
        position.x = origin.x + random.x * range.x;
        position.y = origin.y + random.y * range.y;
        position.z = origin.z + random.z * range.z;
        return position;
    }

    public static Vector3 RandomPosition(float range, Vector3 origin) {
        return (Random.insideUnitSphere + origin) * range;
    }

    public static Vector3 RandomPosition(float range) {
        return Random.insideUnitSphere* range;
    }

    public static Vector3 RandomScale(Vector3 range, Vector3 origin) {
        Vector3 scale = new Vector3();
        return scale;
    } 
}

[CustomEditor(typeof(RandomTransform))]
public class RandomLocationEditor : Editor {

    //public override void OnInspectorGUI() {
    //    DrawDefaultInspector();

    //    RandomTransform spawner = (RandomTransform)target;

    //    if (GUILayout.Button("Randomize")) {
    //        spawner.Randomize(spawner.range, spawner.origin);
    //    }
    //}
}