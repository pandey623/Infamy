using System;
using UnityEngine;
using System.Collections.Generic;

/*
ShipsDestroyed = new ShipsDestroyedCondition("1", "2");
TimeElapsed = new TimeElapsedCondition(100f);

When(ShipsDestoyed | TimeElapsed).Then(() => {
    SetGoal()
    
});

*/

//public class TimeSinceCondition : NumericComparisonCondition {
//
//    //    public TimeSinceCondition(string evtName, float time) : base(this, ) {
//    //        
//    //    }

    //public override float GetValue() {
//      return Timer.missionTime - MissionLog.EventTimestamp("Evt");
//}
//}
//When(TimeSince("Event") + 5f > TimeSince("Other") - TotalShipCount).Then(DoSomething);
//todo implement NumericComparisonCondition and ArithmeticCondition
//When(TimeSinceMissionStart + TimeSinceShipHit > TimesShipHit).Then()


public class TimeElapsedCondition : Condition {
    private float time;
    private float start;

    public TimeElapsedCondition(float time) {
        this.time = time;
    }

    public override bool Eval() {
        //        var t = new NumericComparisonCondition(this, NumericComparisonCondition.ComparisonOperator.GreaterThan, 5f);
        //        var x = t > 5;
        return false;
    }
}

public partial class MissionScript {
    public TimeElapsedCondition TimeElapsed(float time) {
        return new TimeElapsedCondition(time);
    }

    public TimeElapsedCondition TimeSinceEvent(string evtName, float time) {
        return new TimeElapsedCondition(time);
    }
}
//TimeSince("MissionEvent") > TimeSince("MissionEvent2")
//DestroyedCount("ship1", "ship2") > SomeFnReturningAnInt()


//when do these run?
//on events?
//periodically?
//every frame?

//major game events 
//EntityArrived
//EntityDeparted
//EntityDocked
//EntityDestroyed
//EntityDisabled
//EntityFactionChanged
//MissionPhaseChanged
//Custom Mission Events


//Update Times -- Run on configurable cycle
//             -- Custom Scheduler can be set per script
//             -- Defaults to 10 times per frame

//While --> once per frame?
//Every --> once per `arg` seconds (min 1 frame)
//Until --> once per frame
//If --> runs when first encountered, not again
//Else --> runs when first encountered, not again
//ElseIf --> runs when first encountered, not again
//When --> Every Major Mission Event, 10 times per second. Does not repeat
//WhenEver --> Every Major Mission Event, 10 times per second. Repeats 
//If(True).Do().ElseIf().Do().ElseIf().Do().Else(Action);

public partial class MissionScript {
    [NonSerialized]
    protected List<ConditionEvaluator> evaluators;


    public TrueCondition True;
    public FalseCondition False;

    public MissionScript() {
        evaluators = new List<ConditionEvaluator>();
        True = Condition.True;
        False = Condition.False;
    }
    
    //TriggerEvaluator.Reset(); -- Everything starts over, If/Else/ElseIf will run again
    //TriggerEvaluator.Pause(); -- Everything is paused
    //TriggerEvaluator.Resume(); -- Everything resumes

    public void Triggers() {
        //Whenever(MissionPhaseIs("PhaseName"), () => { EnableGroup("A") });
        //Group("A", () => {});
        //Group("B", () => {});
        //DisableGroup("A");
        //EnableGroup("B");
        //ResetGroup("C");
        
        When(True).Then(() => {
            When(True | !False).Then(SomeAction); //disable / enable?
            When(MissionEventTriggered("Alpha Warp In")).Then(SomeAction);
            //When(EntityDestroyed("Dauntless") & TimeSinceDestroyed("Dauntless") >= 10f)
            While(True).Do(SomeAction); //start / stopable?
            Until(True).Do(SomeAction); //start / stopable?
//            If(True).Do(SomeAction)
//            .Else(SomeAction)
//            .ElseIf(True).Do(SomeAction)
        });
    }

    public void SomeAction() { }

    //todo this should be treated differently then When/Then
    public ConditionEvaluator Until(Condition condition) {
        ConditionEvaluator evaluator = new ConditionEvaluator(condition);
        evaluators.Add(evaluator);
        return evaluator;
    }

    //todo this should be treated differently then When/Then
    public ConditionEvaluator While(Condition condition) {
        ConditionEvaluator evaluator = new ConditionEvaluator(condition);
        evaluators.Add(evaluator);
        return evaluator;
    }

    //Runs Do() / ActionChain every `time` seconds
    public ConditionEvaluator Every(float time) {
        ConditionEvaluator evaluator = new ConditionEvaluator(null);
        evaluators.Add(evaluator);
        return evaluator;
    }

    //Acts as a trigger, continuously observes conditions. Executes Then() once
    public ConditionEvaluator When(Condition condition) {
        ConditionEvaluator evaluator = new ConditionEvaluator(condition);
        evaluators.Add(evaluator);
        return evaluator;
    }
}

public enum ConditionEvaluatorState {
   
}

public class ConditionEvaluator {
    private Condition condition;
    public delegate void Del();

    public ConditionEvaluator(Condition condition) {
        this.condition = condition;
    }

    public void Evaluate() {
        
    }

    public void Do(Del del) {
        
    }

    public void Then(Del del) {

    }

}