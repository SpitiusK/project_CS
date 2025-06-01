namespace Mazes;

public static class SnakeMazeTask
{
    public static void MoveOut(Robot robot, int width, int height)
    {
        for (var i = 0; i < height / 2; i++)
        {
            if (i % 2 == 0)
                MoveToDirectionOnNumber(robot, Direction.Right, width - 3);
            else
                MoveToDirectionOnNumber(robot, Direction.Left, width - 3);
            if (!(robot.Finished))
                MoveToDirectionOnNumber(robot, Direction.Down, 2);
        }
    }
    
    public static void MoveToDirectionOnNumber(Robot robot, Direction direction, int number)
    {
        for (var i = 0; i < number; i++)
            robot.MoveTo(direction);
    }
}