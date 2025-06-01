using System;

namespace Fractals;

internal static class DragonFractalTask
{
    public static void DrawDragonFractal(Pixels pixels, int iterationsCount, int seed)
    {
        var x = 1.0;
        var y = 0.0;
        var x1 = 0.0;
        var y1 = 0.0;
        var sqrtTwo = Math.Sqrt(2);
        var random = new Random(seed);
        
        for (var i = 0; i < iterationsCount + 1; i++)
        {
            if (random.Next(2) == 0)
            {
                x1 = GetCoordinateTurnAndCompression(true, x, y, Math.PI / 4, sqrtTwo);
                y1 = GetCoordinateTurnAndCompression(false, x, y, Math.PI / 4, sqrtTwo);
            }
            else
            {
                x1 = GetCoordinateTurnAndCompression(true, x, y, 3 * Math.PI / 4, sqrtTwo, 1);
                y1 = GetCoordinateTurnAndCompression(false, x, y, 3 * Math.PI / 4, sqrtTwo);
            }
            pixels.SetPixel(x, y);
            x = x1;
            y = y1;
        }
    }

    public static double GetCoordinateTurnAndCompression(bool turnX,double x, double y, double turnAngle, 
                                                         double nemberCompression, double constant = 0)
    {
        if (turnX)
            return (x * Math.Cos(turnAngle) - y * Math.Sin(turnAngle)) / nemberCompression + constant;
        return (x * Math.Sin(turnAngle) + y * Math.Cos(turnAngle)) / nemberCompression + constant;
    }
}