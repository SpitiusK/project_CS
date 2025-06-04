using System.Collections.Generic;

public class MaxTracker
{
    private readonly LinkedList<(double Value, int Index)> _list = new LinkedList<(double, int)>();
    private readonly int _windowWidth;
    private int _currentIndex = 0;

    public MaxTracker(int windowWidth)
    {
        _windowWidth = windowWidth;
    }

    public void Add(double value)
    {
        // Удаляем элементы, вышедшие за пределы окна
        while (_list.Count > 0 && _currentIndex - _list.First.Value.Index >= _windowWidth)
        {
            _list.RemoveFirst();
        }

        // Если первый элемент меньше нового значения, очищаем список
        if (_list.Count > 0 && _list.First.Value.Value < value)
        {
            _list.Clear();
        }

        // Удаляем все элементы с конца, которые меньше нового значения
        while (_list.Count > 0 && _list.Last.Value.Value < value)
        {
            _list.RemoveLast();
        }

        // Добавляем новый элемент
        _list.AddLast((value, _currentIndex));
        _currentIndex++;
    }

    public double CurrentMax => _list.First.Value.Value;

    public bool IsEmpty => _list.Count == 0;
}