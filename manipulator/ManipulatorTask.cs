using System;
using System.Linq;
using Avalonia;
using NUnit.Framework;
using static Manipulation.Manipulator;

namespace Manipulation;

public static class ManipulatorTask
{
	/// <summary>
	/// Возвращает массив углов (shoulder, elbow, wrist),
	/// необходимых для приведения эффектора манипулятора в точку x и y 
	/// с углом между последним суставом и горизонталью, равному alpha (в радианах)
	/// См. чертеж manipulator.png!
	/// </summary>
	public static double[] MoveManipulatorTo(double x, double y, double alpha)
	{
		// Используйте поля Forearm, UpperArm, Palm класса Manipulator
		var wristPos = CalculatePoint(new Point(x, y), Palm, Math.PI - alpha);
		var wristToShoulderDistance = CalculateDistance(wristPos, new Point(0, 0));
		
		var elbowToShoulderToWristAngle = TriangleTask.GetABAngle(UpperArm, wristToShoulderDistance, Forearm);
		var angleBetwenBeemAndOx = Math.Atan2(wristPos.Y, wristPos.X);
		
		
		var shoulder = angleBetwenBeemAndOx + elbowToShoulderToWristAngle;
		var elbow = TriangleTask.GetABAngle(UpperArm, Forearm, wristToShoulderDistance);
		
		var wrist = -alpha - shoulder - elbow;
		return new[] { shoulder, elbow, wrist };
	}
	
	/// <summary>
	/// Вычисляет координаты точки, смещённой от начальной позиции на заданную длину под указанным углом.
	/// </summary>
	/// <param name="startPoint">Начальная точка</param>
	/// <param name="length">Длина сегмента</param>
	/// <param name="angle">Угол в радианах относительно горизонтальной оси</param>
	/// <returns>Новая точка</returns>
	public static Point CalculatePoint(Point startPoint, double length, double angle)
	{
		return new Point(
			startPoint.X + length * Math.Cos(angle),
			startPoint.Y + length * Math.Sin(angle)
		);
	}
	
	/// <summary>
	/// Вычисляет расстояние между двумя точками.
	/// </summary>
	public static double CalculateDistance(Point a, Point b)
	{
		var dx = b.X - a.X;
		var dy = b.Y - a.Y;
		return Math.Sqrt(dx * dx + dy * dy);
	}
}



[TestFixture]
public class ManipulatorTask_Tests
{
    private const int TestCount = 1000;
    private const double Tolerance = 1e-5;
    private static readonly Random Random = new Random();

    [Test]
    public void TestMoveManipulatorTo()
    {
        var minRadius = Math.Abs(UpperArm - Forearm);
        var maxRadius = UpperArm + Forearm;
        
        for (int i = 0; i < TestCount; i++)
        {
            // Генерация случайных координат и угла
            var (x, y, alpha) = GenerateRandomInput();
            var wristPos = ManipulatorTask.CalculatePoint(new Point(x, y), Palm, Math.PI - alpha);
            var center = new Point(x - wristPos.X, y - wristPos.Y);
            var distanceToCenter = ManipulatorTask.CalculateDistance(new Point(x, y), center);
            var isReachable = IsPositionReachable(distanceToCenter, minRadius, maxRadius);
            ValidateResult(isReachable, x, y, alpha);
        }
    }

    private static (double x, double y, double alpha) GenerateRandomInput()
    {
        return (
            x: Random.Next(0, (int)(UpperArm + Forearm + Palm + 100)),
            y: Random.Next(0, (int)(UpperArm + Forearm + Palm + 100)),
            alpha: Random.NextDouble() * 2 * Math.PI
        );
    }
    
    private static bool IsPositionReachable(double distanceToCenter, double minRadius, double maxRadius)
    {
        return distanceToCenter >= minRadius - Tolerance 
               && distanceToCenter <= maxRadius + Tolerance;
    }

    private static void ValidateResult(bool isReachable, double x, double y, double alpha)
    {
	    var angleArray = ManipulatorTask.MoveManipulatorTo(x, y, alpha);
	    if (isReachable)
	    {
		    Assert.False(
			    angleArray.Any(double.IsNaN),
			    $"Ожидались что решение есть"
		    );
		    var joints = AnglesToCoordinatesTask.GetJointPositions(angleArray[0], angleArray[1], angleArray[2]);
		    Assert.AreEqual(x, joints[2].X, Tolerance, 
			    $"Ошибка в координате x\n Ожидалось: x = {x} y = {y} alpha = {alpha}\n" 
			    + $"Было: x = {joints[2].X} y = {joints[2].Y} alpha = {alpha}");
		    Assert.AreEqual(y, joints[2].Y, Tolerance, 
			    $"Ошибка в координате y\n Ожидалось: x = {x} y = {y} alpha = {alpha}\n" 
			    + $"Было: x = {joints[2].X} y = {joints[2].Y} alpha = {alpha}");
	    }
	    else
	    {
		    Assert.True(
			    angleArray.Any(double.IsNaN),
			    $"Ожидались что решений нет"
		    );
	    }
    }
}


