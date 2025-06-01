using System;

namespace DistanceTask;

public static class DistanceTask
{
    public static double GetDistanceToSegment(double ax, double ay, double bx, double by, double x, double y)
    {
        
        var parametrT = ((x - ax) * (bx - ax) + (y - ay) * (by - ay)) 
                            / (Math.Pow(bx - ax, 2) + Math.Pow(by - ay, 2));
        var pointProjectionX = ax + parametrT * (bx - ax);
        var pointProjectionY = ay + parametrT * (by - ay);
        
        if (bx - ax == 0 && by - ay == 0)
            return CalculateDistanceBetweenTwoPoints(x, y, ax, ay);
        else if (parametrT < 0)
            return CalculateDistanceBetweenTwoPoints(x, y, ax, ay);
        else if (parametrT > 1)
            return CalculateDistanceBetweenTwoPoints(x, y, bx, by);
        return CalculateDistanceBetweenTwoPoints(x, y, pointProjectionX, pointProjectionY);
    }

    public static double CalculateDistanceBetweenTwoPoints(double xFirst, double yFirst, double xSecond, double ySecond)
    {
        return Math.Sqrt(Math.Pow(xFirst - xSecond, 2) 
                         + Math.Pow(yFirst - ySecond, 2));
    }
}