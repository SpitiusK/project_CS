using System.Collections.Generic;

namespace StructBenchmarking;

public interface ITaskFactory
{
    ITask CreateStructTask(int fieldCount);
    ITask CreateClassTask(int fieldCount);
    string GetChartTitle();
}

public class ArrayCreationTaskFactory : ITaskFactory
{
    public ITask CreateStructTask(int fieldCount) => new StructArrayCreationTask(fieldCount);
    public ITask CreateClassTask(int fieldCount) => new ClassArrayCreationTask(fieldCount);
    public string GetChartTitle() => "Create array";
}

public class MethodCallTaskFactory : ITaskFactory
{
    public ITask CreateStructTask(int fieldCount) => new MethodCallWithStructArgumentTask(fieldCount);
    public ITask CreateClassTask(int fieldCount) => new MethodCallWithClassArgumentTask(fieldCount);
    public string GetChartTitle() => "Call method with argument";
}

public class Experiments
{
    public static ChartData BuildChartData(
        IBenchmark benchmark, int repetitionsCount, ITaskFactory taskFactory)
    {
        var classesTimes = new List<ExperimentResult>();
        var structuresTimes = new List<ExperimentResult>();
        
        foreach (var fieldCount in Constants.FieldCounts)
        {
            var structTask = taskFactory.CreateStructTask(fieldCount);
            var classTask = taskFactory.CreateClassTask(fieldCount);
            
            structuresTimes.Add(new ExperimentResult(fieldCount, 
                benchmark.MeasureDurationInMs(structTask, repetitionsCount)));
            classesTimes.Add(new ExperimentResult(fieldCount, 
                benchmark.MeasureDurationInMs(classTask, repetitionsCount)));
        }
        
        return new ChartData
        {
            Title = taskFactory.GetChartTitle(),
            ClassPoints = classesTimes,
            StructPoints = structuresTimes,
        };
    }

    public static ChartData BuildChartDataForArrayCreation(
        IBenchmark benchmark, int repetitionsCount)
    {
        return BuildChartData(benchmark, repetitionsCount, new ArrayCreationTaskFactory());
    }

    public static ChartData BuildChartDataForMethodCall(
        IBenchmark benchmark, int repetitionsCount)
    {
        return BuildChartData(benchmark, repetitionsCount, new MethodCallTaskFactory());
    }
}