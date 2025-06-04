using System;
using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class MovingMaxTask
{
	public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
	{
		var boundedQueue = new BoundedQueueDataPoint(windowWidth);
		var linkedListWithIndex = new LinkedList<(double variableValue, int index)>();
		var iterationNumber = 0;
		foreach (var currentDataPoint in data)
		{
			boundedQueue.Add(currentDataPoint);
			if (linkedListWithIndex.Count > 0)
			{
				RemoveFirstIfIndexBigger(linkedListWithIndex, iterationNumber, windowWidth);
				if (linkedListWithIndex.Count != 0 && linkedListWithIndex.First.Value.variableValue < currentDataPoint.OriginalY)
				{
					linkedListWithIndex.Clear();
				}
				RemoveAllLess(linkedListWithIndex, currentDataPoint.OriginalY);
				linkedListWithIndex.AddLast((currentDataPoint.OriginalY, iterationNumber));
			}
			else
			{
				linkedListWithIndex.AddFirst((currentDataPoint.OriginalY, iterationNumber));
			}

			yield return new DataPoint(currentDataPoint).WithMaxY(linkedListWithIndex.First.Value.variableValue);
			iterationNumber++;
		}
	}

	static void RemoveFirstIfIndexBigger(LinkedList<(double variableValue, int index)> linkedListWithIndex, int iterationNumber, int windowWidth)
	{
		if (linkedListWithIndex.Count == 0 || iterationNumber  - linkedListWithIndex.First.Value.index < windowWidth) return;
		
		linkedListWithIndex.RemoveFirst();
		RemoveFirstIfIndexBigger(linkedListWithIndex, iterationNumber, windowWidth);
	}

	static void RemoveAllLess(LinkedList<(double variableValue, int index)> linkedListWithIndex, double currentY)
	{
		if (linkedListWithIndex.Count == 0 || linkedListWithIndex.Last.Value.variableValue > currentY) return;
		linkedListWithIndex.RemoveLast();
		RemoveAllLess(linkedListWithIndex, currentY);
	}
}

