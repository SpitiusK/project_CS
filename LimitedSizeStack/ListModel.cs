using System;
using System.Collections.Generic;

namespace LimitedSizeStack;

public class ListModel<TItem>
{
    public List<TItem> Items { get; }
    public int UndoLimit;
    public LimitedSizeStack<ActionItem<TItem>> Actions { get; set; }
    private readonly Dictionary<string, string> _undoActions = new Dictionary<string, string>();

    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
    }

    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items;
        UndoLimit = undoLimit;
        Actions = new LimitedSizeStack<ActionItem<TItem>>(undoLimit);
        _undoActions["Add"] = "Remove";    // Добавление отменяется удалением
        _undoActions["Remove"] = "Insert"; // Удаление отменяется вставкой
    }

    public void AddItem(TItem item)
    {
        Items.Add(item);
        Actions.Push(new ActionItem<TItem> { Action = "Add", Item = item, Index = Items.Count - 1 });
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= Items.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Индекс выходит за пределы списка.");

        TItem removedItem = Items[index];
        Items.RemoveAt(index);
        Actions.Push(new ActionItem<TItem> { Action = "Remove", Item = removedItem, Index = index });
    }

    public bool CanUndo()
    {
        return Actions.Count != 0;
    }

    public void Undo()
    {
        if (Actions.Count == 0)
            throw new InvalidOperationException("Нет действий для отмены.");

        ActionItem<TItem> lastAction = Actions.Pop();
        string undoAction = _undoActions[lastAction.Action];

        if (undoAction == "Remove")
        {
            // Отмена добавления: удаляем последний элемент
            Items.RemoveAt(Items.Count - 1);
        }
        else if (undoAction == "Insert")
        {
            // Отмена удаления: вставляем элемент обратно на его индекс
            Items.Insert(lastAction.Index, lastAction.Item);
        }
    }
}

public class ActionItem<T>
{
    public T Item { get; set; }
    public string Action { get; set; }
    public int Index { get; set; }
}