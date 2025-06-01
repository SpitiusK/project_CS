using System;

namespace LimitedSizeStack
{
    public class LimitedSizeStack<T>
    {
        private int _count; // Приватное поле для хранения текущего количества элементов
        public int Count => _count; // Свойство только для чтения
        public readonly int Capacity; // Максимальная вместимость стека
        private LimitedSizeStackItem<T> head; // Указатель на первый элемент
        private LimitedSizeStackItem<T> tail; // Указатель на последний элемент

        public LimitedSizeStack(int undoLimit)
        {
            if (undoLimit < 0)
                throw new ArgumentException("Capacity cannot be negative", nameof(undoLimit));
            Capacity = undoLimit;
        }

        public void Push(T item)
        {
            var newItem = new LimitedSizeStackItem<T> { Value = item };

            // Если стек полон, удаляем самый старый элемент
            if (Capacity > 0)
            {
                if (_count >= Capacity)
                {
                    Dequeue();
                }

                // Добавляем новый элемент
                if (_count == 0)
                {
                    head = tail = newItem;
                }
                else
                {
                    newItem.Previous = tail;
                    tail.Next = newItem;
                    tail = newItem;
                }

                _count++;
            }
        }

        public T Pop()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Stack is empty");
            }

            T result = tail.Value;
            tail = tail.Previous;
            if (tail != null)
            {
                tail.Next = null;
            }
            else
            {
                head = null;
            }
            _count--;

            return result;
        }

        private void Dequeue()
        {
            if (_count == 0)
            {
                return; // Ничего не делаем, если стек пуст
            }

            head = head.Next;
            if (head != null)
            {
                head.Previous = null;
            }
            else
            {
                tail = null;
            }
            _count--;
        }
    }

    public class LimitedSizeStackItem<T>
    {
        public T Value { get; set; }
        public LimitedSizeStackItem<T> Next { get; set; }
        public LimitedSizeStackItem<T> Previous { get; set; }
    }
}