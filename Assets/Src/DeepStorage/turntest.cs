using UnityEngine;
using System.Collections;

public class turntest : MonoBehaviour {

    public Transform target;
    public float speed = 10;
    public float turnRate = 45;
    public float angle;
    public float maxSpeed = 3;
    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame

    float RadiansToTarget(Vector3 target) {
        var localTarget = transform.InverseTransformPoint(target);
        return Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
    }

    float PitchToTargetRadians(Vector3 localTarget) {
        return -Mathf.Atan2(localTarget.y, localTarget.z);
    }

    float PitchToTargetDegrees(Vector3 localTarget) {
        return -Mathf.Atan2(localTarget.y, localTarget.z) * Mathf.Rad2Deg;
    }

    float YawRollToTarget(Vector3 localTarget) {
        return Mathf.Atan2(localTarget.x, localTarget.z);
    }   

    //I want to know: what is the maximum speed I can move and still hit a target?

    //HUGE CAVEAT: this only takes in one rate of rotation, not rotation per axis

    //right answer here is probably:
    //for pitch only -- find arrival speed
    //for roll/yaw(pick one) -- find arrival speed
    //take smaller speed 
    //compare that speed against some threshold

    //maybe do two caclculations: 1 for pitch 1 for yaw or roll (whichever is the major axis of rotation)
    //if both pass we can reach target
    //if one fails need to slow down / turn harder on that axis
    //if both fail slow down / turn harder
    //if speed is below some threshold we know to get some distance first

    //can reach target at current speed if currendSpeed < Mathf.Abs(SpeedToArriveAtTarget());
    //once distance and angle are known, it is cheap to test if a given rotation rate at a given speed will reach the target
    //be sure to pre compute distance and Cos(Angle) as these are heavy.
    //this can be a negative speed which means target is behind us
    //I *think* if target is behind us, we need to cut turn rate in half in this calculation
    float SpeedToArriveAtTarget(float distance, float angle) {
        return (turnRate * Mathf.Deg2Rad) * (distance / 2) * Mathf.Cos(angle);
    }

    public float pitchRate = 45;

    //this is used to find the direction to target as though we had no z axis
    //it is used to determine if something is our turning radius. 
    //we find our turning radius by taking rotation rate in radians / speed
    //then use that radius and the flat z direction to find a sphere center
    //if target is inside the sphere it is too close to turn to at our current speed

    public Vector3 FlatZToTarget(Vector3 target) {
        float toTargetX = target.x - transform.position.x;
        float toTargetY = target.y - transform.position.y;
        return new Vector3(toTargetX, toTargetY, 0);
    }

    void Update() {
        float pitchRadius = speed / (pitchRate * Mathf.Deg2Rad);
        float turnRadius = speed / (turnRate * Mathf.Deg2Rad);
        
        var targetPos = target.position;
        var dirToTarget = target.position - transform.position;



     //   Debug.DrawLine(transform.position, (toTarget.normalized) * 10, Color.blue);
        //Vector3 lastPosition = transform.position - transform.forward;
        //float radiansToTarget = RadiansToTarget(target.position);
        //float distanceToTarget = Vector3.Distance(transform.position, target.position);
        

        //angle = RadiansToTarget(target.position);
        //speed = (turnRate * Mathf.Deg2Rad) * (distanceToTarget / 2) * Mathf.Cos(angle * Mathf.Deg2Rad);
        //var localTarget = transform.InverseTransformPoint(target.position);
       // var p = new Plane(target.position, lastPosition, transform.position);

       // DrawPlane(transform.position, p.normal * 20);
       /// Vector2 goal = PointToUV(target.position, p);
       // /Vector2 self = PointToUV(transform.position, p);
        //var distance = Vector3.Distance(target.position, transform.position);
        //angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        //speed = (turnRate * Mathf.Deg2Rad) * (distance / 2) * Mathf.Cos(angle);
        //if (speed < 0) {
        //    Debug.Log(speed);
        //}
        //speed = Mathf.Clamp(speed, 0, maxSpeed);
        //transform.position += (transform.forward * speed) * Time.deltaTime;
        //var desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, turnRate * Time.deltaTime);


    }


}
