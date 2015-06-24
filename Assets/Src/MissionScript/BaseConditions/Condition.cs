public abstract class Condition {
    protected bool inverted = false;

    public abstract bool Eval();

    public Condition Invert() {
        var c = MemberwiseClone() as Condition;
        c.inverted = !inverted;
        return c;
    }

    public static Condition operator !(Condition c) {
        return c.Invert();
    }

    public static OrCondition operator |(Condition c1, Condition c2) {
        return new OrCondition(c1, c2);
    }

    public static AndCondition operator &(Condition c1, Condition c2) {
        return new AndCondition(c1, c2);
    }
}