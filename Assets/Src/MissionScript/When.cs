using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/*
ShipsDestroyed = new ShipsDestroyedCondition("1", "2");
TimeElapsed = new TimeElapsedCondition(100f);

When(ShipsDestoyed | TimeElapsed).Then(() => {
    SetGoal()
    
});

*/

//public class TimeSinceCondition : NumericCondition {
//
//    //    public TimeSinceCondition(string evtName, float time) : base(this, ) {
//    //        
//    //    }
//}

//todo implement NumericComparisonCondition and ArithmeticCondition
//When(TimeSinceMissionStart + TimeSinceShipHit > TimesShipHit).Then()

public class NumericCondition : Condition {
    public enum Operator {
        None, GreaterThan, GreaterThanEqualTo, LessThan, LessThanEqualTo,
        Equal, NotEqual, Plus, Minus, Divide, Modulus, Multiply
    }

    protected float value;
    protected Operator op;
    protected NumericCondition condition1;
    protected NumericCondition condition2;

    public NumericCondition(NumericCondition c1, Operator op, NumericCondition c2) {
        this.condition1 = c1;
        this.condition2 = c2;
        this.op = op;
    }

    public NumericCondition(NumericCondition c1, Operator op, float value) {
        this.condition1 = c1;
        this.op = op;
        this.value = value;
    }

    public virtual float GetValue() {
        return 0f;
    }

    public override bool Eval() {
        if (op == Operator.None) {
            return false;
        }

        if (condition2 != null) {
            float c1Value = condition1.GetValue();
            float c2Value = condition2.GetValue();
            switch (op) {
                case Operator.GreaterThan:
                    return c1Value > c2Value;
                case Operator.LessThan:
                    return c1Value < c2Value;
                case Operator.GreaterThanEqualTo:
                    return c1Value >= c2Value;
                case Operator.LessThanEqualTo:
                    return c1Value <= c2Value;
                case Operator.Equal:
                    return c1Value == c2Value;
                case Operator.NotEqual:
                    return c1Value != c2Value;
                case Operator.None:
                    return false;
            }
        } else {

            float c1Value = condition1.GetValue();
            switch (op) {
                case Operator.GreaterThan:
                    return c1Value > value;
                case Operator.LessThan:
                    return c1Value < value;
                case Operator.GreaterThanEqualTo:
                    return c1Value >= value;
                case Operator.LessThanEqualTo:
                    return c1Value <= value;
                case Operator.Equal:
                    return c1Value == value;
                case Operator.NotEqual:
                    return c1Value != value;
                case Operator.None:
                    return false;
            }
        }
        return false;
    }

    public static NumericCondition operator >(NumericCondition condition, float value) {
        return new NumericCondition(condition, Operator.GreaterThan, value);
    }

    public static NumericCondition operator >(NumericCondition c1, NumericCondition c2) {
        return new NumericCondition(c1, Operator.GreaterThan, c2);
    }

    public static NumericCondition operator <(NumericCondition condition, float value) {
        return new NumericCondition(condition, Operator.LessThan, value);
    }

    public static NumericCondition operator <(NumericCondition c1, NumericCondition c2) {
        return new NumericCondition(c1, Operator.LessThan, c2);
    }
}

public class TimeElapsedCondition : Condition {
    private float time;
    private float start;

    public TimeElapsedCondition(float time) {
        this.time = time;
    }

    public override bool Eval() {
        //        var t = new NumericCondition(this, NumericCondition.Operator.GreaterThan, 5f);
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

public partial class MissionScript {
    [NonSerialized]
    protected List<ConditionEvaluator> evaluators;

    private static TrueCondition TrueCondition = new TrueCondition();
    private static FalseCondition FalseCondition = new FalseCondition();

    public TrueCondition True;
    public FalseCondition False;

    public MissionScript() {
        evaluators = new List<ConditionEvaluator>();
        True = MissionScript.TrueCondition;
        False = MissionScript.FalseCondition;
    }

    public void Triggers() {
        When(True).Then(() => {
            When(True | !False).Then(SomeAction); //disable / enable?
            When(MissionEventTriggered("Alpha Warp In")).Then(SomeAction);
            //When(EntityDestroyed("Dauntless") & TimeSinceDestroyed("Dauntless") >= 10f)
            While(True).Do(SomeAction); //start / stopable?
            Until(True).Do(SomeAction); //start / stopable?
        });

        //        WhileDelay(MissionPhase("Second"), 1f).Do(() => {
        //            
        //        });
    }

    public void SomeAction() { }

    //todo this should be treated differently then When/Then
    public ConditionEvaluator Until(Condition condition) {
        ConditionEvaluator evaluator = new ConditionEvaluator(condition);
        evaluators.Add(evaluator);
        return evaluator;
    }

    //todo this should be treated differently then When/Then
    public ConditionEvaluator UntilDelay(Condition condition, float delay) {
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

    //todo this should be treated differently then When/Then
    public ConditionEvaluator WhileDelay(Condition condition, float delay) {
        ConditionEvaluator evaluator = new ConditionEvaluator(condition);
        evaluators.Add(evaluator);
        return evaluator;
    }

    public ConditionEvaluator When(Condition condition) {
        ConditionEvaluator evaluator = new ConditionEvaluator(condition);
        evaluators.Add(evaluator);
        return evaluator;
    }
}

public class ConditionEvaluator {
    private Condition condition;
    private int x = 0;
    public delegate void Del();

    public ConditionEvaluator(Condition condition) {
        this.condition = condition;
    }

    public bool Evaluate() {
        if (condition.Eval()) {
            return true;
        } else {
            return false;
        }
    }
    public void Do(Del del) {
        x++;
        Debug.Log(x);
    }

    public void Then(Del del) {
        del();
    }

}