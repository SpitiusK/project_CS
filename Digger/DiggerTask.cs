using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Media;
using Digger.Architecture;

namespace Digger;
public enum Priority
{
    Terrain = 3,//Местность
    Digger = 2,//Копатель (Игрок)
    Sack = 0,//Мешок (с золотом)
    Gold = 4,//Золото
    Monster = 1//Монстр
}

public class Terrain : ICreature
{
    public string GetImageFileName()
    {
        return "Terrain.png";
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Terrain;
    }

    public CreatureCommand Act(int x, int y)
    {
        return new CreatureCommand();
    }

    
    public bool DeadInConflict(ICreature conflictedObject)
    {
        return conflictedObject is Player;
    }
}

public class Sack : ICreature
{
    public int FallHeight;
    public bool IsFalling;
    public bool WasFalling; // Состояние падения в предыдущем кадре

    public string GetImageFileName() => "Sack.png";
    public int GetDrawingPriority() => (int)Priority.Sack;

    public CreatureCommand Act(int x, int y)
    {
        WasFalling = IsFalling; // Сохраняем состояние падения
        IsFalling = false;

        var command = new CreatureCommand();
        bool canFall = CheckIfCanFall(x, y);

        if (canFall)
        {
            command.DeltaY = 1;
            FallHeight++;
            IsFalling = true;
        }
        else if (FallHeight > 0)
        {
            // Приземлился на Terrain, Sack, Gold или край карты
            if (FallHeight > 1)
                command.TransformTo = new Gold();
            FallHeight = 0;
        }

        return command;
    }

    private bool CheckIfCanFall(int x, int y)
    {
        if (y + 1 >= Game.MapHeight)
            return false; // Достигли дна

        var cellBelow = Game.Map[x, y + 1];
        // Падаем только если под нами пусто или игрок (и мешок уже падал)
        return cellBelow == null || (cellBelow is Player && WasFalling) || (cellBelow is Monster && WasFalling);
    }

    public bool DeadInConflict(ICreature conflictedObject) => false;
}


public class Player : ICreature
{
    public string GetImageFileName() => "Digger.png";
    public int GetDrawingPriority() => (int)Priority.Digger;

    public CreatureCommand Act(int x, int y)
    {
        var command = new CreatureCommand();
        SetDeltaValue(command);

        int newX = x + command.DeltaX;
        int newY = y + command.DeltaY;

        // Проверяем границы и отсутствие Sack в целевой клетке
        bool canMove = newX >= 0 && newX < Game.MapWidth &&
                       newY >= 0 && newY < Game.MapHeight &&
                       Game.Map[newX, newY] is not Sack;

        if (!canMove)
        {
            command.DeltaX = 0;
            command.DeltaY = 0;
        }
        else if ((command.DeltaX != 0 || command.DeltaY != 0) && Game.Map[x, y] is Terrain)
        {
            Game.Map[x, y] = null;
        }

        return command;
    }

    public bool DeadInConflict(ICreature conflictedObject) 
        => (conflictedObject is Sack sack && sack.WasFalling) || conflictedObject is Monster;

    private static void SetDeltaValue(CreatureCommand command)
    {
        switch (Game.KeyPressed)
        {
            case Key.Left:  command.DeltaX = -1; break;
            case Key.Right: command.DeltaX =  1; break;
            case Key.Up:    command.DeltaY = -1; break;
            case Key.Down:  command.DeltaY =  1; break;
        }
    }
}

public class Gold : ICreature
{
    public string GetImageFileName() => "Gold.png";
    public int GetDrawingPriority() => (int)Priority.Gold;

    public CreatureCommand Act(int x, int y)
    {
        // Золото не двигается
        return new CreatureCommand();
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        if (conflictedObject is Player)
        {
            Game.Scores += 10;
            return true; // Золото собирается игроком
        }
        return conflictedObject is Monster;
    }
}

public class Monster : ICreature
{
    private int _playerX = -1;
    private int _playerY = -1;
    private bool _isDead;
    public string GetImageFileName()
    {
        return "Monster.png";
    }

    public int GetDrawingPriority()
    {
        return (int)Priority.Monster;
    }

    public CreatureCommand Act(int x, int y)
    {
        var cmd = new CreatureCommand();
        if (_isDead || !FindPlayer()) 
            return cmd;

        var direction = GetMovementDirection(x, y, _playerX, _playerY);
        cmd.DeltaX = direction.dx;
        cmd.DeltaY = direction.dy;
    
        return cmd;
    }

// Основная логика выбора направления движения
    private (int dx, int dy) GetMovementDirection(int x, int y, int targetX, int targetY)
    {
        var deltaX = targetX - x;
        var deltaY = targetY - y;
    
        return ChoosePrimaryAxis(deltaX, deltaY) 
            ? TryHorizontalFirst(deltaX, deltaY, x, y) 
            : TryVerticalFirst(deltaX, deltaY, x, y);
    }

// Выбор приоритетной оси для движения
    private bool ChoosePrimaryAxis(int deltaX, int deltaY) 
        => Math.Abs(deltaX) >= Math.Abs(deltaY);

// Попытка движения сначала по горизонтали
    private (int dx, int dy) TryHorizontalFirst(int deltaX, int deltaY, int x, int y)
    {
        var move = TryMoveHorizontal(deltaX, x, y);
        return move.dx == 0 && move.dy == 0 
            ? TryMoveVertical(deltaY, x, y) 
            : move;
    }

// Попытка движения сначала по вертикали
    private (int dx, int dy) TryVerticalFirst(int deltaX, int deltaY, int x, int y)
    {
        var move = TryMoveVertical(deltaY, x, y);
        return move.dx == 0 && move.dy == 0 
            ? TryMoveHorizontal(deltaX, x, y) 
            : move;
    }

// Попытка горизонтального движения
    private (int dx, int dy) TryMoveHorizontal(int deltaX, int x, int y)
    {
        if (deltaX > 0 && CanMoveTo(x + 1, y)) return (1, 0);
        if (deltaX < 0 && CanMoveTo(x - 1, y)) return (-1, 0);
        return (0, 0);
    }

// Попытка вертикального движения
    private (int dx, int dy) TryMoveVertical(int deltaY, int x, int y)
    {
        if (deltaY > 0 && CanMoveTo(x, y + 1)) return (0, 1);
        if (deltaY < 0 && CanMoveTo(x, y - 1)) return (0, -1);
        return (0, 0);
    }

    public bool DeadInConflict(ICreature conflictedObject)
    {
        // Умираем ТОЛЬКО если мешок падал (WasFalling) 
        // или при столкновении с другими монстрами
        return (conflictedObject is Sack sack && sack.WasFalling)
               || conflictedObject is Monster;
    }
    
    private bool FindPlayer()
    {
        for (var x = 0; x < Game.MapWidth; x++)
            for (var y = 0; y < Game.MapHeight; y++)
                if (Game.Map[x, y] is Player)
                {
                    _playerX = x;
                    _playerY = y;
                    return true;
                }
        return false;
    }
    
    private bool CanMoveTo(int x, int y)
    {
        // Выход за границы карты
        if (x < 0 || x >= Game.MapWidth || y < 0 || y >= Game.MapHeight)
            return false;

        // Проверка содержимого клетки
        var cell = Game.Map[x, y];
        return cell switch
        {
            Terrain => false,
            Sack => false,
            Monster => false,
            null => true,
            Player => true,
            Gold => true,
        };
    }
}