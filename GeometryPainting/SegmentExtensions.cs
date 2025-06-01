using System.Collections.Generic;
using Avalonia.Media;
using Geometry;


namespace GeometryPainting;

public static class SegmentExtensions
{
    private static Dictionary<Segment, Color> segmentsColor = new ();

    public static void SetColor(this Segment segment, Color color)
    {
        segmentsColor[segment] = color;
    }

    public static Color GetColor(this Segment segment)
    {
        return segmentsColor.TryGetValue(segment, out var color) ? color : Colors.Black;
    }
}