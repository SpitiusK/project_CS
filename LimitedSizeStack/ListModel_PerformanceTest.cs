using System.Diagnostics;
using NUnit.Framework;

namespace LimitedSizeStack;

[TestFixture]
class ListModel_PerformanceTest
{
	[Test, Timeout(500)]
	[Description("Не нужно хранить все состояния модели")]
	public void AntiStupidTest()
	{
		var clock = new Stopwatch();
		var undoLim = 30000;
		var model = new ListModel<int>(undoLim);
		clock.Start();
		for (var i = 0; i < undoLim; ++i)
		{
			model.AddItem(0);
		}
		clock.Stop();
		Assert.AreEqual(undoLim, model.Items.Count);
	}
}