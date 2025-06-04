using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class MovingAverageTask
{
	public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> data, int windowWidth)
	{
		
		var boundedQueue = new BoundedQueueDataPoint(windowWidth);
		foreach (var dataPoint in data)
		{
			var currentDataPoint = dataPoint.WithAvgSmoothedY(dataPoint.OriginalY);
			boundedQueue.Add(currentDataPoint);
			yield return currentDataPoint.WithAvgSmoothedY(boundedQueue.CurrentSum / boundedQueue.Count);
		}
	}
}