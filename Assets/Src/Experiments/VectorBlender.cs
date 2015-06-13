using UnityEngine;
using System.Collections;

public class VectorBlender : MonoBehaviour {

    [Header("Targets")]
    public Transform target1;       //Primary target
    public Transform target2;       //Secondary target
    public Transform target3;       //Tertiary target

    [Header("Target Strengths")]
    public float target1Str = 1f;           //Primary Vector
    public float target2Str = 0.5f;         //Neighboring Vector
    public float target3Str = 0.2f;         //Neighbor's Neighbor -- Scale this one by how far it is from the primary vector

    [Header("Options")]
    public int target3DistanceModifer = 2;  //distance penalty -- divide target3's strength by this
    public bool useDistantNeighbor = false;
    public float drawLengthModifier = 5f;
    public float interestModifier = 1f;

    public string color1 = "Cyan = Weighted Average";
    public string color2 = "Yellow = True Average";
    public bool useRawStrength = true;

    void Update() {
        if (useRawStrength) {
            UpdateWithRawStrength();
        } else if (useDistantNeighbor) {
            UpdateWithDistantNeighbor();
        } else {
            UpdateWithLocalNeighbors();
        }
    }

    void UpdateWithRawStrength() {
        var toTarget1 = (target1.position - transform.position).normalized * target1Str;
        var toTarget2 = (target2.position - transform.position).normalized * target2Str;
        var toTarget3 = (target3.position - transform.position).normalized * target3Str;

        var trueAverage = ((toTarget1 + toTarget2 + toTarget3) / 3).normalized;
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget1, Color.blue);  //to target 1
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget2, Color.red);   //to target 2
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget3, Color.white); //to target 3
        DrawArrow.ForDebug(transform.position, drawLengthModifier * trueAverage, Color.yellow); //True average no weights
    }

    //Note: Normalization doesn't matter, just doing it here for uniform drawing.
    void UpdateWithLocalNeighbors() {
        var toTarget1 = (target1.position - transform.position).normalized;
        var toTarget2 = (target2.position - transform.position).normalized;
        var toTarget3 = (target3.position - transform.position).normalized;

        var strengthDiffBetween1and2 = target1Str - target2Str;
        var strengthDiffBetween1and3 = target1Str - target3Str;

        var toTarget1Scaled = toTarget1;
        var toTarget2Scaled = toTarget2 * (1 - strengthDiffBetween1and2) * interestModifier;
        var toTarget3Scaled = toTarget3 * (1 - strengthDiffBetween1and3) * interestModifier;

        var trueAverage = ((toTarget1 + toTarget2 + toTarget3) / 3).normalized;
        var weightedAverage = ((toTarget1Scaled + toTarget2Scaled + toTarget3Scaled) / 3).normalized;

        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget1, Color.blue);  //to target 1
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget2, Color.red);   //to target 2
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget3, Color.white); //to target 3
        DrawArrow.ForDebug(transform.position, drawLengthModifier * trueAverage, Color.yellow); //True average no weights
        DrawArrow.ForDebug(transform.position, drawLengthModifier * weightedAverage, Color.cyan);   //Weighted Average
    }

    //Note: Normalization doesn't matter, just doing it here for uniform drawing.
    void UpdateWithDistantNeighbor() {
        var toTarget1 = (target1.position - transform.position).normalized;
        var toTarget2 = (target2.position - transform.position).normalized;
        var toTarget3 = (target3.position - transform.position).normalized;

        var strengthDiffBetween1and2 = target1Str - target2Str;
        var strengthDiffBetween1and3 = target1Str - target3Str;

        var toTarget1Scaled = toTarget1;
        var toTarget2Scaled = toTarget2 * (1 - strengthDiffBetween1and2) * interestModifier;
        var toTarget3Scaled = toTarget3 * ((1 - strengthDiffBetween1and3) / target3DistanceModifer) * interestModifier;

        var trueAverage = ((toTarget1.normalized + toTarget2.normalized + toTarget3.normalized) / 3).normalized;
        var weightedAverage = ((toTarget1Scaled + toTarget2Scaled + toTarget3Scaled) / 3).normalized;

        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget1, Color.blue);  //to target 1
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget2, Color.red);   //to target 2
        DrawArrow.ForDebug(transform.position, drawLengthModifier * toTarget3, Color.white); //to target 3
        DrawArrow.ForDebug(transform.position, drawLengthModifier * trueAverage, Color.yellow); //True average no weights
        DrawArrow.ForDebug(transform.position, drawLengthModifier * weightedAverage, Color.cyan);   //Weighted Average
    }
}
