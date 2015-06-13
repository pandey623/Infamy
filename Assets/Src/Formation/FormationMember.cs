using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//public class FormationMember : MonoBehaviour {
//    public bool isLeader = false;
//    public bool IsLeader {
//        get { return isLeader; }
//    }

//    public Formation formation;
//    private float cachedAnglarDrag;
//    private Entity self;

//    void Start() {
//        self = GetComponent<Entity>();
//        //temporary
//        if (isLeader) {
//            List<Entity> entities = RequestFormation();
//            for (int i = 0; i < entities.Count; i++) {

//            }
//        }
//    }

//    public List<Entity> RequestFormation() {
//        List<Entity> list = FactionManager.GetFaction(self.factionId).friendlies;
//        List<Entity> members = new List<Entity>();
//        for (int i = 0; i < list.Count; i++) {
//            //todo put a call to something like CanJoinFormation() here
//            members.Add(list[i]);
//        }
//        return members;
//    }

//}

//[CustomEditor(typeof(FormationMember))]
//public class FormationMemberEditor : Editor {
//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//        FormationMember member = target as FormationMember;

//    }
//}



//while assembling, leader should move a reduced speed, at most 75% of slowest unit in formation
//get behind your slot
//get some space
//turn to slot
//move to slot when within some threshold, negate all thrust force
//set higher drag
//lerp position to slot position at max speed
//when position reached -- lock it in, alert formation that you joined