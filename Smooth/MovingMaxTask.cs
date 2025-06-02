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
				
				if (linkedListWithIndex.Last.Value.variableValue < currentDataPoint.OriginalY)
				{
					linkedListWithIndex.Clear();
				}
				linkedListWithIndex.AddLast((currentDataPoint.OriginalY, iterationNumber));
			}
			else
			{
				linkedListWithIndex.AddFirst((currentDataPoint.OriginalY, iterationNumber));
			}
			RemoveAllIfIndexBigger(linkedListWithIndex, iterationNumber);
			yield return new DataPoint(currentDataPoint).WithMaxY(linkedListWithIndex.First.Value.variableValue);
			iterationNumber++;
		}
	}

	static void RemoveAllIfIndexBigger(LinkedList<(double variableValue, int index)> linkedListWithIndex, int iterationNumber)
	{
		if (iterationNumber  - linkedListWithIndex.First.Value.index <= 0) return;
		
		linkedListWithIndex.RemoveFirst();
		RemoveAllIfIndexBigger(linkedListWithIndex, iterationNumber);
	}
}

