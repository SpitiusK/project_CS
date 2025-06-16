using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon;

public class DungeonTask
{
	public static MoveDirection[] FindShortestPath(Map map)
	{
		var pathsToChests = BfsTask.FindPaths(map, map.InitialPosition, map.Chests);
		var pathsFromExitToChestsDict = BfsTask.FindPaths(map, map.Exit, map.Chests)
			.ToDictionary(p => p.Value, p => p);
		var validPaths = 
			from pathToChest in pathsToChests
			let chestPoint = pathToChest.Value
			where pathsFromExitToChestsDict.ContainsKey(chestPoint)
			let pathToChestFromExit = pathsFromExitToChestsDict[chestPoint]
			let totalLength = pathToChest.Length + pathToChestFromExit.Length - 1 
			select (
				ChestPoint: chestPoint, 
				TotalLength: totalLength,
				PathToChest: pathToChest,
				PathToExcitFromChest: pathToChestFromExit);
		return GetResultPath(validPaths, map);
	}

	private static MoveDirection[] GetResultPath(
		IEnumerable<(Point ChestPoint, int TotalLength, SinglyLinkedList<Point> PathToChest, 
			SinglyLinkedList<Point> PathToExcitFromChest)> validPaths, Map map)
	{
		if (!validPaths.Any())
		{
			var directPath = BfsTask.FindPaths(map, map.InitialPosition, new[] { new Chest(map.Exit, 0) }).FirstOrDefault();
			return directPath != null ? ConvertToMoves(directPath.Reverse()) : new MoveDirection[0];
		}
		var chestValues = map.Chests.ToDictionary(c => c.Location, c => c.Value);
		var bestPath = validPaths
			.OrderBy(p => p.TotalLength)
			.ThenByDescending(p => chestValues[p.ChestPoint])
			.First();
		var movesToChest = bestPath.PathToChest.Reverse(); // От P до сундука
		var pathFromChestToExit = bestPath.PathToExcitFromChest.Skip(1); // От сундука до E
		var moveToExitThroughChest = movesToChest.Concat(pathFromChestToExit);
		var resultPath = ConvertToMoves(moveToExitThroughChest);
		return resultPath;
	}
	
	private static MoveDirection[] ConvertToMoves(IEnumerable<Point> points)
	{
		var moves = new List<MoveDirection>();
		var enumerator = points.GetEnumerator();
		if (!enumerator.MoveNext())
			return moves.ToArray();
        
		var previous = enumerator.Current;
		while (enumerator.MoveNext())
		{
			var current = enumerator.Current;
			if (current.X > previous.X)
				moves.Add(MoveDirection.Right);
			else if (current.X < previous.X)
				moves.Add(MoveDirection.Left);
			else if (current.Y > previous.Y)
				moves.Add(MoveDirection.Down);
			else if (current.Y < previous.Y)
				moves.Add(MoveDirection.Up);
			previous = current;
		}
		return moves.ToArray();
	}
}