//using UnityEngine;
//using System.Collections.Generic;
//
//public class Formation : MonoBehaviour {
//
//    public FactionId factionId;
//    public List<FormationElement> members;
//    public FormationElement leader;
//    private Transform[] slots;
//
//    void Awake() {
//        members = new List<FormationElement>();
//    }
//
//    void Start() {
//        slots = new Transform[transform.childCount];
//        for (int i = 0; i < slots.Length; i++) {
//            slots[i] = transform.GetChild(i);
//        }
//    }
//
//    void Update() {
//        if (leader != null) {
//            transform.position = leader.transform.position;
//            transform.rotation = leader.transform.rotation;
//        }
//    }
//
//    public FormationNode GetNextSlot() {
//        return null;
//    }
//
//    public void Join(FormationElement member) {
//        if (member.isLeader) {
//
//        } else {
//            members.Add(member);
//        }
//    }
//
//    public bool AllMembersJoined() {
//        for (int i = 0; i < members.Count; i++) {
//            if (members[i].state != FormationElement.States.Joined) {
//                return false;
//            }
//        }
//        return true;
//    }
//
//    public void Leave() {
//
//    }
//
//    public void Break() {
//
//    }
//
//    public void SetLeader() {
//
//    }
//
//    public Entity GetLeader() {
//        return null;
//    }
//
//    public int Capacity {
//        get { return slots.Length; }
//    }
//
//    //public int Availiblity {
//    //    get { return slots.Length - taken; }
//    //}
//
//    public void Reflow() {
//
//    }
//}
