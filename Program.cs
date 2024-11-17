using System;
using System.Text;

class Interpolation
{
    static double Function(double x) => 4 * Math.Cos(2 * x);

    static double[] GenerateNodes(int n, double a, double b)
    {
        int nodesCount = n + 1;
        double[] nodes = new double[nodesCount];
        for (int i = 0; i < nodesCount; i++)
        {
            nodes[i] = a + i * (b - a) / (nodesCount - 1);
        }
        return nodes;
    }

    static double LagrangePolynomial(double[] x, double[] y, double targetX, out string lagrangeExpression)
    {
        int n = x.Length;
        double result = 0;
        var expression = new StringBuilder();

        for (int i = 0; i < n; i++)
        {
            double term = y[i];
            var termExpression = new StringBuilder($"{y[i]:0.##}");

            for (int j = 0; j < n; j++)
            {
                if (j != i)
                {
                    term *= (targetX - x[j]) / (x[i] - x[j]);
                    termExpression.Append($" * ((x - {x[j]:0.##}) / ({x[i]:0.##} - {x[j]:0.##}))");
                }
            }
            result += term;
            expression.Append(termExpression + (i < n - 1 ? " + " : ""));
        }

        lagrangeExpression = expression.ToString();
        return result;
    }

    static double[] DividedDifferences(double[] x, double[] y)
    {
        int n = x.Length;
        double[] divDiff = new double[n];
        Array.Copy(y, divDiff, n);

        for (int j = 1; j < n; j++)
        {
            for (int i = n - 1; i >= j; i--)
            {
                divDiff[i] = (divDiff[i] - divDiff[i - 1]) / (x[i] - x[i - j]);
            }
        }
        return divDiff;
    }

    static void PrintDividedDifferencesTable(double[] x, double[] divDiff)
    {
        int n = x.Length;
        var table = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            table[i, 0] = divDiff[i];
        }

        for (int j = 1; j < n; j++)
        {
            for (int i = 0; i < n - j; i++)
            {
                table[i, j] = (table[i + 1, j - 1] - table[i, j - 1]) / (x[i + j] - x[i]);
            }
        }

        Console.WriteLine("\nTable of divided differences:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"x_{i} = {x[i],6:0.##} | ");
            for (int j = 0; j < n - i; j++)
            {
                Console.Write($"{table[i, j],10:0.####} ");
            }
            Console.WriteLine();
        }
    }


    static double NewtonPolynomial(double[] x, double[] divDiff, double targetX, out string newtonExpression)
    {
        double result = divDiff[0];
        double product = 1;
        var expression = new StringBuilder($"{divDiff[0]:0.##}");

        for (int i = 1; i < x.Length; i++)
        {
            product *= (targetX - x[i - 1]);
            result += divDiff[i] * product;

            expression.Append($" + ({divDiff[i]:0.##}");
            for (int j = 0; j < i; j++)
            {
                expression.Append($" * (x - {x[j]:0.##})");
            }
            expression.Append(")");
        }

        newtonExpression = expression.ToString();
        return result;
    }

    static double Derivative(int n, double x)
    {
        double coefficient = 4 * Math.Pow(2, n);
        switch (n % 4)
        {
            case 0: return coefficient * Math.Cos(2 * x);
            case 1: return -coefficient * Math.Sin(2 * x);
            case 2: return -coefficient * Math.Cos(2 * x);
            case 3: return coefficient * Math.Sin(2 * x);
            default: return 0;
        }
    }

    static double ErrorEstimation(double[] xNodes, double targetX, int n)
    {
        double product = 1;
        foreach (double x in xNodes)
        {
            product *= (targetX - x);
        }

        double derivativeValue = Derivative(n + 1, targetX);
        double factorial = Factorial(n + 1);

        return Math.Abs(derivativeValue * product / factorial);
    }

    static double Factorial(int n)
    {
        double result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    static void Main()
    {
        Console.Write("Enter the degree of the polynomial (n): ");
        int n = int.Parse(Console.ReadLine());

        double a = 0, b = Math.PI;
        double[] x = GenerateNodes(n, a, b);
        double[] y = new double[x.Length];
        for (int i = 0; i < x.Length; i++)
        {
            y[i] = Function(x[i]);
        }

        double[] divDiff = DividedDifferences(x, y);

        Console.Write("Enter the value of x to evaluate: ");
        double targetX = double.Parse(Console.ReadLine());

        string lagrangeExpression, newtonExpression;
        double LnX = LagrangePolynomial(x, y, targetX, out lagrangeExpression);
        double PnX = NewtonPolynomial(x, divDiff, targetX, out newtonExpression);
        double fX = Function(targetX);
        double error = ErrorEstimation(x, targetX, n);

        Console.WriteLine($"\nResults at x = {targetX}:\n");
        Console.WriteLine($"Lagrange Polynomial Ln(x) = {LnX}");
        Console.WriteLine($"Newton Polynomial Pn(x) = {PnX}");
        Console.WriteLine($"Exact Function f(x) = {fX}");
        Console.WriteLine($"Interpolation Error Estimate Rn(x) = {error}");

        Console.WriteLine($"\nLagrange Polynomial Expression:\nLn(x) = {lagrangeExpression}");
        Console.WriteLine($"\nNewton Polynomial Expression:\nPn(x) = {newtonExpression}");

        PrintDividedDifferencesTable(x, divDiff);
    }
}
