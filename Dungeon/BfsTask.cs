using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class BfsTask
{	
	public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
	{
		var queue = new Queue<SinglyLinkedList<Point>>();
		var visited = new HashSet<Point>();
		var chestPositions = new HashSet<Point>(chests.Select(chest => chest.Location));
		queue.Enqueue(new SinglyLinkedList<Point>(start));
		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			if (chestPositions.Contains(current.Value)) yield return current;
			foreach (var direction in Walker.PossibleDirections)
			{
				var newPoint = current.Value + direction;
				if (!map.InBounds(newPoint) || map.Dungeon[newPoint.X, newPoint.Y] != MapCell.Empty ||
				    visited.Contains(newPoint)) continue;
				visited.Add(newPoint);
				queue.Enqueue(new SinglyLinkedList<Point>(newPoint, current));
			}
		}
	}
}