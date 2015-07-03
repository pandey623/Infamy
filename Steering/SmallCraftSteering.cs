//using UnityEngine;
//using System;
//using System.Collections.Generic;


//public class SmallCraftSteering : MonoBehaviour {

//    public int resolution = 5;
//    public int castHitLifetime = 60;
//    public float sphereCastRadius = 5f;

//    public SteeringNode highest;
//    public List<SteeringNode> nodes;

//    private SpaceCraftController controller;
//    private List<CastHit> castHits;

//    [Header("Debug")]
//    public bool drawCastLines = true;
//    public bool drawCastHits = true;
//    private Vector3 targetPosition;

//    void Awake() {
//        castHits = new List<CastHit>();
//        nodes = CreateContextMap(resolution);
//        controller = GetComponent<SpaceCraftController>();
//        highest = nodes[0];
//    }

//    void Update() {
//        //        ClearNodes();
//        //        AlternateGatherDangerInfo();
//        //        SaturateTarget();
//        //        SaturateDanger();
//        //        FindHighestNode();
//        //        Move();
//    }


//    //todo turning needs to be tighter -- instead of using node values as input maps, 
//    //use them to compute a go-to vector

//    //build a context map
//    //score the map
//    //go to highest node

//    public void Move() {

//        float rollInput, yawInput, pitchInput;
//        var halfres = ((resolution * resolution) - 1) / 2;
//        //todo && no danger in target direction
//        if (highest.index == halfres || Vector3.Distance(targetPosition, transform.position) < controller.ForwardSpeed * 1.25f) {
//            Vector3 localTarget = transform.InverseTransformPoint(targetPosition);
//            float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
//            float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);
//            float targetAngleRoll = targetAngleYaw;

//            pitchInput = targetAnglePitch;
//            yawInput = targetAngleYaw;
//            rollInput = -(controller.RollAngle - targetAngleRoll);

//        } else {
//            yawInput = Mathf.Lerp(-1, 1, highest.x / (float)(resolution - 1));
//            pitchInput = -Mathf.Lerp(-1, 1, highest.y / (float)(resolution - 1));
//            rollInput = -(controller.RollAngle - yawInput);
//        }

//        controller.Move(rollInput, pitchInput, yawInput, 1);
//    }

//    private void ClearNodes() {
//        for (int i = 0; i < nodes.Count; i++) {
//            nodes[i].score = 0f;
//        }
//        castHits.Clear();
//    }

//    private void FindHighestNode() {
//        float max = float.MinValue;
//        float min = float.MaxValue;

//        for (int i = 0; i < nodes.Count; i++) {
//            float score = nodes[i].score;
//            if (score > max) {
//                max = score;
//                highest = nodes[i];
//            } else if (score < min) {
//                min = score;
//            }
//        }

//        var dots = targetingSystem.DotsToTarget();
//        int index = GetIndex(dots.x, dots.y, resolution);
//        var targetNode = nodes[index];

//        if (targetNode == highest) return;
//        if (targetNode.score == highest.score) {
//            highest = targetNode;
//        }
//    }

//    void AlternateGatherDangerInfo() {
//        Vector3 topLeft = new Vector3(-1, 1, 0);
//        Vector3 topRight = new Vector3(1, 1, 0);
//        Vector3 bottomLeft = new Vector3(-1, -1, 0);
//        Vector3 bottomRight = new Vector3(1, -1, 0);

//        RaycastHit[] hits = Physics.SphereCastAll(transform.position, sphereCastRadius, transform.forward, controller.ForwardSpeed * 1.5f);
//        for (int i = 0; i < hits.Length; i++) {
//            if (hits[i].transform == targetingSystem.target) continue;
//            if (hits[i].transform == transform) continue;

//            var entity = hits[i].transform.GetComponent<Entity>();
//            var contactPoint = hits[i].point;
//            var toContactPoint = contactPoint - transform.position;
//            int casts = (int)entity.size;
//            castHits.Add(new CastHit(contactPoint, toContactPoint.normalized, toContactPoint.sqrMagnitude, Color.red, true));

//            for (int j = 1; j <= casts; j++) {
//                float offset = (int)entity.size * j;
//                if (j % 2 == 0) {
//                    ProcessContactDirection(entity, Vector3.up, contactPoint, offset);
//                    ProcessContactDirection(entity, Vector3.down, contactPoint, offset);
//                    ProcessContactDirection(entity, Vector3.left, contactPoint, offset);
//                    ProcessContactDirection(entity, Vector3.right, contactPoint, offset);
//                } else {
//                    ProcessContactDirection(entity, topLeft, contactPoint, offset);
//                    ProcessContactDirection(entity, topRight, contactPoint, offset);
//                    ProcessContactDirection(entity, bottomLeft, contactPoint, offset);
//                    ProcessContactDirection(entity, bottomRight, contactPoint, offset);
//                }
//            }
//        }
//    }

//    private void ProcessContactDirection(Entity entity, Vector3 offsetPoint, Vector3 contactPoint, float offset) {
//        Vector3 rotatedPoint = transform.rotation * (offsetPoint * offset);
//        Vector3 toOffsetPoint = ((rotatedPoint + contactPoint) - transform.position).normalized;
//        var intersection = entity.GetRayIntersectionPoint(transform.position, toOffsetPoint);
//        if (intersection != null) {
//            float squareDistance = ((Vector3)intersection - transform.position).sqrMagnitude;
//            castHits.Add(new CastHit((Vector3)intersection, toOffsetPoint, squareDistance, Color.white));
//            if (drawCastLines) {
//                Debug.DrawLine(transform.position, transform.position + toOffsetPoint * 10f, Color.green);
//            }
//        }
//    }

//    void OnDrawGizmos() {
//        if (controller != null) {
//            Gizmos.color = Color.magenta;
//            Gizmos.DrawWireSphere(transform.position + (transform.forward * controller.ForwardSpeed * 1.5f), 5f);
//        }

//        if (castHits == null || !drawCastHits) return;
//        Gizmos.color = Color.blue;
//        foreach (var hit in castHits) {
//            Gizmos.color = hit.color;
//            Gizmos.DrawWireSphere(hit.point, 0.25f);
//        }
//    }

//    private void SaturateTarget() {
//        var dots = targetingSystem.DotsToTarget();
//        int index = GetIndex(dots.x, dots.y, resolution);
//        Saturate(index, 1f, 0.75f);
//    }

//    private void SaturateDanger() {
//        float squareVelocity = GetComponent<Rigidbody>().velocity.sqrMagnitude;
//        foreach (CastHit hit in castHits) {
//            Vector3 dots = targetingSystem.DotsToNormalizedVector(hit.direction);
//            int index = GetIndex(dots.x, dots.y, resolution);
//            float strength = 0.5f;
//            float dropOff = 0.5f;
//            if (hit.primary) {
//                strength = 1f;
//                dropOff = 0.75f;
//            }
//            float distanceScale = 1 - (hit.squareDistance / squareVelocity);
//            if (distanceScale > 0.9f) distanceScale = 1.25f;
//            strength *= dots.z;
//            strength *= distanceScale;
//            if (strength > 0) strength = -strength;
//            Saturate(index, strength, dropOff);
//        }

//    }

//    private void Saturate(int index, float strength, float dropOff) {
//        SteeringNode origin = nodes[index];
//        for (int i = 0; i < nodes.Count; i++) {
//            SteeringNode node = nodes[i];
//            if (node == origin) {
//                node.score += strength;
//            } else {
//                var xDiff = Math.Abs(origin.x - node.x);
//                var yDiff = Math.Abs(origin.y - node.y);

//                float xScale = Mathf.Pow(dropOff, xDiff);
//                if (xScale == 0) xScale = 1;
//                node.score += strength * xScale * 0.5f;

//                float yScale = Mathf.Pow(dropOff, yDiff);
//                if (yScale == 0) yScale = 1;
//                node.score += strength * yScale * 0.5f;
//            }
//        }
//    }

//    public static int GetIndex(float x, float y, int resolution) {
//        int xIndex, yIndex;
//        if (x == 1) {
//            xIndex = resolution - 1;
//        } else {
//            float xPercent = (x + 1) * 0.5f;
//            xIndex = (int)((resolution - 1) * xPercent);
//        }
//        if (y == 1) {
//            yIndex = resolution - 1;
//        } else {
//            float yPercent = (y + 1) * 0.5f;
//            yIndex = (int)((resolution - 1) * yPercent);
//        }

//        return yIndex * resolution + xIndex;
//    }

//    public static List<SteeringNode> CreateContextMap(int resolution) {
//        var nodes = new List<SteeringNode>(resolution * resolution);
//        for (int i = 0; i < resolution * resolution; i++) {
//            nodes.Add(new SteeringNode(i, i % resolution, i / resolution));
//        }
//        return nodes;
//    }

//}