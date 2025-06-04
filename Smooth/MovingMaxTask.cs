using System;
using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class MovingMaxTask
{
	public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> data, int windowWidth)
	{
		var boundedQueue = new BoundedQueueDataPoint(windowWidth);
		var maxTracker = new MaxTracker(windowWidth);

		foreach (var currentDataPoint in data)
		{
			boundedQueue.Add(currentDataPoint);
			maxTracker.Add(currentDataPoint.OriginalY);

			double maxY = maxTracker.CurrentMax;
			yield return new DataPoint(currentDataPoint).WithMaxY(maxY);
		}
	}
}


	

