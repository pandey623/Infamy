using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//todo consider having AIGoals extend AIAttackGoal which can implement things related to 
//target selection & prioritization

public class AIState_Attack : AIState {

    public AIState_Attack(AIPilot pilot) : base("Attack", pilot) {
        substates = GetSubstates(this);
    }

    public bool freelySelectTargets = true; //move this to goal or introduce an api

    public float DISTANCE_THRESHOLD_SQR = 150f * 150f;
    public float radius = 8f;

    protected struct TargetScore {
        public Entity entity;
        public float score;

        public TargetScore(Entity entity, float score) {
            this.entity = entity;
            this.score = score;
        }
    }

    public void SelectTarget() {
        /*  List<Entity> targetOptions = entity.sensorSystem.GetHostiles();
          List<TargetScore> scoreList = new List<TargetScore>(3);
          Debug.Log(targetOptions.Count);
          for (int i = 0; i < targetOptions.Count; i++) {
              Entity option = targetOptions[i];
              float distanceSqr = (transform.position - option.transform.position).sqrMagnitude;
              float score = 0f;
              if (IsGoalTarget(option)) score += 1;
              if (sensorSystem.target == option) score += 1;
              if (DISTANCE_THRESHOLD_SQR < distanceSqr) score -= 1;
              if (IsEntityThreateningToSelf(option)) score += 1;
              if (IsEntityThreateningToAllies(option)) score += 1;
              //todo factor in integrity
              if (weaponSystem.InPrimaryRange(option)) score += 1;
              scoreList.Add(new TargetScore(option, score));
          }*/
        //temp
        entity.sensorSystem.AquireTarget(sensorSystem.GetNearestHostile());
        //        entity.sensorSystem.AquireTarget(target);
    }

//    protected bool IsEntityThreateningToSelf(Entity other) {
//        return other.piloted;
//    }
//
//    protected bool IsEntityThreateningToAllies(Entity other) {
//        return other.piloted;
//    }

    public bool IsEntityThreateningToGoal(Entity other) {
        return false; //for escort / defend 
    }

    protected bool IsGoalTarget(Entity entity) {
        return true;
    }

    public override void OnEnter() {
        /* if (activeGoal.aiSubstate != null) {
             SetSubstate(activeGoal.aiSubstate);
         } else {
             UpdateActiveSubstate();
         }*/
        SelectTarget();
        activeSubstate = substates[2]; //chase
        activeSubstate.Enter();
    }

    protected void MaybeSelectNewTarget() {
        if (sensorSystem.Target == null || pilot.CanSelectNewTarget) {
            SelectTarget();
        } //else if goal changed
    }

    public override void Update() {
        //assert target somehow
        if (sensorSystem.Target == null) {
            Debug.Log("No target, figure out what to do");
            return;
        }

        SubstateStatus status = activeSubstate.Update();
        if (status != SubstateStatus.Running) {
            UpdateActiveSubstate();
        }

     //   Debug.Log(sensorSystem.ForwardDotToTarget + ", " + weaponSystem.InPrimaryRange(sensorSystem.target));

        if (weaponSystem.InPrimaryRange(sensorSystem.Target) && sensorSystem.ForwardDotToTarget >= 0.995f) {
            weaponSystem.Fire(); // todo add target
        }
    }

    public void UpdateActiveSubstate() {
        activeSubstate.Exit();
        float distance = sensorSystem.DistanceToTarget;
        if (distance < 30f) {
            Debug.Log("Getting Space");
            activeSubstate = substates[0];
        } else {
            Debug.Log("Resuming Attack");
            activeSubstate = substates[2];
        }
        activeSubstate.Enter();
    }

    public static AISubstate[] GetSubstates(AIState parent) {
        AISubstate[] substates = new AISubstate[3];
        substates[0] = new AISubstate_GetSpace(parent);
        substates[1] = new AISubstate_GetBehind(parent);
        substates[2] = new AISubstate_Chase(parent); //maybe add missile lock state, maybe make it parallel
        //substates[3] = new AISubstate_AttackEvade --> if shit hits the fan can add a straight up evasion goal, cancelling attack. 
        //if threat is tolerable blend attack vector with evasion vector. contains substates
        //substates[4] = new AISubstate_AttackAvoidCollision --> similar to AttackEvade 
        return substates;
    }
}