using System.Collections.Generic;

namespace yield;

public static class ExpSmoothingTask
{
	public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> data, double alpha)
	{
		DataPoint previosPoint = null;
		foreach (var dataPoint in data)
		{
			if (previosPoint == null)
			{
				previosPoint = dataPoint.WithExpSmoothedY(dataPoint.OriginalY);
				yield return previosPoint;
			}
			else
			{
				previosPoint = dataPoint.WithExpSmoothedY(alpha * dataPoint.OriginalY + (1 - alpha) * previosPoint.ExpSmoothedY);
				yield return previosPoint;
			}
		}
	}
}
