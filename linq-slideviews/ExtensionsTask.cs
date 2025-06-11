﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public static class ExtensionsTask
{
	/// <summary>
	/// Медиана списка из нечетного количества элементов — это серединный элемент списка после сортировки.
	/// Медиана списка из четного количества элементов — это среднее арифметическое 
    /// двух серединных элементов списка после сортировки.
	/// </summary>
	/// <exception cref="InvalidOperationException">Если последовательность не содержит элементов</exception>
	public static double Median(this IEnumerable<double> items)
	{
		if (items == null) throw new ArgumentNullException(nameof(items));
		var itemsArray = items.ToArray();
		if (itemsArray.Length == 0) throw new InvalidOperationException();
		
		var sortedItems = itemsArray.OrderBy(x => x).ToArray();
		if (sortedItems.Length % 2 != 0) return sortedItems[sortedItems.Length / 2];
		return (sortedItems[sortedItems.Length / 2] +  sortedItems[sortedItems.Length / 2 - 1]) / 2.0;
	}

	/// <returns>
	/// Возвращает последовательность, состоящую из пар соседних элементов.
	/// Например, по последовательности {1,2,3} метод должен вернуть две пары: (1,2) и (2,3).
	/// </returns>
	public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> items)
	{
		var enumerator = items.GetEnumerator();
		if (!enumerator.MoveNext()) yield break;
		T previous = enumerator.Current;
		while (enumerator.MoveNext())
		{
			T current = enumerator.Current;
			yield return (previous, current);
			previous = current; 
		}
	}
}