using System.Collections.Generic;

namespace yield;

public class BoundedQueueDataPoint
{
    private readonly Queue<DataPoint> _queue;
    private readonly int _maxSize;
    private double _currentSum;

    public BoundedQueueDataPoint(int maxSize)
    {
        _maxSize = maxSize;
        _queue = new Queue<DataPoint>();
        _currentSum = 0;
    }

    public void Add(DataPoint item)
    {
        if (_queue.Count >= _maxSize)
        {
            RemoveFirst();
        }
        _queue.Enqueue(item);
        _currentSum += item.OriginalY;
    }

    public DataPoint RemoveFirst()
    {
        DataPoint oldest = _queue.Dequeue();
        _currentSum -= oldest.OriginalY;
        return oldest;
    }

    public int Count => _queue.Count;
    public double CurrentSum => _currentSum;
}