public class FalseCondition : Condition {
    public override bool Eval() {
        return inverted;
    }
}
