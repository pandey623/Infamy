using UnityEngine;
using System.Collections;
using UnityEditor;

public class ExperimentalForceSteering : MonoBehaviour {

    public Transform target;
    public float radius;
    public bool drawSphere = true;
    public bool drawForces = false;
    public float speed = 1f;
    public float toTargetForce = 5f;
    public float rotationSpeed = 25f;
    public float radiusOffset = 4f;
    public int hitCount = 0;
    public float topSpeed = 8f;
    public float acceleration = 1f;

    void OnDrawGizmos() {
        if (drawSphere) {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position - new Vector3(0, 0, radiusOffset), radius);
        }
    }

    void OnCollisionEnter(Collision other) {
        Debug.Log("Collided!");
        Debug.Break();
    }

    // Update is called once per frame
    private void FixedUpdate() {
        hitCount = 0;
        Vector3 topLeft = new Vector3(-1, 1, 0);
        Vector3 topRight = new Vector3(1, 1, 0);
        Vector3 bottomLeft = new Vector3(-1, -1, 0);
        Vector3 bottomRight = new Vector3(1, -1, 0);
        Color[] colors = new Color[4];
        colors[0] = Color.red;
        colors[1] = Color.blue;
        colors[2] = Color.green;
        colors[3] = Color.white;
        Vector3 toTarget = (target.position - transform.position).normalized;
        Vector3 steeringForce = Vector3.zero;
        DrawArrow.ForDebug(transform.position, toTarget * radius, Color.gray);

        //things to be smarter about
        // * stationary vs mobile -- use velocity direction & speed to 'project' entity n frames into the future
        // * speed control -- slow down when in crowded area
        // * prefer to turn in direction already turning (add bonus strength to forces pointing in direction of rotation)
        // * straight on collision
        // * if steering force is too small, pick a random direction instead
        // * scale forces from behind appropriately
        Collider[] hitColliders = Physics.OverlapSphere(transform.position - new Vector3(0, 0, radiusOffset), radius);
        for (int i = 0; i < hitColliders.Length; i++) {
            Collider collider = hitColliders[i];

            if (collider.transform == transform) continue;
            Entity entity = collider.gameObject.GetComponent<Entity>();
            if (entity == null) continue;

            Vector3 contactPoint = entity.orientedBoundingBox.ClosestPoint(transform.position);

            Vector3 force = ScoreContactPoint(contactPoint);
            Debug.DrawLine(transform.position, contactPoint, Color.magenta);
            for (int j = 1; j <= (int)entity.size; j++) {
                float offset = j * 1;
                if (j % 2 == 0) {
                    force += ProcessContactDirection(collider, Vector3.up * offset, contactPoint);
                    force += ProcessContactDirection(collider, Vector3.down * offset, contactPoint);
                    force += ProcessContactDirection(collider, Vector3.left * offset, contactPoint);
                    force += ProcessContactDirection(collider, Vector3.right * offset, contactPoint);
                } else {
//                    force += ProcessContactDirection(collider, topLeft * offset, contactPoint);
//                    force += ProcessContactDirection(collider, topRight * offset, contactPoint);
//                    force += ProcessContactDirection(collider, bottomLeft * offset, contactPoint);
//                    force += ProcessContactDirection(collider, bottomRight * offset, contactPoint);
                }
            }

            steeringForce += force;
        }
        if (hitCount == 0) {
            toTargetForce = 1f;
        } else {
            toTargetForce = 10f;
        }
        float magnitude = steeringForce.magnitude;
        steeringForce += toTarget * toTargetForce;
        DrawArrow.ForDebug(transform.position, steeringForce.normalized * 5, Color.yellow);
    }

    private Vector3 ScoreContactPoint(Vector3 hitPosition) {
        hitCount++;
        Vector3 direction = Vector3.zero;
        Vector3 toPoint = (hitPosition - transform.position).normalized;
        Debug.DrawLine(transform.position, hitPosition, Color.green);
       
        float fdot = Vector3.Dot(transform.forward, (hitPosition - transform.position).normalized);
        float strength = 10f * (1 - (hitPosition - transform.position).sqrMagnitude / (radius * radius));

        if (fdot == 1) {
            //todo -- special case somehow
        } else if (fdot > 0.1f) {
            strength *= fdot;
        } else {
            strength = 0f;
        }

    //    Debug.Log("Strength: " + strength + ", FDOT: " + fdot);
        direction += transform.up * strength * -Vector3.Dot(transform.up, toPoint);
        direction += transform.right * strength * -Vector3.Dot(transform.right, toPoint);
        return direction;
    }

    private Vector3 ProcessContactDirection(Collider collider, Vector3 offsetPoint, Vector3 contactPoint) {
        Vector3 rotatedPoint = offsetPoint;
        Vector3 toOffsetPoint = ((rotatedPoint + contactPoint) - transform.position);
        RaycastHit hit;
        if (collider.Raycast(new Ray(transform.position, toOffsetPoint), out hit, radius)) {
            return ScoreContactPoint(hit.point);
        }
        return Vector3.zero;
    }

    //	    float count = 0;
    //	    for (int i = 0; i < obstacles.Length; i++) {
    //	        Transform obstacle = obstacles[i].transform;
    //	        float distance = Vector3.Distance(transform.position, obstacle.position);
    //	        float strength = (1 - (distance / radius));
    //            DotContainer dots = DotContainer.ToVector(transform, obstacle.position);
    //            if(distance > radius) continue; //temp till sphere cast / overlap
    //	        if (dots.forward > highestForwardDot && distance >= distanceToHighestDot) {
    //	            highestForwardDot = dots.forward;
    //	            distanceToHighestDot = distance;
    //	        }
    //	        count ++;
    //	        Vector3 toObstacle = transform.position.To(obstacle.position);
    //
    //            //if obstacle space is very crowded ||
    //            //if an obstacle is within 0.5 * radius && forward dot is >= 0.9f ||
    //            //if a rough turn radius check says we cant make it at this speed
    //            //otherwise speed up
    //
    //            //for moving things
    //                //get its velocity
    //                //see if we might intersect -- how?
    //                //if intersection likely 
    //	      //  if (dots.forward > 0.95f) {
    //	     //       strength = strength * 2f;
    //	     //   }
    //	       // if (dots.forward < 0.5f) strength *= dots.forward;
    //	        if (strength < 0) strength = 0;
    //            steeringForce += transform.right * strength * -dots.right;
    //            steeringForce += transform.up * strength * -dots.up;
    //
    //            if (drawForces) {
    //                DrawArrow.ForDebug(transform.position, steeringForce.normalized * strength * 10f, colors[0]);
    //                //DrawArrow.ForDebug(transform.position, transform.right * -dots.right * strength, colors[1]);
    //                //DrawArrow.ForDebug(transform.position, transform.up * -dots.up * strength, colors[2]);
    //            }
    //	    }

    //        if (count == 0) {
    //	        toTargetForce = 1f;
    //	    } else {
    //            toTargetForce = 0.5f * steeringForce.magnitude;
    //	    }
    //
    //	    if (Vector3.Distance(target.position, transform.position) <= radius) {
    //	        steeringForce += toTarget.normalized * toTargetForce * 2;
    //	    } else {
    //	        steeringForce += toTarget.normalized * toTargetForce;
    //	    }
    //	    steeringForce /= obstacles.Length;
    //
    //	    if (highestForwardDot >= 0.9f && distanceToHighestDot <= 0.75f * radius) {
    //	        speed -= acceleration * Time.deltaTime;
    //	    } else {
    //	        speed += acceleration * Time.deltaTime;
    //	    }
    //        speed = Mathf.Clamp(speed, topSpeed * 0.5f, topSpeed);
    //
    //	    DrawArrow.ForDebug(transform.position, steeringForce.normalized * 3f, Color.yellow);
    //	    var dir = Vector3.RotateTowards(transform.forward, steeringForce.normalized, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 0.0f);
    //	    transform.rotation = Quaternion.LookRotation(dir);
    //	    transform.position += transform.forward * speed * Time.deltaTime;
    //	}
}
