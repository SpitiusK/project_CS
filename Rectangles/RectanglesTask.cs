using System;

namespace Rectangles;

public static class RectanglesTask
{ 
    public static bool AreIntersected(Rectangle r1, Rectangle r2)
    {
        var intersectionX = (r1.Left <= r2.Left && r1.Right >= r2.Left) || (r1.Left <= r2.Right && r1.Right >= r2.Right);
        var intersectionY = (r1.Top <= r2.Top && r1.Bottom >= r2.Top) || (r1.Top <= r2.Bottom && r1.Bottom >= r2.Bottom);
        var enclosureX = (r1.Left >= r2.Left && r1.Right <= r2.Right) || (r2.Left >= r1.Left && r2.Right <= r1.Right);
        var enclosureY = (r1.Top >= r2.Top && r1.Bottom <= r2.Bottom) || (r2.Top >= r1.Top && r2.Bottom <= r1.Bottom);
        if ((intersectionX && intersectionY) || (intersectionX && enclosureY) || (enclosureX && intersectionY) || (enclosureX && enclosureY))
        {
            return true;
        }
        return false;
    }
    public static int IntersectionSquare(Rectangle r1, Rectangle r2)
    {
        if (AreIntersected(r1, r2))
        {
            return (Math.Min(r1.Right, r2.Right) - Math.Max(r1.Left, r2.Left)) * (Math.Min(r1.Bottom, r2.Bottom) - Math.Max(r1.Top, r2.Top));
        }
        return 0;
    }

    public static int IndexOfInnerRectangle(Rectangle r1, Rectangle r2)
    {
        var enclosureR1onR2 = (r1.Left >= r2.Left && r1.Right <= r2.Right) && (r1.Top >= r2.Top && r1.Bottom <= r2.Bottom);
        var enclosureR2onR1 = (r2.Left >= r1.Left && r2.Right <= r1.Right) && (r2.Top >= r1.Top && r2.Bottom <= r1.Bottom);
        if (enclosureR1onR2)
        {
            return 0;
        }
        else if(enclosureR2onR1)
        {
            return 1;
        }
        return -1;
    }
}