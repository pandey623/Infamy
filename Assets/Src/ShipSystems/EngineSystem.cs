using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EngineSystem : MonoBehaviour {
    private Rigidbody rigidbody;
    private FlightControls controls;
    private Entity entity;

    public float MaxSpeed = 20f;
    public float AccelerationRate = 0.25f;
    public float TurnRate = 45f;
    public float Speed;
    public float radius;
    public float horizon = 5f;
    public float detectionRange = 10f;
    public Transform debugTransform;

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
        }
        radius = GetComponent<Collider>().bounds.extents.magnitude;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
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

    private Vector3 blend = Vector3.zero;
    private Vector3 lastDirection;
    private float lastDot = 0;

    public void FixedUpdate() {
        if(Vector3.Distance(debugTransform.position, transform.position) < 3f) {
         //   debugTransform.position = UnityEngine.Random.insideUnitSphere * UnityEngine.Random.Range(-100f, 100f);
        }
        controls.destination = debugTransform.position;
        Vector3 direction = AdjustForAvoidance();
        controls.SetThrottle(direction.magnitude / MaxSpeed);

        TurnTowardsDirection(direction);

        DrawArrow.ForDebug(transform.position, direction * 4f, Color.green);
      //  TurnTowardsPoint(controls.destination, controls.rollOverride);
        AdjustThrottle();
    }
    //        Vector3 force = goalVelocity - rigidbody.velocity; this yields a really interesting corkscrew behavior, cool for evasion

    public float SLOW_THRESHOLD = 0.25f; // this is a knob to turn to dictate how much time to spend decelerating --> lower value = more time
    private Vector3 AdjustForAvoidance() {
        Vector3 goalVelocity = (controls.destination - transform.position).normalized;// * MaxSpeed;//(rigidbody.velocity.magnitude + 0.25f); //todo this should be more dynamic
        float angle = Vector3.Angle(transform.forward, goalVelocity);
        float speed = TurnRate * Mathf.Deg2Rad * ((Vector3.Distance(transform.position, controls.destination) * SLOW_THRESHOLD)  / Mathf.Cos(angle * Mathf.Deg2Rad) );
       // if (speed < 0.25f) speed = 0f;
        if (speed < 0) speed = -speed;
        speed = Mathf.Clamp(speed, 0, MaxSpeed);
        Vector3 force = goalVelocity * speed;//(goalVelocity - transform.forward) * MaxSpeed;//2f * (goalVelocity - rigidbody.velocity);// -- this a know we can turn

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        for (var i = 0; i < colliders.Length; i++) {
            if (colliders[i].transform == transform) continue;
            Vector3 otherVelocity = Vector3.zero;
            Vector3 otherPosition = colliders[i].transform.position;
            float otherRadius = (colliders[i] as SphereCollider).radius * colliders[i].transform.localScale.x;
            Rigidbody otherRb = colliders[i].attachedRigidbody;

            if (otherRb) {
                otherVelocity = otherRb.velocity;
            }

            float timeToCollision = TimeToCollision(otherPosition, otherVelocity, otherRadius);
            if (timeToCollision < 0f || timeToCollision >= horizon) {
                continue;
            }
            Vector3 avoidForce = Vector3.zero;

            if (timeToCollision == 0) {
                avoidForce += (transform.position - colliders[i].transform.position);// * 2 * MaxSpeed;
                Debug.DrawRay(transform.position, avoidForce, Color.red);
            } else {
                avoidForce = transform.position + rigidbody.velocity * timeToCollision - otherPosition - otherVelocity * timeToCollision;
                if (avoidForce[0] != 0 && avoidForce[1] != 0 && avoidForce[2] != 0) {
                    avoidForce /= Mathf.Sqrt(Vector3.Dot(avoidForce, avoidForce));
                }
                float mag = 0f;
                if (timeToCollision >= 0 && timeToCollision <= horizon) {
                    mag = (horizon - timeToCollision) / (timeToCollision + 0.001f);
                }

                if (mag > MaxSpeed) {
                    mag = MaxSpeed;
                }

                if (mag > 0) {
                    Debug.DrawRay(transform.position, avoidForce, Color.black);
                } else {
                    Debug.DrawRay(transform.position, avoidForce, Color.yellow);
                }

                avoidForce *= mag;
            }


            force += avoidForce;
        }
        Debug.Log("MAG: " + force.magnitude);
        if (force.magnitude > MaxSpeed) {
            force = force.normalized * MaxSpeed;
        }

        return force;
    }

    //todo this is a helpful function for most AI -- move it elsewhere
    public float TimeToCollision(Vector3 otherPosition, Vector3 otherVelocity, float otherRadius) {
        float r = radius + (otherRadius);
        r *= 1.2f;
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

    public float m_AeroFactor = 1f;
    public float slip = 0.75f;
    public float testForce = 0f;
    public Timer timer = new Timer();

    private void AdjustThrottle() {

        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        float ForwardSpeed = Mathf.Max(0, localVelocity.z);

        //if(controls.ArrivalMode)
        //{

        //} else
        //{

        //}
        float targetSpeed = MaxSpeed * controls.throttle;
        float speedStep = AccelerationRate * MaxSpeed * Time.fixedDeltaTime;
        if (Speed > targetSpeed)
        {
            Speed = Mathf.Clamp(Speed - speedStep, 0, Speed);
        }
        else if (Speed < targetSpeed)
        {
            Speed = Mathf.Clamp(Speed + speedStep, 0, MaxSpeed);
        }

        //Speed = ForwardSpeed;
        rigidbody.velocity += transform.forward * Speed * Time.fixedDeltaTime;
        //rigidbody.AddForce(Speed * transform.forward, ForceMode.Acceleration);
        Debug.Log(ForwardSpeed + " --> " + timer.ElapsedTime);
        
        //rigidbody.AddForce(MaxSpeed * controls.throttle * transform.forward, ForceMode.Acceleration);
        if (rigidbody.velocity.sqrMagnitude > 0) {
            // compare the direction we're pointing with the direction we're moving
            //areoFactor is between 1 and -1
            m_AeroFactor = Vector3.Dot(transform.forward, rigidbody.velocity.normalized);
            // multipled by itself results in a desirable rolloff curve of the effect
            m_AeroFactor *= m_AeroFactor;
            // Finally we calculate a new velocity by bending the current velocity direction towards
            // the the direction the plane is facing, by an amount based on this aeroFactor
            var newVelocity = Vector3.Lerp(rigidbody.velocity, transform.forward * ForwardSpeed,
                                           m_AeroFactor * ForwardSpeed *
                                           slip * Time.deltaTime);
            rigidbody.velocity = newVelocity;

            // also rotate the plane towards the direction of movement
            //rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation,
            //                                      Quaternion.LookRotation(rigidbody.velocity, transform.up),
            //                                      m_AerodynamicEffect * Time.deltaTime);
        }

        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, MaxSpeed);
//        var velocity = rigidbody.velocity;
//        var magnitude = velocity.magnitude;
//        if (magnitude > 0 && magnitude > m_MaxEnginePower) {
//            velocity *= (m_MaxEnginePower / magnitude);
//            rigidbody.velocity = velocity;
//        }
        // Forward speed is the speed in the forward direction (not the same as its velocity)
        //  var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        //  ForwardSpeed = Mathf.Max(0, localVelocity.z);
        //rigidbody.velocity = transform.forward * Speed;
    }

    //input values are directions RELATIVE to CURRENT ORIENTATION, not accounting for angular velocity
    //which means to actually go in the desired direction, we need to take that input and adjust for AV

    //this treats inputs as 'desired angular velocities' 
    //note that we do not turn as tightly as we can in this mode because input nodes are mapped
    //to input values, so only outer nodes will result in a full strength turn

    //todo -- play with mode where we roll to the right orientation then pitch / yaw


    private void TurnTowardsDirection(Vector3 direction, float roll = 0f) {
        float turnRateModifier = 0.1f;
        float frameTurnModifier =  0.5f;
        Vector3 localTarget = transform.InverseTransformDirection(direction);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);

        Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
        float FrameTurnRateRadians = TurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime;
        //NOTE -- THIS USES A DIFFERENT ROLL MECHANISM THAN TURNTOWARDSPOINT!!!
        float desiredX = targetAnglePitch;
        float desiredY = targetAngleYaw;
        float desiredZ = -Mathf.Atan2(localTarget.x, localTarget.z);//roll * Mathf.Deg2Rad + GetRollAngle();
        float theta = 5 * Mathf.Deg2Rad;
        if (Util.Between(-theta, desiredX, theta) && Util.Between(-theta, desiredY, theta)) {
            desiredZ = roll * Mathf.Deg2Rad + GetRollAngle();
        }
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

        if (Mathf.Abs(desiredY) >= TurnRateRadians * turnRateModifier) {
            localAV.y += FrameTurnRateRadians * Mathf.Sign(desiredY - TurnRateRadians * turnRateModifier);
            localAV.y = Mathf.Clamp(localAV.y, -TurnRateRadians, TurnRateRadians);
        } else {
            if (desiredY >= localAV.y) {
                localAV.y = Mathf.Clamp(localAV.y + FrameTurnRateRadians * frameTurnModifier, localAV.y, desiredY);
            } else {
                localAV.y = Mathf.Clamp(localAV.y - FrameTurnRateRadians * frameTurnModifier, desiredY, localAV.y);
            }
        }

        if (Mathf.Abs(desiredZ) >= TurnRateRadians * turnRateModifier) {
            localAV.z += FrameTurnRateRadians * Mathf.Sign(desiredZ - TurnRateRadians * turnRateModifier);
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

    private void TurnTowardsPoint(Vector3 point, float roll = 0f) {
        float turnRateModifier = 0.1f;
        float frameTurnModifier = 0.5f;
        Vector3 localTarget = transform.InverseTransformPoint(point);
        float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
        float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);

        Vector3 localAV = transform.InverseTransformDirection(rigidbody.angularVelocity);
        float FrameTurnRateRadians = TurnRate * Mathf.Deg2Rad * Time.fixedDeltaTime;

        float desiredX = targetAnglePitch;
        float desiredY = targetAngleYaw;
        float desiredZ = roll * Mathf.Deg2Rad + GetRollAngle();
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

        if (Mathf.Abs(desiredY) >= TurnRateRadians * turnRateModifier) {
            localAV.y += FrameTurnRateRadians * Mathf.Sign(desiredY - TurnRateRadians * turnRateModifier);
            localAV.y = Mathf.Clamp(localAV.y, -TurnRateRadians, TurnRateRadians);
        } else {
            if (desiredY >= localAV.y) {
                localAV.y = Mathf.Clamp(localAV.y + FrameTurnRateRadians * frameTurnModifier, localAV.y, desiredY);
            } else {
                localAV.y = Mathf.Clamp(localAV.y - FrameTurnRateRadians * frameTurnModifier, desiredY, localAV.y);
            }
        }

        if (Mathf.Abs(desiredZ) >= TurnRateRadians * turnRateModifier) {
            localAV.z += FrameTurnRateRadians * Mathf.Sign(desiredZ - TurnRateRadians * turnRateModifier);
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



    //    public class ContextMap {
    //
    //        public int resolution;
    //        public float sphereCastRadius;
    //        private Entity entity;
    //        private SensorSystem sensorSystem;
    //        private Timer castTimer;
    //        public SteerNode[] nodes;
    //        public SteerNode highest;
    //        private List<CastHit> castHits;
    //        private Transform transform;
    //        public float maxScore;
    //        public float minScore;
    //
    //        public ContextMap(Entity entity, int resolution) {
    //            this.sphereCastRadius = 5f;
    //            this.entity = entity;
    //            this.resolution = resolution;
    //            this.sensorSystem = entity.sensorSystem;
    //            this.castTimer = new Timer();
    //            this.nodes = CreateNodeMap(resolution);
    //            this.castHits = new List<CastHit>(20); //todo pool these for less garbage
    //            this.transform = entity.transform;
    //            highest = nodes[0];
    //        }
    //
    //        public Vector3 Update() {
    //            for (int i = 0; i < nodes.Length; i++) {
    //                nodes[i].score = 0f;
    //            }
    //            castHits.Clear();
    //            GatherDangerInfo();
    //            SaturateTarget();
    //            SaturateDanger();
    //            return DesiredDirection();
    //        }
    //
    //        private Vector3 DesiredDirection() {
    //            maxScore = float.MinValue;
    //            minScore = float.MaxValue;
    //            for (int i = 0; i < nodes.Length; i++) {
    //                float score = nodes[i].score;
    //                if (score > maxScore) {
    //                    maxScore = score;
    //                    highest = nodes[i];
    //                } else if (score == maxScore && UnityEngine.Random.Range(0f, 1f) >= 0.5f) {
    //                    Debug.Log("TIE");
    //                    highest = nodes[i];
    //                } else if (score < minScore) {
    //                    minScore = score;
    //                }
    //            }
    //
    //            DotContainer dots = sensorSystem.RightUpForwardDotToVector(entity.controls.destination);
    //
    //            int index = GetIndex(dots.right, dots.up, resolution);
    //            var targetNode = nodes[index];
    //            if (targetNode != highest && targetNode.score == highest.score) {
    //                highest = targetNode;
    //            }
    //
    //            float percentX = highest.x / (float)(resolution - 1);
    //            float percentY = highest.y / (float)(resolution - 1);
    //
    //            if (percentX > 0.45f && percentX < 0.55f && percentY > 0.45f && percentY < 0.55f) {
    //                return entity.controls.destination;
    //            }
    //
    //            float xValue = (2 * percentX) - 1;
    //            float yValue = (2 * percentY) - 1;
    //            // Debug.Log("index: " + highest.x + ", " + highest.y + " mapped[-1, 1] " + xValue + ", " + yValue);
    //            return transform.position + transform.TransformDirection(new Vector3(xValue, yValue, 1f).normalized);
    //        }
    //
    //        private void GatherDangerInfo() {
    //            Vector3 topLeft = new Vector3(-1, 1, 0);
    //            Vector3 topRight = new Vector3(1, 1, 0);
    //            Vector3 bottomLeft = new Vector3(-1, -1, 0);
    //            Vector3 bottomRight = new Vector3(1, -1, 0);
    //
    //            RaycastHit[] hits = Physics.SphereCastAll(transform.position + transform.forward * sphereCastRadius, sphereCastRadius, transform.forward, 30f);//entity.engineSystem.Speed * 1.5f);
    //            for (int i = 0; i < hits.Length; i++) {
    //                if (hits[i].transform == sensorSystem.Target) continue;
    //                if (hits[i].transform == transform) continue;
    //
    //                var hitEntity = hits[i].transform.GetComponentInParent<Entity>();
    //                Debug.DrawLine(transform.position, hits[i].point, Color.yellow);
    //
    //                var contactPoint = hits[i].point;
    //                var toContactPoint = contactPoint - entity.transform.position;
    //                int casts = (int)hitEntity.size;
    //                castHits.Add(new CastHit(contactPoint, toContactPoint.normalized, toContactPoint.sqrMagnitude, Color.red, true));
    //                for (int j = 1; j <= casts; j++) {
    //                    float offset = (int)hitEntity.size * j;
    //                    if (j % 2 == 0) {
    //                        ProcessContactDirection(hitEntity, Vector3.up, contactPoint, offset);
    //                        ProcessContactDirection(hitEntity, Vector3.down, contactPoint, offset);
    //                        ProcessContactDirection(hitEntity, Vector3.left, contactPoint, offset);
    //                        ProcessContactDirection(hitEntity, Vector3.right, contactPoint, offset);
    //                    } else {
    //                        ProcessContactDirection(hitEntity, topLeft, contactPoint, offset);
    //                        ProcessContactDirection(hitEntity, topRight, contactPoint, offset);
    //                        ProcessContactDirection(hitEntity, bottomLeft, contactPoint, offset);
    //                        ProcessContactDirection(hitEntity, bottomRight, contactPoint, offset);
    //                    }
    //                }
    //            }
    //        }
    //
    //        private void ProcessContactDirection(Entity hitEntity, Vector3 offsetPoint, Vector3 contactPoint, float offset) {
    //            Vector3 rotatedPoint = transform.rotation * (offsetPoint * offset);
    //            Vector3 toOffsetPoint = ((rotatedPoint + contactPoint) - transform.position).normalized;
    //            //todo replace with raycast
    //            var intersection = hitEntity.GetRayIntersectionPoint(transform.position, toOffsetPoint);
    //            if (intersection != null) {
    //                float squareDistance = ((Vector3)intersection - transform.position).sqrMagnitude;
    //                castHits.Add(new CastHit((Vector3)intersection, toOffsetPoint, squareDistance, Color.white));
    //                //    if (drawCastLines) {transform.position + toOffsetPoint * 10f
    //                Debug.DrawLine(transform.position, (Vector3)intersection, Color.green);
    //                //  }
    //            }
    //        }
    //
    //        private void SaturateTarget() {
    //            DotContainer dots = sensorSystem.RightUpForwardDotToVector(entity.controls.destination);
    //            int index = GetIndex(dots.right, dots.up, resolution);
    //            Saturate(index, 1f, 0.75f);
    //            //todo -- add bonus strength to all nodes in the direction of our angular velocity trend
    //        }
    //
    //        //i need enough context to make a speed up / slow down / use afterburner decision
    //        private void SaturateDanger() {
    //            float speed = entity.engineSystem.Speed;
    //            speed *= speed;
    //            speed *= 1.5f;
    //            if (speed == 0) speed = 1;
    //            foreach (CastHit hit in castHits) {
    //                DotContainer dots = sensorSystem.RightUpForwardDotToVector(hit.direction);
    //                int index = GetIndex(dots.right, dots.up, resolution);
    //                float strength = 1f;
    //                float dropOff = 0.5f;
    //                if (hit.primary) {
    //                    strength = 1f;
    //                    dropOff = 0.75f;
    //                }
    //                float distanceScale = 1 - (hit.squareDistance / (speed));
    //                if (distanceScale > 0.9f) distanceScale = 1.25f;
    //                float forwardDotScale = dots.forward;
    //                if (dots.forward >= 0.95f) forwardDotScale = 2f;
    //                strength *= forwardDotScale;
    //                strength *= distanceScale;
    //                if (strength > 0) strength = -strength;
    //                Saturate(index, strength, dropOff);
    //            }
    //        }
    //
    //        private void Saturate(int index, float strength, float dropOff) {
    //            SteerNode origin = nodes[index];
    //            for (int i = 0; i < nodes.Length; i++) {
    //                SteerNode node = nodes[i];
    //                if (i == index) {
    //                    origin.score += strength;
    //                } else {
    //                    int xDiff = Math.Abs(origin.x - node.x);
    //                    int yDiff = Math.Abs(origin.y - node.y);
    //
    //                    float xScale = Mathf.Pow(dropOff, xDiff);
    //                    if (xScale == 0) xScale = 1;
    //                    node.score += strength * xScale * 0.5f;
    //
    //                    float yScale = Mathf.Pow(dropOff, yDiff);
    //                    if (yScale == 0) yScale = 1;
    //                    node.score += strength * yScale * 0.5f;
    //                }
    //            }
    //        }
    //
    //        public static SteerNode[] CreateNodeMap(int resolution) {
    //            if (resolution <= 2) resolution = 3;
    //            if (resolution % 2 == 0) resolution += 1;
    //            var nodes = new SteerNode[resolution * resolution];
    //            for (int i = 0; i < resolution * resolution; i++) {
    //                nodes[i] = new SteerNode(i, i % resolution, i / resolution);
    //            }
    //            return nodes;
    //        }
    //
    //        public static int GetIndex(float x, float y, int resolution) {
    //            int xIndex, yIndex;
    //
    //            if (x < 0.0001f && x > -0.0001f) x = 0;
    //            if (y < 0.0001f && y > -0.0001f) y = 0;
    //
    //            if (x == 1) {
    //                xIndex = resolution - 1;
    //            } else {
    //                float xPercent = (x + 1) / 2f;
    //                xIndex = (int)((resolution) * xPercent);
    //            }
    //            if (y == 1) {
    //                yIndex = resolution - 1;
    //            } else {
    //                float yPercent = (y + 1) / 2f;
    //                yIndex = (int)((resolution) * yPercent);
    //
    //            }
    //
    //            return yIndex * resolution + xIndex;
    //        }
    //
    //    }
    //
    //    public class SteerNode {
    //        public float score;
    //        public int x;
    //        public int y;
    //        public int index;
    //
    //        public SteerNode(int index, int x, int y) {
    //            this.index = index;
    //            this.x = x;
    //            this.y = y;
    //            score = 0f;
    //        }
    //    }
    //

}
