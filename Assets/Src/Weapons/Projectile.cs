using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public float velocity = 300f;
    public float rayCastAdvance = 2;
    public LayerMask layerMask;
    public GameObject impact;
    public float lifeTime = 1f;
    public float impactOffsetModifier = 0.2f;
    Timer timer;

	void Start () {
        timer = new Timer(lifeTime);
    }
	
	void Update () {
        Vector3 step = transform.forward * Time.deltaTime * velocity;
        RaycastHit hitPoint;
        if (Physics.Raycast(transform.position, transform.forward, out hitPoint, step.magnitude * rayCastAdvance, layerMask)) {
            if (impact) {
                Instantiate(impact, hitPoint.point + hitPoint.normal * impactOffsetModifier, Quaternion.identity);
            }
            Destroy(gameObject);
        } else if(timer.Ready) {
            Destroy(gameObject);
        }

        transform.position += step;
	}

}
