using System;
using System.Linq;

namespace GaussAlgorithm;

public class Solver
{
	public double[] Solve(double[][] matrix, double[] freeMembers)
	{
		var systemEquations = matrix.Select((row, i) => new Row(row, freeMembers[i])).ToArray();
		var columns = systemEquations[0].Coefficients.Length;

		// Этап исключения
		for (var j = 0; j < columns; j++)
		{
			for (var i = 0; i < systemEquations.Length; i++)
			{
				if (!systemEquations[i].IsUsed && Math.Abs(systemEquations[i].Coefficients[j]) > 1e-10)
				{
					systemEquations[i].IsUsed = true;
					ReduceAllToZero(i, j, systemEquations);
					break;
				}
			}
		}

		// Получение решения
		return GetSolution(systemEquations, columns);
	}

	public void ReduceAllToZero(int row, int col, Row[] systemEquations)
	{
		var pivot = systemEquations[row].Coefficients[col];
		systemEquations[row] = systemEquations[row] * (1.0 / pivot);
		for (var i = 0; i < systemEquations.Length; i++)
		{
			if (i == row || !(Math.Abs(systemEquations[i].Coefficients[col]) > 1e-10)) continue;
			var k = -systemEquations[i].Coefficients[col];
			systemEquations[i] = systemEquations[i] + (systemEquations[row] * k);
		}
	}
	
	private double[] GetSolution(Row[] systemEquations, int columns)
	{
		int rows = systemEquations.Length;
		double[] solution = new double[columns];

		// Проверка на противоречия
		foreach (var row in systemEquations)
		{
			if (row.Coefficients.All(c => Math.Abs(c) < 1e-10))
			{
				if (Math.Abs(row.FreeMember) > 1e-10)
					throw new NoSolutionException("NoSolutionException");
			}
		}

		// Обратная подстановка
		for (int i = rows - 1; i >= 0; i--)
		{
			var row = systemEquations[i];
			if (row.Coefficients.All(c => Math.Abs(c) < 1e-10))
				continue; // Пропускаем нулевые строки

			int pivotCol = -1;
			for (int j = 0; j < columns; j++)
			{
				if (Math.Abs(row.Coefficients[j]) > 1e-10)
				{
					pivotCol = j;
					break;
				}
			}

			double value = row.FreeMember;
			for (int j = pivotCol + 1; j < columns; j++)
			{
				if (Math.Abs(row.Coefficients[j]) > 1e-10)
				{
					value -= row.Coefficients[j] * solution[j];
				}
			}
			solution[pivotCol] = value / row.Coefficients[pivotCol];
		}

		return solution;
	}

}

public class Row
{
	public double[] Coefficients { get; set; }
	public bool IsUsed { get; set; }
	public double FreeMember { get; set; }

	public Row(double[] coefficient, double freeMember, bool isUsed = false)
	{
		Coefficients = coefficient.Clone() as double[];
		FreeMember = freeMember;
		IsUsed = isUsed;
	}

	public static Row operator *(Row currentRow, double multiplier)
	{
		var newCoefficient = currentRow.Coefficients.Select(c => c * multiplier).ToArray();
		return new Row(newCoefficient, currentRow.FreeMember * multiplier, currentRow.IsUsed);
	}

	public static Row operator +(Row currentRow, Row otherRow)
	{
		var newCoefficient = currentRow.Coefficients.Zip(otherRow.Coefficients, (x, y) => x + y).ToArray();
		return new Row(newCoefficient, currentRow.FreeMember + otherRow.FreeMember, currentRow.IsUsed);
	}
}