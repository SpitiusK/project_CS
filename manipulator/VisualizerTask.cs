using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;


namespace Manipulation;

/// <summary>
/// Класс для визуализации и управления манипулятором
/// </summary>
public static class VisualizerTask
{
    // Параметры манипулятора
    public static double X = 220;
    public static double Y = -100;
    public static double Alpha = 0.05;
    public static double Wrist = 2 * Math.PI / 3;
    public static double Elbow = 3 * Math.PI / 4;
    public static double Shoulder = Math.PI / 2;

    // Настройки отрисовки
    public static Brush UnreachableAreaBrush = new SolidColorBrush(Color.FromArgb(255, 255, 230, 230));
    public static Brush ReachableAreaBrush = new SolidColorBrush(Color.FromArgb(255, 230, 255, 230));
    public static Pen ManipulatorPen = new Pen(Brushes.Black, 3);
    public static Brush JointBrush = new SolidColorBrush(Colors.Gray);

    /// <summary>
    /// Обработка нажатия клавиш для управления углами суставов
    /// </summary>
    public static void KeyDown(Visual visual, KeyEventArgs key)
    {
        const double changeStep = 0.1;
        var angleChanged = HandleJointAdjustment(key.Key, changeStep);
    
        if (angleChanged)
        {
            Wrist = -Alpha - Shoulder - Elbow;
            visual.InvalidateVisual();
        }
    }

    /// <summary>
    /// Обработка изменения углов суставов через клавиатуру
    /// </summary>
    /// <returns>True если было произведено изменение углов</returns>
    private static bool HandleJointAdjustment(Key key, double step)
    {
        switch (key)
        {
            case Key.Q: AdjustShoulder(step); return true;
            case Key.A: AdjustShoulder(-step); return true;
            case Key.W: AdjustElbow(step); return true;
            case Key.S: AdjustElbow(-step); return true;
            default: return false;
        }
    }

    /// <summary>
    /// Корректировка угла плечевого сустава
    /// </summary>
    private static void AdjustShoulder(double delta)
    {
        Shoulder += delta;
    }

    /// <summary>
    /// Корректировка угла локтевого сустава
    /// </summary>
    private static void AdjustElbow(double delta)
    {
        Elbow += delta;
    }
    
    /// <summary>
    /// Обработка перемещения мыши для изменения позиции манипулятора
    /// </summary>
    public static void MouseMove(Visual visual, PointerEventArgs e)
    {
        var shoulderPos = GetShoulderPos(visual);
        var windowToMath = ConvertWindowToMath(e.GetPosition(visual), shoulderPos);
        
        X = windowToMath.X;
        Y = windowToMath.Y;
        
        UpdateManipulator();
        visual.InvalidateVisual();
    }

    /// <summary>
    /// Обработка прокрутки колеса мыши для изменения угла Alpha
    /// </summary>
    public static void MouseWheel(Visual visual, PointerWheelEventArgs e)
    {
        Alpha += e.Delta.Y * 0.1;
        UpdateManipulator();
        visual.InvalidateVisual();
    }

    /// <summary>
    /// Обновление углов суставов на основе текущей позиции
    /// </summary>
    public static void UpdateManipulator()
    {
        var angles = ManipulatorTask.MoveManipulatorTo(X, Y, Alpha);
        if (!angles.Any(double.IsNaN))
        {
            Shoulder = angles[0];
            Elbow = angles[1];
            Wrist = angles[2];
        }
    }

    /// <summary>
    /// Отрисовка всего манипулятора и информационной панели
    /// </summary>
    public static void DrawManipulator(DrawingContext context, Point shoulderPos)
    {
        var joints = AnglesToCoordinatesTask.GetJointPositions(Shoulder, Elbow, Wrist);
        DrawReachableZone(context, ReachableAreaBrush, UnreachableAreaBrush, shoulderPos, joints);

        // Отрисовка информационного текста
        var statusText = new FormattedText(
            $"X={X:0}, Y={Y:0}, Alpha={Alpha:0.00}",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            18,
            Brushes.DarkRed)
        {
            TextAlignment = TextAlignment.Center
        };
        context.DrawText(statusText, new Point(10, 10));

        DrawManipulatorSegments(context, shoulderPos, joints);
    }

    /// <summary>
    /// Отрисовка сегментов манипулятора (плечо, предплечье, кисть)
    /// </summary>
    private static void DrawManipulatorSegments(DrawingContext context, Point shoulderPos, Point[] joints)
    {
        var elbowPos = ConvertMathToWindow(joints[0], shoulderPos);
        var wristPos = ConvertMathToWindow(joints[1], shoulderPos);
        var grabPos = ConvertMathToWindow(joints[2], shoulderPos);

        // Отрисовка суставов и соединительных линий
        context.DrawEllipse(JointBrush, null, shoulderPos, 10, 10);
        context.DrawLine(ManipulatorPen, shoulderPos, elbowPos);
        context.DrawEllipse(JointBrush, null, elbowPos, 10, 10);
        context.DrawLine(ManipulatorPen, elbowPos, wristPos);
        context.DrawEllipse(JointBrush, null, wristPos, 10, 10);
        context.DrawLine(ManipulatorPen, wristPos, grabPos);
    }

    /// <summary>
    /// Отрисовка зоны досягаемости манипулятора
    /// </summary>
    private static void DrawReachableZone(
        DrawingContext context,
        Brush reachableBrush,
        Brush unreachableBrush,
        Point shoulderPos,
        Point[] joints)
    {
        // Вычисление радиусов зоны досягаемости
        var minRadius = Math.Abs(Manipulator.UpperArm - Manipulator.Forearm);
        var maxRadius = Manipulator.UpperArm + Manipulator.Forearm;
        
        var center = ConvertMathToWindow(
            new Point(joints[2].X - joints[1].X, joints[2].Y - joints[1].Y), 
            shoulderPos);

        // Отрисовка внешней (доступной) и внутренней (недоступной) зон
        context.DrawEllipse(reachableBrush, null, center, maxRadius, maxRadius);
        context.DrawEllipse(unreachableBrush, null, center, minRadius, minRadius);
    }

    /// <summary>
    /// Получение позиции основания манипулятора (плеча)
    /// </summary>
    public static Point GetShoulderPos(Visual visual)
    {
        return new Point(visual.Bounds.Width / 2, visual.Bounds.Height / 2);
    }

    /// <summary>
    /// Преобразование математических координат в оконные
    /// </summary>
    public static Point ConvertMathToWindow(Point mathPoint, Point shoulderPos)
    {
        return new Point(mathPoint.X + shoulderPos.X, shoulderPos.Y - mathPoint.Y);
    }

    /// <summary>
    /// Преобразование оконных координат в математические
    /// </summary>
    public static Point ConvertWindowToMath(Point windowPoint, Point shoulderPos)
    {
        return new Point(windowPoint.X - shoulderPos.X, shoulderPos.Y - windowPoint.Y);
    }
}