using System;

namespace Mazes;

public static class EmptyMazeTask
{
    public static void MoveOut(Robot robot, int width, int height)
    {
        MoveToDirectionOnNumber(robot, Direction.Down, height - 3);
        MoveToDirectionOnNumber(robot, Direction.Right, width - 3);
    }

    public static void MoveToDirectionOnNumber(Robot robot, Direction direction, int number)
    {
        for (var i = 0; i < number; i++)
            robot.MoveTo(direction);
    }
}