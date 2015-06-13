using UnityEngine;
using System.Collections;

public class FormationNode : MonoBehaviour {

    public float radius = 1.5f;
    private Vector3 target;
    Vector3 originalPosition;

	void Start () {
        originalPosition = transform.localPosition;
        target = Random.insideUnitSphere * radius;
	}
	
	void Update () {
        if ((originalPosition + target - transform.localPosition).sqrMagnitude < 1f) {
            target = Random.insideUnitSphere;
        } else {
            //could use a lastTargetPosition to have constant speed instead of transform.position which will be slower as it approaches and faster when far away
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + target, 0.25f * Time.deltaTime);
        }
	}
}
