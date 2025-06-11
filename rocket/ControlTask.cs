using System;

namespace func_rocket;

public class ControlTask
{
    public static Turn ControlRocket(Rocket rocket, Vector target)
    {
        // Возможен подбор более идеальных значений maxFactor, minFactor, maxDistance
        const double tolerance = 0.01;
        var currentDirection = rocket.Direction;
        var directionVector = target - rocket.Location;
        var distance = (target - rocket.Location).Length; // Расстояние до цели
        var maxFactor = 6.7;   // Максимальная коррекция
        var minFactor = 1.0;   // Минимальная коррекция
        var maxDistance = 150.0; // Расстояние, при котором factor максимален
        var factor = Math.Max(minFactor, maxFactor * (distance / maxDistance));
        var adjustedDirection = directionVector - rocket.Velocity * factor; // Новое направление
        var rightDirection = adjustedDirection.Angle;

        var angleDiff = Math.Atan2(Math.Sin(rightDirection - currentDirection), 
            Math.Cos(rightDirection - currentDirection));

		
        if (Math.Abs(angleDiff) < tolerance)
            return Turn.None;
        return angleDiff > 0 ? Turn.Right : Turn.Left;
    }
}