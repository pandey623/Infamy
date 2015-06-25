public class ArithmeticCondition : NumericCondition {
    protected NumericCondition c1;
    protected NumericCondition c2;
    protected ArithmeticOperator op;

    public ArithmeticCondition(NumericCondition c1, ArithmeticOperator op, NumericCondition c2) {
        this.c1 = c1;
        this.c2 = c2;
        this.op = op;
    }

    public override float GetValue() {
        switch (op) {
            case ArithmeticOperator.Plus:
                return c1.GetValue() + c2.GetValue();
            case ArithmeticOperator.Minus:
                return c1.GetValue() - c2.GetValue();
            case ArithmeticOperator.Multiply:
                return c1.GetValue() * c2.GetValue();
            case ArithmeticOperator.Modulus:
                return c1.GetValue() % c2.GetValue();
            case ArithmeticOperator.Divide:
                return c1.GetValue() / c2.GetValue();
        }
        return float.MinValue;
    }

    public static ArithmeticCondition operator +(ArithmeticCondition c1, float v) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Plus, new FloatConstantCondition(v));
    }

    public static ArithmeticCondition operator +(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Plus, c2);
    }

    public static ArithmeticCondition operator -(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Minus, c2);
    }

    public static ArithmeticCondition operator -(ArithmeticCondition c1, float v) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Minus, new FloatConstantCondition(v));
    }

    public static ArithmeticCondition operator *(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Multiply, c2);
    }

    public static ArithmeticCondition operator *(ArithmeticCondition c1, float v) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Multiply, new FloatConstantCondition(v));
    }

    public static ArithmeticCondition operator /(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Divide, c2);
    }
    
    public static ArithmeticCondition operator /(ArithmeticCondition c1, float v) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Divide, new FloatConstantCondition(v));
    }

    public static ArithmeticCondition operator %(ArithmeticCondition c1, float v) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Modulus, new FloatConstantCondition(v));
    }

    public static ArithmeticCondition operator %(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ArithmeticCondition(c1, ArithmeticOperator.Modulus, c2);
    }

    public static ComparisonCondition operator >(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ComparisonCondition(c1, ComparisonOperator.GreaterThan, c2);
    }

    public static ComparisonCondition operator >(ArithmeticCondition c1, float v) {
        return new ComparisonCondition(c1, ComparisonOperator.GreaterThan, new FloatConstantCondition(v));
    }

    public static ComparisonCondition operator >=(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ComparisonCondition(c1, ComparisonOperator.GreaterThanEqualTo, c2);
    }

    public static ComparisonCondition operator >=(ArithmeticCondition c1, float v) {
        return new ComparisonCondition(c1, ComparisonOperator.GreaterThanEqualTo, new FloatConstantCondition(v));
    }

    public static ComparisonCondition operator <(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ComparisonCondition(c1, ComparisonOperator.LessThan, c2);
    }

    public static ComparisonCondition operator <(ArithmeticCondition c1, float v) {
        return new ComparisonCondition(c1, ComparisonOperator.LessThan, new FloatConstantCondition(v));
    }

    public static ComparisonCondition operator <=(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ComparisonCondition(c1, ComparisonOperator.LessThanEqualTo, c2);
    }

    public static ComparisonCondition operator <=(ArithmeticCondition c1, float v) {
        return new ComparisonCondition(c1, ComparisonOperator.LessThanEqualTo, new FloatConstantCondition(v));
    }

    public static ComparisonCondition operator ==(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ComparisonCondition(c1, ComparisonOperator.Equal, c2);
    }

    public static ComparisonCondition operator ==(ArithmeticCondition c1, float v) {
        return new ComparisonCondition(c1, ComparisonOperator.Equal, new FloatConstantCondition(v));
    }

    public static ComparisonCondition operator !=(ArithmeticCondition c1, ArithmeticCondition c2) {
        return new ComparisonCondition(c1, ComparisonOperator.NotEqual, c2);
    }

    public static ComparisonCondition operator !=(ArithmeticCondition c1, float v) {
        return new ComparisonCondition(c1, ComparisonOperator.NotEqual, new FloatConstantCondition(v));
    }
}