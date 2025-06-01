using System;

namespace Mazes;

public static class DiagonalMazeTask
{
    public static void MoveOut(Robot robot, int width, int height)
    {
        var widthLarger = width > height;
        var numberSteps = (int)Math.Round((double)Math.Max(width, height) / Math.Min(width, height));
        while (!(robot.Finished))
            MoveOnDiagonal(robot, widthLarger, numberSteps);
    }

    public static void MoveToDirectionOnNumber(Robot robot, Direction direction, int number)
    {
        for (var i = 0; i < number; i++)
            robot.MoveTo(direction);
    }

    public static void MoveOnDiagonal(Robot robot, bool widthLarger, int numberSteps)
    {
        if (widthLarger)
            MoveToDirectionOnNumber(robot, Direction.Right, numberSteps);
        else
            MoveToDirectionOnNumber(robot, Direction.Down, numberSteps);
        if (!robot.Finished && widthLarger)
            robot.MoveTo(Direction.Down);
        else if (!robot.Finished)
            robot.MoveTo(Direction.Right);
    }
}