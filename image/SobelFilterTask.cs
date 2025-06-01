using System;
namespace Recognizer;
internal static class SobelFilterTask
{
    public static double[,] SobelFilter(double[,] imagePixels, double[,] sx)
    {
        var width = imagePixels.GetLength(0);
        var height = imagePixels.GetLength(1);
        var resultSobelFilter = new double[width, height];
        var sxLength = sx.GetLength(0);
        var sy = GetTransposed(sx);
        var sxRadius = sxLength / 2;

        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                if (x + sxRadius < width && y + sxRadius < height && x - sxRadius > -1 && y - sxRadius > -1)
                    resultSobelFilter[x, y] = GetSobelFilterValue(sx, sy, imagePixels, x, y, sxRadius);
        return resultSobelFilter;
    }

    private static double[,] GetTransposed(double[,] sx)
    {
        var sxLength = sx.GetLength(0);
        var transoseMatrix = new double[sxLength,sxLength];
        for(int i = 0; i < sxLength; i++)
            for(int j = 0; j < sxLength; j++)
                transoseMatrix[i, j] = sx[j, i];
        return transoseMatrix;
    }

    private static double GetSobelFilterValue(double[,] sx, double[,] sy, 
                        double[,] imagePixels, int x, int y, int sxRadius)
    {
        var gradientX = 0.0;
        var gradientY = 0.0;
        
        for (var i = -sxRadius; i <= sxRadius; i++)
            for (var j = -sxRadius; j <= sxRadius; j++)
            {
                gradientX += imagePixels[i + x, y + j] * sx[sxRadius + i, sxRadius + j];
                gradientY += imagePixels[i + x, y + j] * sy[sxRadius + i, sxRadius + j];
            }

        return Math.Sqrt(gradientX * gradientX + gradientY * gradientY);
    }
}