using System;
using System.Linq;

namespace GaussAlgorithm;

public class Solver
{
    private const double Epsilon = 1e-10;

    public double[] Solve(double[][] matrix, double[] freeMembers)
    {
        // Проверка входных данных
        ValidateInput(matrix, freeMembers);

        // Преобразование в массив Row
        var systemEquations = matrix.Select((row, i) => new Row(row, freeMembers[i])).ToArray();
        var columns = systemEquations[0].Coefficients.Length;

        // Прямой ход (этап исключения)
        for (var j = 0; j < columns; j++)
        {
            for (var i = 0; i < systemEquations.Length; i++)
            {
                if (!systemEquations[i].IsUsed && IsNonZero(systemEquations[i].Coefficients[j]))
                {
                    systemEquations[i].IsUsed = true;
                    ReduceAllToZero(i, j, systemEquations);
                    break;
                }
            }
        }

        // Обратный ход (получение решения)
        return GetSolution(systemEquations, columns);
    }

    private void ValidateInput(double[][] matrix, double[] freeMembers)
    {
        if (matrix == null || freeMembers == null || matrix.Length == 0 
            || matrix[0].Length == 0 || matrix.Length != freeMembers.Length)
            throw new ArgumentException("Invalid input data.");
        if (matrix.Any(row => row.Length != matrix[0].Length))
            throw new ArgumentException("All matrix rows must have the same length.");
    }

    private void ReduceAllToZero(int row, int col, Row[] systemEquations)
    {
        var pivot = systemEquations[row].Coefficients[col];
        if (!IsNonZero(pivot)) return;

        systemEquations[row] = systemEquations[row].Normalize(pivot);
        for (var i = 0; i < systemEquations.Length; i++)
        {
            if (i == row || !IsNonZero(systemEquations[i].Coefficients[col])) continue;
            var k = -systemEquations[i].Coefficients[col];
            systemEquations[i] = systemEquations[i] + (systemEquations[row] * k);
        }
    }

    private double[] GetSolution(Row[] systemEquations, int columns)
    {
        CheckForNoSolution(systemEquations);

        int rows = systemEquations.Length;
        double[] solution = new double[columns];

        // Обратная подстановка
        for (int i = rows - 1; i >= 0; i--)
        {
            var row = systemEquations[i];
            int leadingColumnIndex = FindLeadingColumnIndex(row);
            if (leadingColumnIndex == -1) continue; // Пропускаем нулевые строки

            double variableValue = CalculateVariableValue(row, leadingColumnIndex, solution);
            solution[leadingColumnIndex] = variableValue / row.Coefficients[leadingColumnIndex];
        }

        return solution;
    }

    private void CheckForNoSolution(Row[] systemEquations)
    {
        foreach (var row in systemEquations)
        {
            if (IsZeroRow(row) && IsNonZero(row.FreeMember))
                throw new NoSolutionException("NoSolutionException");
        }
    }

    private int FindLeadingColumnIndex(Row row)
    {
        for (int j = 0; j < row.Coefficients.Length; j++)
        {
            if (IsNonZero(row.Coefficients[j]))
                return j;
        }
        return -1;
    }

    private double CalculateVariableValue(Row row, int leadingColumnIndex, double[] solution)
    {
        double variableValue = row.FreeMember;
        for (int j = leadingColumnIndex + 1; j < row.Coefficients.Length; j++)
        {
            if (IsNonZero(row.Coefficients[j]))
                variableValue -= row.Coefficients[j] * solution[j];
        }
        return variableValue;
    }

    private bool IsNonZero(double value) => Math.Abs(value) > Epsilon;

    private bool IsZeroRow(Row row) => row.Coefficients.All(c => !IsNonZero(c));
}

public class Row
{
    public double[] Coefficients { get; }
    public bool IsUsed { get; set; }
    public double FreeMember { get; }

    public Row(double[] coefficients, double freeMember, bool isUsed = false)
    {
        Coefficients = (double[])coefficients.Clone();
        FreeMember = freeMember;
        IsUsed = isUsed;
    }

    public Row Normalize(double pivot) => this * (1.0 / pivot);

    public static Row operator *(Row currentRow, double multiplier)
    {
        var newCoefficients = currentRow.Coefficients.Select(c => c * multiplier).ToArray();
        return new Row(newCoefficients, currentRow.FreeMember * multiplier, currentRow.IsUsed);
    }

    public static Row operator +(Row currentRow, Row otherRow)
    {
        var newCoefficients = currentRow.Coefficients.Zip(otherRow.Coefficients, (x, y) => x + y).ToArray();
        return new Row(newCoefficients, currentRow.FreeMember + otherRow.FreeMember, currentRow.IsUsed);
    }
}