using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace StructBenchmarking;

public class Benchmark : IBenchmark
{
    public double MeasureDurationInMs(ITask task, int repetitionCount)
    {
        task.Run();                    // Прогревочный вызов
        GC.Collect();                  // Сборка мусора
        GC.WaitForPendingFinalizers(); // Ожидание финализаторов
        var time = Stopwatch.StartNew();
        for (var i = 0; i < repetitionCount; i++)
        {
            task.Run();
        }
        time.Stop();
        return time.Elapsed.TotalMilliseconds / repetitionCount; // Точное измерение
    }
}

[TestFixture]
public class RealBenchmarkUsageSample
{
    [Test]
    public void StringConstructorFasterThanStringBuilder()
    {
        var benchmark = new Benchmark();
        int repetitionCount = 10000; 
        var actual = benchmark.MeasureDurationInMs(new StingConstruct(), repetitionCount);
        var expected = benchmark.MeasureDurationInMs(new StringBuilderTask(), repetitionCount);
        Console.WriteLine($"String Constructor: {actual} ms");
        Console.WriteLine($"StringBuilder: {expected} ms");
        Assert.Less(actual, expected);
    }
}

public class StingConstruct : ITask
{
    public void Run()
    {
        var testString = new string('a', 10000);
    }
}

public class StringBuilderTask : ITask
{
    public void Run()
    {
        var testStringBuilder = new StringBuilder();
        for (var i = 0; i < 10000; i++)
        {
            testStringBuilder.Append('a');
        }
        testStringBuilder.ToString();
    }
}