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

        public CellInfo()
        {
            Owner = -1;
            Distance = -1;
            Time = int.MaxValue;
        }
    }

    public static IEnumerable<OwnedLocation> AssignOwners(Map map)
    {
        int mapWidth = map.Maze.GetLength(0);
        int mapHeight = map.Maze.GetLength(1);
        int totalPlayers = map.Players.Length;

        // Инициализация массива информации о клетках
        var cellInfos = new CellInfo[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
                cellInfos[x, y] = new CellInfo();

        var chests = new HashSet<Point>(map.Chests);

        // Установка стартовых позиций игроков
        for (int playerIndex = 0; playerIndex < totalPlayers; playerIndex++)
        {
            var startPosition = map.Players[playerIndex];
            if (map.InBounds(startPosition) && map.Maze[startPosition.X, startPosition.Y] == MapCell.Empty)
            {
                cellInfos[startPosition.X, startPosition.Y] = new CellInfo
                {
                    Owner = playerIndex,
                    Distance = 0,
                    Time = 0
                };
            }
        }

        var directions = new Point[]
        {
            new Point(0, -1), // Вверх
            new Point(0, 1),  // Вниз
            new Point(-1, 0), // Влево
            new Point(1, 0)   // Вправо
        };

        // Запуск BFS для каждого игрока
        for (int playerIndex = 0; playerIndex < totalPlayers; playerIndex++)
        {
            ExplorePlayerTerritory(map, playerIndex, totalPlayers, cellInfos, chests, directions);
        }

        // Сбор результатов
        return Enumerable.Range(0, mapHeight)
            .SelectMany(y => Enumerable.Range(0, mapWidth)
                .Where(x => cellInfos[x, y].Time < int.MaxValue)
                .Select(x => new OwnedLocation(cellInfos[x, y].Owner, new Point(x, y), cellInfos[x, y].Distance)))
            .ToList();
    }

    private static void ExplorePlayerTerritory(Map map, int playerIndex, int totalPlayers,
        CellInfo[,] cellInfos, HashSet<Point> chests, Point[] directions)
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

            // Обновляем клетку только если новое время меньше текущего
            if (time < cellInfos[position.X, position.Y].Time)
            {
                cellInfos[position.X, position.Y] = new CellInfo
                {
                    Owner = playerIndex,
                    Distance = distance,
                    Time = time
                };
            }

            // Если клетка не сундук, проверяем соседние клетки
            if (!chests.Contains(position))
            {
                foreach (var direction in directions)
                {
                    var nextPosition = position + direction;
                    int nextTime = playerIndex + distance * totalPlayers;
                    if (IsValidMove(map, nextPosition, visited, cellInfos, nextTime))
                    {
                        queue.Enqueue((nextPosition, distance + 1));
                        visited.Add(nextPosition);
                    }
                }
            }
        }
    }

    private static bool IsValidMove(Map map, Point position, HashSet<Point> visited, CellInfo[,] cellInfos, int nextTime)
    {
        return map.InBounds(position) &&
               map.Maze[position.X, position.Y] == MapCell.Empty &&
               !visited.Contains(position) &&
               cellInfos[position.X, position.Y].Time > nextTime;
    }
}