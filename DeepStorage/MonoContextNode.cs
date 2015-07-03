using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class MonoContextNode : MonoBehaviour {

    public Vector3 toInterestTarget;
    public Vector3 toDangerTarget;
    public Vector3 rawInterestVector;
    public Vector3 rawDangerVector;
    public List<string> contributions = new List<string>();

    public Gradient interestGradient;
    public Gradient dangerGradient;
    public bool selected;
    public float interestStrength;
    public float dangerStrength;
    public Vector3 id;

    public List<MonoContextNode> neighbors = new List<MonoContextNode>();
    private float trackId;
    private MeshRenderer meshRenderer;
    public Vector3 offset;

    public float debug_interestChange = 0.2f;
    public bool additiveMode = false;
    public float rearNodePenalty = 0f;

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        Reset();
    }

    void Update() {

        if (selected) {
            meshRenderer.material.color = Color.magenta;
        } else if(interestStrength > dangerStrength) {
            meshRenderer.material.color = interestGradient.Evaluate(interestStrength - dangerStrength);
        } else if (interestStrength < dangerStrength) {
            meshRenderer.material.color = dangerGradient.Evaluate(dangerStrength - interestStrength);

        } else {
            meshRenderer.material.color = Color.white;
        }
    }

    public void AddInterest(string contributor, Vector3 toTarget, float strength, float dropOff = 0.4f) {
        contributions.Add(contributor + " : " + strength);
        if (additiveMode) {
            interestStrength += strength;
            toInterestTarget = toTarget;
            // we add influence to neighbors if the added interest score > current interest score
            //saturating this would be likely to have a pretty small effect, might as well save some cpu and not do it
            trackId = Random.Range(-9999, 9999); //generate a somewhat unique tracking id to ensure nodes are not visited twice.
            foreach (var node in neighbors) {
                node.SaturateNeighborInterest(contributor, trackId, strength * dropOff);
            }
        } else if (strength > interestStrength) {
      //      if (interestStrength != 0f) {
    //            toInterestTarget = toTarget;
             //   var strengthDifference = strength - interestStrength;
             //   var scaledCurrentInterest = weightedInterestedVector * (1 - strengthDifference);
             //   weightedInterestedVector =  (scaledCurrentInterest + toTarget) / 2;
  //          } else {
                //rawInterestVector = toTarget;

//            }
            interestStrength = strength;
            toInterestTarget = toTarget;
            // we add influence to neighbors if the added interest score > current interest score
            //saturating this would be likely to have a pretty small effect, might as well save some cpu and not do it
            trackId = Random.Range(-9999, 9999); //generate a somewhat unique tracking id to ensure nodes are not visited twice.
            foreach (var node in neighbors) {
                node.SaturateNeighborInterest(contributor, trackId, strength * dropOff);
            }
        }
        //else {
            //work new vector into our weighted average but dont set any raw values or saturate input strength to neighbors
            //var strengthDifference = interestStrength - strength;
            //var scaledToTarget = weightedInterestedVector * (1 - strengthDifference);
            //weightedInterestedVector = (weightedInterestedVector + scaledToTarget) / 2;
      //  }
    }

    //todo -- maybe do a slight scaling here to bend the node's influence slightly toward the saturating neighbor -- see comment on GetAggregateVector
    private void SaturateNeighborInterest(string item, float visitedId, float strength) {
        if (visitedId == trackId) return;
        trackId = visitedId;
        contributions.Add(item + "_saturated : " + strength);
        if (additiveMode) {
            interestStrength += strength;
        } else if (strength > interestStrength) {
            //if new strength is larger than current interest strength, assign the new strength but do not change 
            //the direction vector since we dont want to point this node towards its neighbor we just want this node
            //to have more influence while retaining its current direction. 
            interestStrength = strength;
        }        
    }

    public void AddDanger(string contributor, Vector3 toTarget, float strength, float dropOff = 0.5f) {
        contributions.Add(contributor + " : -" + strength);
        if (additiveMode) {
            dangerStrength += strength;
            toDangerTarget = toTarget;
            trackId = Random.Range(-9999, 9999); //generate a somewhat unique tracking id to ensure nodes are not visited twice.
            foreach (var node in neighbors) {
                node.SaturateNeighborDanger(contributor, trackId, strength * dropOff);
            }
        } else if (strength > dangerStrength) {
            dangerStrength = strength;
            toDangerTarget = toTarget;
            trackId = Random.Range(-9999, 9999); //generate a somewhat unique tracking id to ensure nodes are not visited twice.
            foreach (var node in neighbors) {
                //todo -- this doesnt work right now, figure it out later
                if(!(node.id.x == id.x || node.id.y == id.y || node.id.z == id.z)) {
                    strength = strength * 0.75f;
                    node.contributions.Add("Diagonal_saturated");
                }
                node.SaturateNeighborDanger(contributor, trackId, strength * dropOff);
            }
        }
    }

    private void SaturateNeighborDanger(string contributor, float visitedId, float strength) {
        if (visitedId == trackId) return;
        trackId = visitedId;
        contributions.Add(contributor + "_saturated : -" + strength);
        if (additiveMode) {
            dangerStrength += strength;
        } else if (strength > dangerStrength) {
            dangerStrength = strength;
        }
    }

    public float GetAggregateInterest() {
        float interest = interestStrength;
        for (var i = 0; i < neighbors.Count; i++) {
            interest += neighbors[i].interestStrength;
        }
        return interest;
    }

    public float GetAggregateDanger() {
        float danger = dangerStrength;
        for (var i = 0; i < neighbors.Count; i++) {
            danger += neighbors[i].dangerStrength;
        }
        return danger;
    }


    //todo this needs work still -- right now this isnt useful because the direction doesnt truly point 
    //at anything which will induce wobbling I think. For now just use raw toTarget as desired direction
    //if strength is larger than last strength
    //public Vector3 GetAggregateInterestVector() {
        //Vector3 aggregate = weightedInterestedVector;
        //float localSqrMagnitude = weightedInterestedVector.sqrMagnitude;
        //for (var i = 0; i < neighbors.Count; i++) {
        //    var neighbor = neighbors[i];
        //    if (localSqrMagnitude > neighbor.weightedInterestedVector.sqrMagnitude) {
        //        var strengthDifference = interestStrength - neighbor.interestStrength;
        //        var scaledNeighbor = neighbor.weightedInterestedVector * (1 - strengthDifference);
        //        aggregate = (scaledNeighbor + aggregate) / 2;
        //    } 
        //    //else {
        //    //    var strengthDifference = neighbor.interestStrength - interestStrength;
        //    //    var scaledSelf = 
        //    //}
        //}
        //return aggregate;
    //}
    public void Reset() {
        interestStrength = 0;
        dangerStrength = 0;

        rawInterestVector = id - offset;
        rawDangerVector = id - offset;
        toInterestTarget = id - offset;
        toDangerTarget = id - offset;
        contributions.Clear();
        if (rawDangerVector.z < 0) {
            dangerStrength += rearNodePenalty;
        }
    }
}

[CustomEditor(typeof(MonoContextNode))]
public class MonoContextNodeEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MonoContextNode node = (MonoContextNode)target;
        if (GUILayout.Button("Add Interest")) {
            node.AddInterest("Click" + node.transform.name, Quaternion.Euler(UnityEngine.Random.onUnitSphere * 3) * node.rawInterestVector, node.debug_interestChange);
        }
    }
}