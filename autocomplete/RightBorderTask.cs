using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Autocomplete;

[TestFixture]
public class RightBorderTaskTests
{
    [Test]
    public void RightBorderTest()
    {
        var prefix = "abc";
        var phrases = new List<string>(){"ab", "abc", "abc", "abd"};
        var result = RightBorderTask.GetRightBorderIndex(phrases, prefix, -1, phrases.Count);
        
    }
}


public class RightBorderTask
{
    /// <returns>
    /// Возвращает индекс правой границы. 
    /// То есть индекс минимального элемента, который не начинается с prefix и большего prefix.
    /// Если такого нет, то возвращает items.Length
    /// </returns>
    /// <remarks>
    /// Функция должна быть НЕ рекурсивной
    /// и работать за O(log(items.Length)*L), где L — ограничение сверху на длину фразы
    /// </remarks>
    public static int GetRightBorderIndex(IReadOnlyList<string> phrases, string prefix, int left, int right)
    {
        // IReadOnlyList похож на List, но у него нет методов модификации списка.
        // Этот код решает задачу, но слишком неэффективно. Замените его на бинарный поиск!
        while (left + 1 < right)
        {
            var center = left + (right - left) / 2;;
            if (string.Compare(phrases[center], prefix) > 0 && !phrases[center].StartsWith(prefix))
            {
                right = center;
            }
            else
            {
                left = center;
            }
        }
        return right;
    }
}