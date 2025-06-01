using System;

namespace Geometry
{
    public class Vector
    {
        public double X;
        public double Y;

        public double GetLength()
        {
            return Geometry.GetLength(this);
        }

        public Vector Add(Vector other)
        {
            return Geometry.Add(this, other);
        }

        public bool Belongs(Segment segment)
        {
            return Geometry.IsVectorInSegment(this, segment);
        }
    }

    public class Segment
    {
        public Vector Begin = new Vector();
        public Vector End = new Vector();

        public double GetLength()
        {
            return Geometry.GetLength(this);
        }

        public bool Contains(Vector vector)
        {
            return Geometry.IsVectorInSegment(vector, this);
        }
    }

    public static class Geometry
    {
        public static double GetLength(Vector vector)
        {
            return CalculateDistance(0, 0, vector.X, vector.Y);
        }

        public static double GetLength(Segment segment)
        {
            return CalculateDistance(
                segment.Begin.X, 
                segment.Begin.Y, 
                segment.End.X, 
                segment.End.Y
            );
        }

        private static double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector Add(Vector first, Vector second)
        {
            return new Vector 
            { 
                X = first.X + second.X,
                Y = first.Y + second.Y
            };
        }

        public static bool IsVectorInSegment(Vector vector, Segment segment)
        {
            var totalLength = GetLength(new Segment { Begin = segment.Begin, End = vector })
                            + GetLength(new Segment { Begin = vector, End = segment.End });
            
            return AreEqual(totalLength, GetLength(segment));
        }

        public static bool AreEqual(double a, double b, double epsilon = 1e-9)
        {
            var diff = Math.Abs(a - b);
            if (diff < epsilon) return true;

            var max = Math.Max(Math.Abs(a), Math.Abs(b));
            return diff <= epsilon * max;
        }
    }
}