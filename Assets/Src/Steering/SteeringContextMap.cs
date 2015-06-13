//using UnityEngine;
//using System.Collections.Generic;

//public class SteeringContextMap : MonoBehaviour {
//    private float timeSinceLastCollision = 0f;

//    [Range(0, 1)]
//    public float dropOff = 0.5f;
//    [Range(0, 1)]
//    public float strength = 1f;
//    [Range(0, 1)]
//    public float targetStrength = 1f;
//    [Range(0, 1)]
//    public float targetDropOff = 0.5f;

//    public Transform target;
//    public Transform SteerNodePrefab;
//    public float sphereCastRadius = 4f;
//    public int resolution = 5;
//    public float spacing = 2f;
//    public float drawScale = 1f;
//    public float zOffset = 0f;
//    public bool move = false;
//    public bool useDanger = true;
//    public bool useTarget = true;

//    public Vector2 indexVector;
//    public Gradient interestGradient;
//    public Gradient dangerGradient;

//    private SpaceCraftController controller;
//    private Dictionary<Vector2, SteeringContextNode> map;
//    private List<SteeringContextNode> list;
//    private List<DangerTracker> dangerList;

//    void Start() {
//        controller = GetComponent<SpaceCraftController>();
//        list = new List<SteeringContextNode>();
//        map = CreateSteeringMap(ref list);
//        dangerList = new List<DangerTracker>();
//    }

//    public static int GetIndex(int subdvisions, float input) {
//        if (input == 1) return subdvisions - 1;
//        var percent = (input + 1) * 0.5f;
//        return (int)(subdvisions * percent);
//    }

//    void OnCollisionEnter() {
//        Util.LogAll("Went", timeSinceLastCollision, "seconds without colliding!");
//    }

//    void OnCollisionExit() {
//        timeSinceLastCollision = 0f;
//    }

//    public bool drawTurnRadius = true;
//    void OnDrawGizmos() {
//        try {
//            Gizmos.color = Color.magenta;
//            Gizmos.DrawWireSphere(transform.position + (transform.forward * controller.ForwardSpeed * 1.5f), sphereCastRadius);
//            foreach (var dangerTracker in dangerList) {
//                if (dangerTracker.rayCasterId == 1) {
//                    Gizmos.color = Color.blue;
//                } else {
//                    Gizmos.color = Color.magenta;
//                }
//                Gizmos.DrawWireSphere(dangerTracker.point, 1.5f);
//            }
//            if (drawTurnRadius) {
//                float turningRadius = controller.TurnRadius;
//                var toTarget = target.position - transform.position;
//                toTarget = Vector3.ProjectOnPlane(toTarget, transform.forward);
//                var toCenterNormalized = toTarget.normalized;
//                var turnRadiusCenter = transform.position + toCenterNormalized * turningRadius;
//                Debug.DrawLine(transform.position, transform.position + toCenterNormalized * turningRadius, Color.blue);
//                Gizmos.color = Color.white;
//                Gizmos.DrawWireSphere(turnRadiusCenter, turningRadius);
//            }
//        } catch { }
//    }

//    void Update() {
//        timeSinceLastCollision += Time.deltaTime;
//        if (Vector3.Distance(transform.position, target.transform.position) < 3) {
//            target.GetComponent<RandomLocation>().Randomize();
//        }

//        list.ForEach((node) => node.Reset());

//        for (var i = 0; i < dangerList.Count; i++) {
//            var dangerTracker = dangerList[i];
//            if (!dangerTracker.Update()) {
//                dangerList.RemoveAt(i);
//                i--;
//            }
//        }

//        RayCast();
//        if (useTarget) {
//            SaturateTarget();
//        }
//        if (useDanger) {

//            SaturateDanger();
//        }
//        Move(ScoreNodes());

//    }

//    public SteeringContextNode ScoreNodes() {
//        float max = -100f;
//        float min = 100f;
//        SteeringContextNode highestNode = null;
//        map[new Vector2(resolution - 1, resolution - 1)].score = -1;
//        map[new Vector2(0, 0)].score = -1;
//        map[new Vector2(0, resolution - 1)].score = -1;
//        map[new Vector2(resolution - 1, 0)].score = -1;

//        var toTargetNormalized = (target.transform.position - transform.position).normalized;
//        var targetIndex = new Vector2();
//        targetIndex.x = GetIndex(resolution, Vector3.Dot(transform.right, toTargetNormalized));
//        targetIndex.y = GetIndex(resolution, Vector3.Dot(transform.up, toTargetNormalized));
//        var targetNode = map[targetIndex];

//        for (var i = 0; i < list.Count; i++) {
//            if (list[i].score > max) {
//                max = list[i].score;
//                highestNode = list[i];
//            }
//            if (list[i].score < min) {
//                min = list[i].score;
//            }
//        }

//        if (targetNode != highestNode) {
//            if (targetNode.score == highestNode.score) {
//                targetNode.score = highestNode.score + 1f;
//                highestNode = targetNode;
//            } else if (Mathf.Abs(targetNode.score - highestNode.score) < 0.15f * highestNode.score) {
//                targetNode.score = highestNode.score + 1f;
//                highestNode = targetNode;
//            }
//        }

//        foreach (var node in list) {

//            if (node.score >= 0) {
//                node.color = interestGradient.Evaluate(node.score / max);
//            } else {
//                node.color = dangerGradient.Evaluate((node.score * -1) / min * -1);
//            }
//        }
//        highestNode.color = Color.magenta;
//        return highestNode;
//    }

//    public float thrust = 1f;

//    public void Move(SteeringContextNode highestNode) {
//        if (move) {
//            thrust += 0.05f;
//            float rollInput, yawInput, pitchInput;
//            var halfres = (resolution - 1) / 2;
//            if (highestNode.index == new Vector2(halfres, halfres)) {
//                Vector3 localTarget = transform.InverseTransformPoint(target.position);
//                float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
//                float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);
//                float targetAngleRoll = targetAngleYaw;

//                pitchInput = targetAnglePitch;
//                yawInput = targetAngleYaw;
//                rollInput = -(controller.RollAngle - targetAngleRoll);

//            } else {
//                yawInput = Mathf.Lerp(-1, 1, highestNode.index.x / (resolution - 1));
//                pitchInput = -Mathf.Lerp(-1, 1, highestNode.index.y / (resolution - 1));
//                rollInput = -(controller.RollAngle - yawInput);
//            }

//            if (InsideTurningRadius(target.transform.position)) {
//                thrust = 0.25f;
//            }
//            Util.LogAll("Yaw", yawInput, "Pitch", pitchInput, "|", highestNode.index.x);

//            thrust = Mathf.Clamp01(thrust);
//            controller.Move(rollInput, pitchInput, yawInput, thrust);
//        }
//    }

//    //NOTE: turning radius degrees need to be multiplied by m_AeroFactor if useAeroFactor is turned on
//    //might need to factor in aerodyanmic effect as well.
//    public bool InsideTurningRadius(Vector3 point) {
//        return false;
//        ////  Vector3 flatZDirection = FlatZToTarget(point);
//        //  if (Vector3.Dot(flatZDirection, transform.forward) >= 0.99f) return false;
//        //  float turningRadius = GetTurnRadius();
//        //  Vector3 center = transform.position + (flatZDirection.normalized * turningRadius);
//        //  return (point - center).sqrMagnitude < turningRadius * turningRadius;// *1.3f; //little buffer room
//    }
//    //can reach target at current speed if currendSpeed < Mathf.Abs(SpeedToArriveAtTarget());
//    //once distance and angle are known, it is cheap to test if a given rotation rate at a given speed will reach the target
//    //be sure to pre compute distance and Cos(Angle) as these are heavy.
//    //this can be a negative speed which means target is behind us
//    //I *think* if target is behind us, we need to cut turn rate in half in this calculation
//    //float SpeedToArriveAtTarget(float distance, float angle) {
//    //    return (GetTurnRadius() * (distance / 2) * Mathf.Cos(angle);
//    //}


//    //TODO saturday morning -- when we get a collision point back from casting -- fastcast into a corona around the contact point
//    //up, down, left right, maybe diagonals too. use a spacing based on scale of ship ie massive is larger spacing than medium
//    //the goal is to get more resolution in the steering while not being overwhelming

//    public void SaturateDanger() {
//        Vector3 velocityDirection = GetComponent<Rigidbody>().velocity.normalized;
//        float distToTarget = (target.position - transform.position).sqrMagnitude;
//        float squareVelocity = GetComponent<Rigidbody>().velocity.sqrMagnitude;

//        RaycastHit hit;
//        Vector3 toTarget = (target.position - transform.position).normalized;
//        //if (Physics.Raycast(transform.position, toTarget, out hit, 20f)) {
//        //    if (hit.transform != target.transform) {
//        //        Vector2 indexVector = new Vector2();
//        //        float rightDot = Vector3.Dot(transform.right, toTarget);
//        //        float upDot = Vector3.Dot(transform.up, toTarget);
//        //        indexVector.x = GetIndex(resolution, rightDot);
//        //        indexVector.y = GetIndex(resolution, upDot);
//        //        FillSaturationScheme.Saturate(indexVector, list, resolution, -2f, dropOff);

//        //    }
//        //}

//        foreach (var d in dangerList) {
//            var dangerPoint = d.point;
//            var toDanger = dangerPoint - transform.position;
//            var toDangerNormalized = toDanger.normalized;
//            float rightDot = Vector3.Dot(transform.right, toDangerNormalized);
//            float upDot = Vector3.Dot(transform.up, toDangerNormalized);
//            float forwardDot = Vector3.Dot(transform.forward, toDangerNormalized);
//            float distToDanger = (dangerPoint - transform.position).sqrMagnitude;

//            indexVector = new Vector2();
//            indexVector.x = GetIndex(resolution, rightDot);
//            indexVector.y = GetIndex(resolution, upDot);

//            //target obscured -- reduce target strength
//            //if dot target.velocity, transform.forward > 0 && < 0.85 we might be in danger
//            //if dot target.velocity, transform.forward < 0 && > 0.85 we might be in danger

//            //casting, fastcast, oob intersection, positional projection

//            //if comprable heading 
//                //if comprable speed & distance > some threshold

//            //if colliding

//            //else if on collision path
//                //strength = 1.25f dropoff *=2
//                //steer away very strongly

//            //else if near collision path
//                //steer away strongly
//            //else if collision possible but not immediate
//                //steer away gently
                
//            //else target is facing away and wont collide at present velocity
//                //strength = 0

//            //todo maybe avoid points projected into the future -- find collision point from spherecast then move it by heading * entity speed and use that as collision point
//            if (Vector3.Dot(velocityDirection, toDangerNormalized) >= 0) {
//                //if mobile
//                    //if dot target.velocity, transform.forward > 0 && < 0.85 we might be in danger
//                    //if dot target.velocity, transform.forward < 0 && > 0.85 we might be in danger
                

//                float str = 2f;
//                float velocityDot = Vector3.Dot(transform.forward, d.transform.forward);
//                //Debug.Log(velocityDot);
//                //if (velocityDot > 0 && velocityDot < 0.85) {
//                //    str = 0;
//                //} else if (velocityDot < 0 && velocityDot > -0.85) {
//                //    str = 0;
//                //}
                
//                var distScale = 1 - (distToDanger / (squareVelocity));
//                str *= distScale;
//                str *= forwardDot;
//                //if (forwardDot > 0.85f && distToTarget < distToDanger) {
//                //    str *= 0.5f; //        
//                //}
                
//                if (str > 0) str = -str;
//                FillSaturationScheme.Saturate(indexVector, list, resolution, str, dropOff);
//            }
//        }
//    }

//    public void SaturateTarget() {
//        var toTarget = target.transform.position - transform.position;
//        var toTargetNormalized = toTarget.normalized;
//        indexVector = new Vector2();
//        indexVector.x = GetIndex(resolution, Vector3.Dot(transform.right, toTargetNormalized));
//        indexVector.y = GetIndex(resolution, Vector3.Dot(transform.up, toTargetNormalized));
//        FillSaturationScheme.Saturate(indexVector, list, resolution, targetStrength, targetDropOff);
//    }

//    private void RayCast() {

//        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, controller.ForwardSpeed * 1.5f);
//        ProcessRayCastHits(hits, 1);


//        hits = Physics.SphereCastAll(transform.position - (3 * transform.forward), sphereCastRadius, transform.forward, controller.ForwardSpeed * 1.5f);
//        ProcessRayCastHits(hits, 2);

//    }

//    private void ProcessRayCastHits(RaycastHit[] hits, int casterId) {
//        for (var i = 0; i < hits.Length; i++) {
//            if (hits[i].transform == transform) continue;
//            if (hits[i].transform == target) continue;

//            MeshRenderer renderer = hits[i].transform.GetComponent<MeshRenderer>();
//            bool skip = false;
//            foreach (var x in dangerList) {
//                if (x.rayCasterId == casterId && x.transform == hits[i].transform) {
//                    x.point = hits[i].point;
//                    x.frames = 0;
//                    skip = true;
//                    break;
//                }
//            }

//            if (skip) continue;
//            DangerTracker dangerTracker;
//            //var entity = hits[i].transform.GetComponent<Entity>();
//            //if (entity != null) {
//            //    for (var j = 0; j < list.Count; j++) {
//            //        var toNode = transform.rotation *  list[j].direction;
//            //        Debug.DrawLine(transform.position, transform.position + (toNode * 10f));
//            //        Vector3? intersection = entity.orientedBoundingBox.GetLineSegmentIntersectionPoint(transform, new Ray(transform.position, toNode), 20f);
//            //        if (intersection != null) {
//            //            dangerTracker = new DangerTracker();
//            //            dangerTracker.rayCasterId = casterId;
//            //            dangerTracker.transform = list[j].transform;
//            //            dangerTracker.point = (Vector3)intersection;
//            //            dangerTracker.material = renderer.material;
//            //            dangerTracker.frames = 0;
//            //            dangerList.Add(dangerTracker);
//            //        }
//            //    }
//            //}
//                renderer.material.color = Color.yellow;
//            dangerTracker = new DangerTracker();
//            dangerTracker.rayCasterId = casterId;
//            dangerTracker.transform = hits[i].transform;
//            dangerTracker.point = hits[i].point;
//            dangerTracker.material = renderer.material;
//            dangerTracker.frames = 0;
//            dangerList.Add(dangerTracker);
//        }
//    }

//    public Vector3 GetDrawOffset() {
//        float offset = (resolution / 2);
//        if (resolution % 2 == 0) {
//            offset -= 0.5f;
//        }
//        return new Vector3(offset, offset, 0);
//    }

//    public Dictionary<Vector2, SteeringContextNode> CreateSteeringMap(ref List<SteeringContextNode> list) {
//        Dictionary<Vector2, SteeringContextNode> retnMap = new Dictionary<Vector2, SteeringContextNode>();
//        //pitch to target scaled by allowable pitch rate
//        //yaw to target scaled by allowable yaw rate
//        var drawOffset = GetDrawOffset();
//        GameObject nodes = new GameObject();
//        nodes.transform.parent = transform;
//        nodes.transform.localPosition = Vector3.zero;
//        nodes.transform.localScale = Vector3.one;
//        for (var x = 0; x < resolution; x++) {
//            for (var y = 0; y < resolution; y++) {
//                var obj = Instantiate(SteerNodePrefab);
//                var node = obj.GetComponent<SteeringContextNode>();
//                node.transform.parent = nodes.transform;
//                node.transform.name = "(x: " + x + ", y : " + y + ")";
//                node.transform.localPosition = (new Vector3(x, y, 0) - drawOffset) * spacing;
//                node.transform.localScale = new Vector3(drawScale, drawScale, drawScale);
//                node.index = new Vector2(x, y);
//             //   node.direction = new Vector3(node.index.x - drawOffset.x, node.index.y - drawOffset.y, 1f);
//                retnMap.Add(node.index, node);
//                list.Add(node);
//            }
//        }

//        for (var i = 0; i < list.Count; i++) {
//            var node = list[i];
//            var x = node.index.x;
//            var y = node.index.y;
//            Vector2[] array = new Vector2[8];
//            array[0] = new Vector2(x - 1, y - 1);
//            array[1] = new Vector2(x - 1, y);
//            array[2] = new Vector2(x - 1, y + 1);
//            array[3] = new Vector2(x, y - 1);
//            array[4] = new Vector2(x, y + 1);
//            array[5] = new Vector2(x + 1, y - 1);
//            array[6] = new Vector2(x + 1, y);
//            array[7] = new Vector2(x + 1, y + 1);
//            foreach (Vector2 vec in array) {
//                SteeringContextNode neighbor;
//                if (retnMap.TryGetValue(vec, out neighbor)) {
//                    node.neighbors.Add(retnMap[vec]);
//                }
//            }

//        }
//        return retnMap;
//    }
//}