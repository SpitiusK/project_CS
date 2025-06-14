using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public class StatisticsTask
{
	public static double GetMedianTimePerSlide(List<VisitRecord> visits, SlideType slideType)
	{
		var times = visits
			.GroupBy(visit => visit.UserId)
			.SelectMany(group => group
				.OrderBy(visit => visit.DateTime)
				.Bigrams()
				.Select(bigram => (visitTime: bigram.Second.DateTime - bigram.First.DateTime,
					slideType: bigram.First.SlideType))
				.Where(timeInfo => timeInfo.slideType == slideType
				                   && timeInfo.visitTime.TotalMinutes is >= 1 and <= 120)
				.Select(time => time.visitTime.TotalMinutes)).ToList();
		return times.Any() ? times.Median() : 0;
	}
}