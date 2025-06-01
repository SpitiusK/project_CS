using System.Linq;

namespace Recognizer;
public static class ThresholdFilterTask
{
	public static double[,] ThresholdFilter(double[,] original, double whitePixelsFraction)
	{
		var sortableList = original.Cast<double>().ToList();
		var rowNumber = original.GetLength(0);
		var colNumber = original.GetLength(1);
		var result = new double[rowNumber, colNumber];
		var  thresholdValue = (int)(sortableList.Count * whitePixelsFraction);
		sortableList.Sort();
		sortableList.Reverse();
		// возможно ошибка в тестах на Юлерн, но этот метод берёт точное кол-во thresholdValue и не берет значения >=, то есть по идеии не берёт доп значения
		var thresholdList = sortableList.GetRange(0, thresholdValue);

		for (var row = 0; row < rowNumber; row++)
			for (var col = 0; col < colNumber; col++)
				if (thresholdList.Contains(original[row, col]))
					result[row, col] = 1;
		return result;
	}
}
