using System;
using NUnit.Framework;
namespace Manipulation;

public class TriangleTask
{
    /// <summary>
    /// Возвращает угол (в радианах) между сторонами a и b в треугольнике со сторонами a, b, c 
    /// </summary>
    public static double GetABAngle(double a, double b, double c)
    {
        // Проверка на недопустимые аргументы и невозможность треугольника
        if (a <= 0 || b <= 0 || c < 0 || c > a + b || a > b + c || b > a + c)
            return double.NaN;

        var numerator = a * a + b * b - c * c;
        var denominator = 2 * a * b;
        var cosGamma = numerator / denominator;

        return Math.Acos(cosGamma);
    }
}

[TestFixture]
public class TriangleTask_Tests
{
    [TestCase(0, 4, 5, double.NaN, TestName = "Нулевая сторона a (не треугольник)")]
    [TestCase(1, 1, 0, 0, TestName = "Нулевая сторона c (не треугольник)")]
    [TestCase(3, 4, 5, Math.PI / 2, TestName = "Прямоугольный треугольник (3-4-5)")]
    [TestCase(1, 1, 1, Math.PI / 3, TestName = "Равносторонний треугольник (углы 60°)")]
    [TestCase(2, 2, 4, Math.PI, TestName = "Вырожденный треугольник (угол 180°)")]
    [TestCase(1, 1, 1.41421356237, Math.PI / 2, TestName = "Равнобедренный прямоугольный (гипотенуза = √2)")]
    [TestCase(2, 3, 4, 1.82347658297, TestName = "Произвольный треугольник")]
    [TestCase(-3, 4, 5, double.NaN, TestName = "Отрицательная сторона a (не треугольник)")]
    [TestCase(1, 2, 3, Math.PI, TestName = "Вырожденный треугольник (1-2-3)")]
    [TestCase(3, 4, 6, 2.046916, TestName = "Тупоугольный треугольник (3-4-6)")]
    [TestCase(0, 0, 0, double.NaN, TestName = "Все стороны нулевые (не треугольник)")]
    [TestCase(5, 0, 5, double.NaN, TestName = "Нулевая сторона b (не треугольник)")]
    [TestCase(5, 5, 5, Math.PI / 3, TestName = "Равносторонний треугольник (5-5-5)")]
    
    public void TestGetABAngle(double a, double b, double c, double expectedAngle)
    {
        var actual = TriangleTask.GetABAngle(a, b, c);
        if (double.IsNaN(expectedAngle))
            Assert.IsTrue(double.IsNaN(actual));
        else
            Assert.AreEqual(expectedAngle, actual, 1e-5);
    }
}