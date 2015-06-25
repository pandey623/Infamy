public abstract class NumericCondition : Condition {
    public abstract float GetValue();

    public override bool Eval() {
        return true;
    }
}