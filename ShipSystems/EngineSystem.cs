using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


/*
Spline Mode
    Auto -- Follow spline-defined parameters for acceleration / orientation / arrival
    Arrive -- Follow spline and arrive at end of spline, used for docking probably
    Manual -- Follow spline with AI defined acceleration and orientation -- custom maneuvers
    Loose -- Follow spline & avoid things, this is used for patrol and evade

MovementModes
    TurnTowards -- no explicit speed adjustments past AI input
    ArriveAt -- will stop at point, AI sets preferred speed (or top speed)
    SlipTowoards     
    
Formation Movement -- use Arrive at control mode
        Adjusting arrivalDecelerationScale between 0.1 & 1 will scale the tightness to formation point
        Leader should move at 60% of slowest formation member speed && turn at 70% of least agile formation member turn rate
        Formation members should have a reduced avoidance to each other
        When leader is turning, slightly reduce arrivalDecelerationScale if it is over 0.6 to give formation members time to align before overshooting formation node
*/


public class EngineSystem : MonoBehaviour {
    private new Rigidbody rigidbody;
    private FlightControls controls;
    private Entity entity;

    public float MaxSpeed = 10f;
    public float AccelerationRate = 0.25f;
    public float TurnRate = 90f;
    public float Speed;
    public bool useColliderRadius;
    public float radius;
    public float horizon = 5f;
    public float detectionRange = 10f;
    public Transform debugTransform;
    public CurvySpline spline;
    public float areoFactor = 1f;
    public float slip = 0.75f;
    public Timer timer = new Timer();
    public float arrivalDecelerationScale = 0.25f; // this is a knob to turn to dictate how much time to spend decelerating --> lower value = more time

    public float currentTF = 0f;
    public int dr = 1;
    public bool drawRadius = false;
    public bool drawAvoidanceForces = false;

    public void Awake() {
        this.controls = new FlightControls();
    }

    public void Start() {
        this.entity = GetComponentInParent<Entity>();
        this.rigidbody = GetComponentInParent<Rigidbody>();
        Assert.IsNotNull(this.entity, "EngineSystem has a null Entity");
        Assert.IsNotNull(this.rigidbody, "EngineSystem expects to find a Rigidbody in its parent");
        if (rigidbody != null) {
            rigidbody.angularDrag = 1.5f;
            rigidbody.useGravity = false;
        }
        Collider collider = GetComponent<Collider>();
        if (useColliderRadius && collider != null) {
            radius = GetComponent<Collider>().bounds.extents.magnitude;
        }
        controls.spline = spline; // TEMPORARY!!
        controls.destination = transform.position;
    }

    void OnDrawGizmos() {
        if (drawRadius) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    private float GetRollAngle() {
        var flatForward = transform.forward;
        float rollAngle = 0f;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude > 0) {
            flatForward.Normalize();
            var localFlatForward = transform.InverseTransformDirection(flatForward);
            var flatRight = Vector3.Cross(Vector3.up, flatForward);
            var localFlatRight = transform.InverseTransformDirection(flatRight);
            rollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
        }
        return rollAngle;
    }

    //todo this doesnt belong here, spline following should be a behavior
    //assumes we are aligned with the spline we want to follow already
    public void AutoPilotFollowSpline() {
        CurvySpline spline = FlightControls.spline;
        if (spline == null || !spline.IsInitialized) return;
        //when following a spline, need to check for obstructions and avoid them
        float roll = 0f;
        float splineSpeed = MaxSpeed;
        float tf = FlightControls.splineTF;
        if (spline == null) return;

        CurvySplineSegment next = spline.TFToSegment(currentTF).NextSegment;
        if (next != null) {
            roll = next.transform.eulerAngles.z;
            InfamySplineSegment segment = next.gameObject.GetComponent<InfamySplineSegment>();
            if (segment != null) {
                controls.SetThrottle(segment.throttle);

            } else {
                controls.SetThrottle(1f);
            }
        }


        controls.SetThrottle(1f);
        float dist = (controls.destination - transform.position).magnitude;
        float speed = controls.throttle * MaxSpeed;
        if (dist > MaxSpeed) {
            speed = MaxSpeed;
            currentTF = spline.GetNearestPointTF(transform.position + (transform.forward * speed * 0.75f));
        }
        controls.destination = spline.MoveByFast(ref currentTF, ref dr, speed * Time.fixedDeltaTime, CurvyClamping.Clamp);
        Vector3 goalDirection = (controls.destination - transform.position).normalized;
        TurnTowardsDirection(goalDirection, roll);
        AdjustThrottleByVelocity();
    }
    public bool playerMode = false;

    private void UpdatePlayerMode() {
        Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
        float turnRateRadians = TurnRate * Mathf.Deg2Rad;
        float turnStep = turnRateRadians * Time.fixedDeltaTime;

        localAV.x += FlightControls.pitch * turnStep;
        localAV.y += FlightControls.yaw * 0.5f * turnStep; //todo make these variables
        localAV.z += FlightControls.roll * 2f * turnStep;

        localAV.x = Mathf.Clamp(localAV.x, -turnRateRadians, turnRateRadians);
        localAV.y = Mathf.Clamp(localAV.y, -turnRateRadians, turnRateRadians);
        localAV.z = Mathf.Clamp(localAV.z, -turnRateRadians, turnRateRadians);

        rigidbody.angularVelocity = transform.TransformDirection(localAV);
        AdjustThrottleByForce();
    }

    public void FixedUpdate() {
        if (playerMode) {
            UpdatePlayerMode();
        } else {
            Vector3 direction = (controls.destination - transform.position).normalized;
            //TurnTowardsDirection(direction);
            TurnTest(direction);
            if (Vector3.Distance(transform.position, debugTransform.position) < 5f) {
               // debugTransform.position = Random.insideUnitSphere * 100f;
            }
            //Vector3 direction = AdjustForAvoidance();
           // controls.SetThrottle(1f);
            //controls.SetThrottle(direction.magnitude / MaxSpeed);
            if (controls.throttle != 0) {
          //      TurnTowardsDirection(direction, 0f);
          //      DrawArrow.ForDebug(transform.position, direction.normalized * 4f, Color.green);
            }
            AdjustThrottleByForce();
            //AdjustThrottleByVelocity();
        }
    }

    //maybe this should live on sensor system?
    public List<PossibleCollision> GetPossibleCollisions() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        List<PossibleCollision> possibleCollisions = new List<PossibleCollision>(colliders.Length);
        for (var i = 0; i < colliders.Length; i++) {
            if (colliders[i].transform == transform) continue;
            Vector3 otherVelocity = Vector3.zero;
            Vector3 otherPosition = colliders[i].transform.position;
            Vector3 size = colliders[i].bounds.size;
            float otherRadius = Mathf.Max(size.x, Mathf.Max(size.y, size.z)) * colliders[i].transform.localScale.x;
            Rigidbody otherRb = colliders[i].attachedRigidbody;

            if (otherRb) {
                otherVelocity = otherRb.velocity;
            }

            float timeToCollision = TimeToCollision(otherPosition, otherVelocity, otherRadius);
            if (timeToCollision < 0f || timeToCollision >= horizon) {
                continue;
            }
            possibleCollisions.Add(new PossibleCollision(otherRadius, timeToCollision, otherVelocity, colliders[i].transform));

        }
        return possibleCollisions;
    }

    private Vector3 GetCollisionAvoidanceForce(List<PossibleCollision> possibleCollisions) {
        Vector3 force = Vector3.zero;
        Vector3 velocity = rigidbody.velocity;
        Vector3 position = transform.position;

        for (var i = 0; i < possibleCollisions.Count; i++) {
            PossibleCollision ps = possibleCollisions[i];
            Vector3 avoidForce = Vector3.zero;
            float timeToImpact = ps.timeToImpact;

            if (ps.timeToImpact == 0) {
                avoidForce += (position - ps.transform.position);// * 2 * MaxSpeed;
                if (drawAvoidanceForces) Debug.DrawRay(position, avoidForce, Color.red);
            } else {
                avoidForce = position + velocity * timeToImpact - ps.transform.position - ps.velocity * timeToImpact;
                if (avoidForce[0] != 0 && avoidForce[1] != 0 && avoidForce[2] != 0) {
                    avoidForce /= Mathf.Sqrt(Vector3.Dot(avoidForce, avoidForce));
                }
                float mag = 0f;
                if (timeToImpact >= 0 && timeToImpact <= horizon) {
                    mag = Mathf.Clamp(mag, 0, (horizon - timeToImpact) / (timeToImpact + 0.001f));
                }

                if (drawAvoidanceForces) {
                    if (mag > 0) {
                        Debug.DrawRay(position, avoidForce, Color.black);
                    } else {
                        Debug.DrawRay(position, avoidForce, Color.yellow);
                    }
                }

                avoidForce *= mag;
            }

            force += avoidForce;
        }
        return (force.magnitude > MaxSpeed) ? force.normalized * MaxSpeed : force;
    }

    //Vector3 force = goalVelocity - rigidbody.velocity; this yields a really interesting corkscrew behavior, cool for evasion
    //todo when flight controls are not in auto mode will need to calculate this differently
    private Vector3 FindGoalVelocity() {
        //if controls.arriveAt || controls.spline.arriveAt -- do speed calc, else return goal velocity * throttle
        Vector3 goalVelocity = (controls.destination - transform.position).normalized;// * MaxSpeed;//(rigidbody.velocity.magnitude + 0.25f); //todo this should be more dynamic
        float angle = Vector3.Angle(transform.forward, goalVelocity) * Mathf.Deg2Rad;
        float distance = Vector3.Distance(transform.position, controls.destination);
        float speed = TurnRate * Mathf.Deg2Rad * ((distance * arrivalDecelerationScale) / Mathf.Cos(angle));
        if (speed < 0) speed = -speed;
        if (speed < 0.25f) speed = 0f;
        speed = Mathf.Clamp(speed, 0, MaxSpeed);
        return goalVelocity * speed;// * speed;//(goalVelocity - transform.forward) * MaxSpeed;//2f * (goalVelocity - rigidbody.velocity);// -- this a knob we can turn
    }

    private Vector3 AdjustForAvoidance() {
        List<PossibleCollision> possibleCollisions = GetPossibleCollisions();
        Vector3 goalForce = FindGoalVelocity();
        Vector3 avoidanceForce = GetCollisionAvoidanceForce(possibleCollisions);
        return goalForce + avoidanceForce;
    }

    private void AdjustThrottleByForce() {
        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        float ForwardSpeed = Mathf.Max(0, localVelocity.z);
        if (controls.throttle == 0) {
            float speedStep = AccelerationRate * MaxSpeed * Time.fixedDeltaTime;
            var mag = localVelocity.magnitude;
            //todo setting velocity outright is not so good
            if (ForwardSpeed > 0) {
                rigidbody.velocity -= transform.forward * speedStep;
            }
            if (ForwardSpeed <= 0) {
                rigidbody.velocity = Vector3.zero;
            }
            return;
        }
        rigidbody.AddForce(MaxSpeed * controls.throttle * transform.forward, ForceMode.Acceleration);
        if (rigidbody.velocity.sqrMagnitude > 0) {
            // compare the direction we're pointing with the direction we're moving
            //areoFactor is between 1 and -1
            areoFactor = Vector3.Dot(transform.forward, rigidbody.velocity.normalized);
            // multipled by itself results in a desirable rolloff curve of the effect
            areoFactor *= areoFactor;
            // Finally we calculate a new velocity by bending the current velocity direction towards
            // the the direction the plane is facing, by an amount based on this aeroFactor
            rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, transform.forward * ForwardSpeed, areoFactor * ForwardSpeed * slip * Time.deltaTime);
        }
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, MaxSpeed);
        Speed = rigidbody.velocity.magnitude;
    }

    private void AdjustThrottleByVelocity() {
        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        float targetSpeed = MaxSpeed * controls.throttle;
        float speedStep = AccelerationRate * MaxSpeed * Time.fixedDeltaTime;
        var mag = localVelocity.magnitude;
        //todo setting velocity outright is not so good
        if (mag > targetSpeed) {
            rigidbody.velocity = transform.forward * targetSpeed;
        } else if (mag < targetSpeed) {
            rigidbody.velocity += transform.forward * speedStep;
        }
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, MaxSpeed);
    }

    //input values are directions RELATIVE to CURRENT ORIENTATION, not accounting for angular velocity
    //which means to actually go in the desired direction, we need to take that input and adjust for AV

    //this treats inputs as 'desired angular velocities' 
    //note that we do not turn as tightly as we can in this mode because input nodes are mapped
    //to input values, so only outer nodes will result in a full strength turn

    private void TurnTest(Vector3 direction) {
        Vector3 localTarget = transform.InverseTransformDirection(direction);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);
        float yaw = Mathf.Clamp(targetAngleYaw, -1, 1);
        float pitch = Mathf.Clamp(targetAnglePitch, -1, 1);
        float roll = Mathf.Clamp(-targetAngleYaw + GetRollAngle(), -1, 1);
        controls.SetStickInputs(yaw, pitch, roll);
        UpdatePlayerMode();
    }

    private void TurnTowardsDirection(Vector3 direction, float roll = 0f, float rollTheta = 50f) {
        roll = Util.WrapAngle180(roll);

        float turnRateModifier = 1f;
        float frameTurnModifier = 1f;// 0.5f;
        Vector3 localTarget = transform.InverseTransformDirection(direction);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);

        Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
        float FrameTurnRateRadians = TurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime;

        float desiredX = targetAnglePitch;
        float desiredY = targetAngleYaw;
        float desiredZ = -targetAngleYaw + GetRollAngle();//roll * Mathf.Deg2Rad + GetRollAngle();
        desiredZ = Util.WrapRadianPI(desiredZ);//stops endless spinning 

        float TurnRateRadians = TurnRate * Mathf.Deg2Rad;

        if (Mathf.Abs(desiredX) >= TurnRateRadians * turnRateModifier) {
            localAV.x += FrameTurnRateRadians * Mathf.Sign(desiredX - TurnRateRadians * turnRateModifier);
            localAV.x = Mathf.Clamp(localAV.x, -TurnRateRadians, TurnRateRadians);
        } else {
            if (desiredX >= localAV.x) {
                localAV.x = Mathf.Clamp(localAV.x + FrameTurnRateRadians * frameTurnModifier, localAV.x, desiredX);
            } else {
                localAV.x = Mathf.Clamp(localAV.x - FrameTurnRateRadians * frameTurnModifier, desiredX, localAV.x);
            }
        }
        float yawMod = 1f;
        float rollMod = 1f;
        if (Mathf.Abs(desiredY) >= TurnRateRadians * turnRateModifier) {
            localAV.y += FrameTurnRateRadians * Mathf.Sign(desiredY - TurnRateRadians * turnRateModifier) * yawMod;
            localAV.y = Mathf.Clamp(localAV.y, -TurnRateRadians, TurnRateRadians);
        } else {
            if (desiredY >= localAV.y) {
                localAV.y = Mathf.Clamp(localAV.y + (FrameTurnRateRadians * frameTurnModifier * yawMod), localAV.y, desiredY);
            } else {
                localAV.y = Mathf.Clamp(localAV.y - (FrameTurnRateRadians * frameTurnModifier * yawMod), desiredY, localAV.y);
            }
        }

        if (Mathf.Abs(desiredZ) >= TurnRateRadians * turnRateModifier) {
            localAV.z += FrameTurnRateRadians * Mathf.Sign(desiredZ - TurnRateRadians * turnRateModifier) * rollMod;
            localAV.z = Mathf.Clamp(localAV.z, -TurnRateRadians, TurnRateRadians);
        } else {
            if (desiredZ >= localAV.z) {
                localAV.z = Mathf.Clamp(localAV.z + FrameTurnRateRadians * frameTurnModifier, localAV.z, desiredZ);
            } else {
                localAV.z = Mathf.Clamp(localAV.z - FrameTurnRateRadians * frameTurnModifier, desiredZ, localAV.z);
            }
        }

        rigidbody.angularVelocity = transform.TransformDirection(localAV);
    }

    public FlightControls FlightControls {
        get {
            return controls;
        }
    }

    //todo this is a helpful function for most AI -- move it elsewhere
    public float TimeToCollision(Vector3 otherPosition, Vector3 otherVelocity, float otherRadius) {
        float r = radius + (otherRadius);
        r *= 1f; // todo tweak this
        Vector3 w = otherPosition - transform.position;
        float c = Vector3.Dot(w, w) - r * r;
        if (c < 0) {
            return 0;
        }
        Vector3 v = rigidbody.velocity - otherVelocity;
        float a = Vector3.Dot(v, v);
        float b = Vector3.Dot(w, v);
        float discr = b * b - a * c;
        if (discr <= 0)
            return -1;
        float tau = (b - Mathf.Sqrt(discr)) / a;
        if (tau < 0)
            return -1;
        return tau;
    }

}


//todo pool these
public class PossibleCollision {
    public float radius;
    public float timeToImpact;
    public Vector3 velocity;
    public Transform transform;

    public PossibleCollision(float radius, float timeToImpact, Vector3 velocity, Transform transform) {
        this.radius = radius;
        this.timeToImpact = timeToImpact;
        this.velocity = velocity;
        this.transform = transform;
    }
}