using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour, IWeapon {

    public float range;
    //todo lifetime etc

    public void Fire(Transform origin, Transform target) {
        transform.position = origin.position;
        transform.rotation = origin.rotation;
    }

    public float Range {
        get { return range; }
    }

}