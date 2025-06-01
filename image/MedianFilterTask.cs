using System;
using System.Collections.Generic;
using NUnit.Framework;
namespace Recognizer;


[TestFixture]
public class FieldParserTaskTests
{
	[Test]
	public static void Test()
	{
		var array = new double [,] {{0.1, 0.3}};
		var actualResult = MedianFilterTask.MedianFilter(array);
		Assert.AreEqual(array,array);
	}
}
internal static class MedianFilterTask
{
	/* 
	 * Для борьбы с пиксельным шумом, подобным тому, что на изображении,
	 * обычно применяют медианный фильтр, в котором цвет каждого пикселя, 
	 * заменяется на медиану всех цветов в некоторой окрестности пикселя.
	 * https://en.wikipedia.org/wiki/Median_filter
	 * 
	 * Используйте окно размером 3х3 для не граничных пикселей,
	 * Окно размером 2х2 для угловых и 3х2 или 2х3 для граничных.
	 */
	public static double[,] MedianFilter(double[,] original)
	{
		var row = original.GetLength(0);
		var col = original.GetLength(1);
		var result = new double[row, col];
		for (var i  = 0; i < row; i++)
			for (var j  = 0; j < col; j++)
				result[i, j] = GetMedianFilterValue(original, i, j);
		return result;
	}

	private static double GetMedianFilterValue(double[,] original, int currentRow, int currentCol)
	{
		var numberRow = original.GetLength(0);
		var numberCol = original.GetLength(1);
		var sortableList = new List<double>();
		
		var rowMin = Math.Max(currentRow - 1, 0);
		var rowMax = currentRow + 1 < numberRow ? currentRow + 1 : currentRow;
		
		var colMin = Math.Max(currentCol - 1, 0);
		var colMax = currentCol + 1 < numberCol ? currentCol + 1 : currentCol;
		
		for (var i = rowMin; i < rowMax + 1; i++)
			for (var j = colMin; j < colMax + 1; j++)
				sortableList.Add(original[i, j]);
		
		sortableList.Sort();
		if (sortableList.Count % 2 == 0) 
			return (sortableList[sortableList.Count / 2] + sortableList[sortableList.Count / 2 - 1]) / 2;
		return sortableList[sortableList.Count / 2];
	}
}
