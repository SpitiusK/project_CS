using System;
using Avalonia;
using NUnit.Framework;
using static Manipulation.Manipulator;

namespace Manipulation;

public static class AnglesToCoordinatesTask
{
	/// <summary>
	/// По значению углов суставов возвращает массив координат суставов
	/// в порядке new []{elbow, wrist, palmEnd}
	/// </summary>
	public static Point[] GetJointPositions(double shoulder, double elbow, double wrist)
	{
		var elbowPos = CalculatePoint(new Point(0, 0), UpperArm, shoulder);
		var wristPos = CalculatePoint(elbowPos, Forearm, shoulder + elbow - Math.PI);
		var palmEndPos = CalculatePoint(wristPos, Palm, shoulder + elbow + wrist - 2 * Math.PI);
        
		return new[] { elbowPos, wristPos, palmEndPos };
	}

	/// <summary>
	/// Вычисляет координаты точки, смещённой от начальной позиции на заданную длину под указанным углом.
	/// </summary>
	/// <param name="startPoint">Начальная точка</param>
	/// <param name="length">Длина сегмента</param>
	/// <param name="angle">Угол в радианах относительно горизонтальной оси</param>
	/// <returns>Новая точка</returns>
	private static Point CalculatePoint(Point startPoint, double length, double angle)
	{
		return new Point(
			startPoint.X + length * Math.Cos(angle),
			startPoint.Y + length * Math.Sin(angle)
		);
	}
}

[TestFixture]
public class AnglesToCoordinatesTask_Tests
{
	// Плечо: 90° (вертикально вверх),
	// Локоть: 90° (предплечье согнуто вверх под прямым углом),
	// Запястье: 180° (ладонь направлена вправо)
	[TestCase(
		Math.PI / 2,
		Math.PI / 2,
		Math.PI,
		Forearm + Palm,
		UpperArm
	)]

	// Плечо: 0° (вдоль оси X),
	// Локоть: 180° (предплечье продолжает линию плеча),
	// Запястье: 180° (ладонь продолжает линию предплечья)
	[TestCase(
		0,
		Math.PI,
		Math.PI,
		UpperArm + Forearm + Palm,
		0
	)]

	// Плечо: 180° (вдоль оси X влево),
	// Локоть: 180° (предплечье направлено влево),
	// Запястье: 180° (ладонь направлена влево)
	[TestCase(
		Math.PI,
		Math.PI,
		Math.PI,
		-UpperArm - Forearm - Palm,
		0
	)]

	// Плечо: 90° (вертикально вверх),
	// Локоть: 180° (предплечье продолжает вертикаль),
	// Запястье: 180° (ладонь продолжает вертикаль)
	[TestCase(
		Math.PI / 2,
		Math.PI,
		Math.PI,
		0,
		UpperArm + Forearm + Palm
	)]
	
	public void TestGetJointPositions(
		double shoulder, 
		double elbow, 
		double wrist, 
		double expectedPalmEndX, 
		double expectedPalmEndY)
	{
		var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);
		var palmEndPos = joints[2];
    
		Assert.AreEqual(expectedPalmEndX, palmEndPos.X, 1e-5, "palm endX");
		Assert.AreEqual(expectedPalmEndY, palmEndPos.Y, 1e-5, "palm endY");
    
		// Проверка длин сегментов
		Assert.AreEqual(UpperArm, Distance(new Point(0, 0), joints[0]), 1e-5, "UpperArm");
		Assert.AreEqual(Forearm, Distance(joints[0], joints[1]), 1e-5, "Forearm");
		Assert.AreEqual(Palm, Distance(joints[1], joints[2]), 1e-5, "Palm");
	}

// Вспомогательный метод для расчета расстояния
	private static double Distance(Point a, Point b)
	{
		return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
	}
}