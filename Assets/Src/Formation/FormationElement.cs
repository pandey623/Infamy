//using UnityEngine;
//using System.Collections;
//
//public class FormationElement : MonoBehaviour {
//    public Transform target;
//    public Formation formation;
//    private SpaceCraftController controller;
//    private SmallCraftSteering steering;
//    public States state;
//    private FormationNode slot;
//
//    public float tension = 2.5f;
//    public bool isLeader = false;
//
//    void Start () {
//        controller = GetComponent<SpaceCraftController>();
//        steering = GetComponent<SmallCraftSteering>();
//        JoinFormation();
//    }
//
//    public enum States {
//        Joining, Leaving, Joined, Leading
//    }
//
//    //void Update() {
//    //    switch (state) {
//    //        case States.Joining:
//    //            break;
//    //        case States.Joined:
//    //            break;
//    //        case States.Leading:
//    //            bool allJoined = formation.AllMembersJoined();
//    //            if (allJoined) {
//    //                Debug.Log("We got everybody");
//    //                //slow down speed & turn rate & try flying straight & away from danger
//    //            }
//    //            break;
//    //    }
//    //}
//	
//	void FixedUpdate () {
//
//        switch (state) {
//            case States.Joining:
//                //GetComponent<TargetingSystem>().target = slot ;
//                break;
//            case States.Joined:
//                if (Vector3.Dot(transform.forward, target.forward) < 0.85f) {
//
//                    GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(transform.rotation, target.rotation, 2f * Time.deltaTime));
//
//                } else {
//                    Vector3 localTarget = transform.InverseTransformPoint(target.position);
//                    float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
//                    float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);
//                    float targetAngleRoll = targetAngleYaw;
//
//                    float pitchInput = targetAnglePitch;
//                    float yawInput = targetAngleYaw;
//                    float rollInput = -(controller.GetRollAngle() - targetAngleRoll);
//                    controller.Move(rollInput, pitchInput, yawInput, 0f);
//                }
//                transform.position = target.position - (target.forward * tension);
//                break;
//            case States.Leading:
//                break;
//        }
//
//        if (Vector3.Dot(transform.forward, target.forward) < 0.85f) {
//
//            GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(transform.rotation, target.rotation, 2f * Time.deltaTime));
//
//        } else {
//            Vector3 localTarget = transform.InverseTransformPoint(target.position);
//            float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);
//            float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);
//            float targetAngleRoll = targetAngleYaw;
//
//            float pitchInput = targetAnglePitch;
//            float yawInput = targetAngleYaw;
//            float rollInput = -(controller.GetRollAngle() - targetAngleRoll);
//            controller.Move(rollInput, pitchInput, yawInput, 0f);
//        }
//    }
//
//    //todo start thinking about higher level AI controls, this is totally temporary
//    void JoinFormation() {
//        if (!isLeader) {
//            slot = formation.GetNextSlot();
//            state = States.Joining;
//        } else {
//            formation.transform.parent = transform;
//            formation.transform.localPosition = Vector3.zero;
//            formation.transform.localRotation = Quaternion.identity;
//            state = States.Leading;
//        }
//        formation.Join(this);
//    }
//}
//
////JoinFormation
//        
//    //Steer behind slot 3x max velcity
//    
//
////Formation
//    //Leader
//    //Members
//    //Slot
//
////Ship
//    //FormationMember -> Leader | Slot
//
//
//
///*
// SteeringController
// * yaw, pitch, roll
// * speed, slip
// * Approach(arrivalSpeedMin, arrivalSpeedMax)
// * Follow()
// * Intercept()
// * Evade()
// * FormUp()
// * Dock()
// * Avoid(Entity) --> Prefers to stay away from this entity
// * 
// * 
// * ShipData
// *  roll, pitch, yaw
// *  speed
// *  slip
// */