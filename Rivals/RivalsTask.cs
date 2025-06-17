using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivals;

public class RivalsTask
{
    private struct CellInfo
    {
        public int Owner;    // Индекс игрока, владеющего клеткой
        public int Distance; // Расстояние от стартовой позиции игрока
        public int Time;     // Время достижения клетки

        public static CellInfo Unreachable => new()
        {
            Owner = -1,
            Distance = -1,
            Time = int.MaxValue
        };
    }

    private static readonly Point[] MovementDirections =
    {
        new(0, -1), // Вверх
        new(0, 1),  // Вниз
        new(-1, 0), // Влево
        new(1, 0)   // Вправо
    };

    /// <summary>
    /// Assigns owners to map cells based on the fastest player to reach each cell.
    /// </summary>
    public static IEnumerable<OwnedLocation> AssignOwners(Map map)
    {
        if (map.Players.Length == 0)
            return Enumerable.Empty<OwnedLocation>();

        int mapWidth = map.Maze.GetLength(0);
        int mapHeight = map.Maze.GetLength(1);
        int totalPlayers = map.Players.Length;

        // Инициализация клеток и стартовых позиций
        var cellOwners = InitializeCellOwners(map, mapWidth, mapHeight, totalPlayers);

        // Создание множества сундуков
        var chests = CreateChestsSet(map);

        // Обработка территории каждого игрока
        for (int playerIndex = 0; playerIndex < totalPlayers; playerIndex++)
        {
            ExplorePlayerTerritory(map, playerIndex, totalPlayers, cellOwners, chests);
        }

        // Сбор результатов
        return CollectResults(cellOwners, mapWidth, mapHeight);
    }

    /// <summary>
    /// Initializes the cell ownership array and sets starting positions for players.
    /// </summary>
    private static CellInfo[,] InitializeCellOwners(Map map, int mapWidth, int mapHeight, int totalPlayers)
    {
        var cellOwners = new CellInfo[mapWidth, mapHeight];

        // Инициализация всех клеток как недостижимых
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                cellOwners[x, y] = CellInfo.Unreachable;

        // Установка стартовых позиций игроков
        for (int playerIndex = 0; playerIndex < totalPlayers; playerIndex++)
        {
            var startPosition = map.Players[playerIndex];
            if (map.InBounds(startPosition) && map.Maze[startPosition.X, startPosition.Y] == MapCell.Empty)
            {
                cellOwners[startPosition.X, startPosition.Y] = new CellInfo
                {
                    Owner = playerIndex,
                    Distance = 0,
                    Time = 0
                };
            }
        }

        return cellOwners;
    }

    /// <summary>
    /// Creates a set of chest positions for efficient lookup.
    /// </summary>
    private static HashSet<Point> CreateChestsSet(Map map)
    {
        return new HashSet<Point>(map.Chests);
    }

    /// <summary>
    /// Explores the territory of a single player using BFS.
    /// </summary>
    private static void ExplorePlayerTerritory(Map map, int playerIndex, int totalPlayers,
        CellInfo[,] cellOwners, HashSet<Point> chests)
    {
        var startPosition = map.Players[playerIndex];
        if (!map.InBounds(startPosition) || map.Maze[startPosition.X, startPosition.Y] == MapCell.Wall)
            return;

        var queue = new Queue<(Point Position, int Distance)>();
        var visited = new HashSet<Point>();
        queue.Enqueue((startPosition, 0));
        visited.Add(startPosition);

        while (queue.Count > 0)
        {
            var (position, distance) = queue.Dequeue();
            int time = distance == 0 ? 0 : playerIndex + (distance - 1) * totalPlayers;

            // Обновление владельца клетки
            UpdateCellOwner(cellOwners, position, playerIndex, distance, time);

            // Проверка и добавление соседних клеток
            if (!chests.Contains(position))
            {
                TryAddNeighborsToQueue(map, position, distance, playerIndex, totalPlayers, cellOwners, visited, queue);
            }
        }
    }

    /// <summary>
    /// Updates the cell owner if the new time is less than the current time.
    /// </summary>
    private static void UpdateCellOwner(CellInfo[,] cellOwners, Point position, int playerIndex, int distance, int time)
    {
        if (time < cellOwners[position.X, position.Y].Time)
        {
            cellOwners[position.X, position.Y] = new CellInfo
            {
                Owner = playerIndex,
                Distance = distance,
                Time = time
            };
        }
    }

    /// <summary>
    /// Tries to add neighboring cells to the BFS queue if they are valid.
    /// </summary>
    private static void TryAddNeighborsToQueue(Map map, Point position, int distance, int playerIndex, int totalPlayers,
        CellInfo[,] cellOwners, HashSet<Point> visited, Queue<(Point Position, int Distance)> queue)
    {
        int nextTime = playerIndex + distance * totalPlayers;
        foreach (var direction in MovementDirections)
        {
            var nextPosition = position + direction;
            if (IsValidMove(map, nextPosition, visited, cellOwners, nextTime))
            {
                queue.Enqueue((nextPosition, distance + 1));
                visited.Add(nextPosition);
            }
        }
    }

    /// <summary>
    /// Checks if a move to the given position is valid.
    /// </summary>
    private static bool IsValidMove(Map map, Point position, HashSet<Point> visited,
        CellInfo[,] cellOwners, int nextTime)
    {
        return map.InBounds(position) &&                    // Клетка в пределах карты
               map.Maze[position.X, position.Y] == MapCell.Empty && // Клетка не стена
               !visited.Contains(position) &&              // Клетка не посещена
               cellOwners[position.X, position.Y].Time > nextTime; // Новое время меньше текущего
    }

    /// <summary>
    /// Collects the final results as a list of owned locations.
    /// </summary>
    private static IEnumerable<OwnedLocation> CollectResults(CellInfo[,] cellOwners, int mapWidth, int mapHeight)
    {
        return Enumerable.Range(0, mapHeight)
            .SelectMany(y => Enumerable.Range(0, mapWidth)
                .Where(x => cellOwners[x, y].Time < int.MaxValue)
                .Select(x => new OwnedLocation(cellOwners[x, y].Owner, new Point(x, y), cellOwners[x, y].Distance)));
    }
}