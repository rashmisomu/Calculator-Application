

namespace CalculatorApp.Model
{
    public class CalculatorModel
    {
        public class HistoryItem
        {
            public string Expression { get; set; }
            public string Result { get; set; }
        }

        public double Calculate(double left, double right, string op)
        {
            return op switch
            {
                "+" => left + right,
                "-" => left - right,
                "×" => left * right,
                "÷" => (right == 0 && left == 0) ? throw new InvalidOperationException("Result is undefined ")
                               : (right != 0 ? left / right : throw new DivideByZeroException("Cannot divide by zero")),

                "%" => (left * right) / 100, // e.g., 200 % 10 = 20
                _ => throw new InvalidOperationException("Unknown binary operator")
            };
        }

        public double UnaryCalculate(double value, string operation)
        {

            return operation switch
            {
                "sqrt" => value >= 0 ? Math.Sqrt(value) : double.NaN,
                "sqr" => value * value,
                "1/x" => value != 0 ? 1 / value : throw new DivideByZeroException("Cannot divide by zero."),
                "+/-" => -value,
                _ => value
            };
        }


    }
}
