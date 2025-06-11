using System;
using System.Collections.Generic;

namespace func_rocket;

public class LevelsTask
{
	static readonly Physics standardPhysics = new();

	private static Level CreateLevel(string name, Gravity gravity, Vector target = null)
	{
		var defaultRocket = new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI);
		var defaultTarget = new Vector(600, 200);
		return new Level(name, defaultRocket, target ?? defaultTarget, gravity, standardPhysics);
	}

	private static Gravity WhiteHoleGravity(Vector target)
	{
		return (size, v) =>
		{
			var d = (target - v).Length;
			return (target - v).Normalize() * (-140 * d) / (d * d + 1);
		};
	}

	private static Gravity BlackHoleGravity(Vector target, Vector rocketStart)
	{
		var anomaly = (target + rocketStart) / 2;
		return (size, v) =>
		{
			var d = (anomaly - v).Length;
			return (anomaly - v).Normalize() * (300 * d) / (d * d + 1);
		};
	}

	public static IEnumerable<Level> CreateLevels()
	{
		yield return CreateLevel("Zero", (size, v) => Vector.Zero);
		yield return CreateLevel("Heavy", (size, v) => new Vector(0, 0.9));
		yield return CreateLevel("Up", (size, v) => new Vector(0, -300 / (size.Y - v.Y + 300)), new Vector(700, 500));
		yield return CreateLevel("WhiteHole", WhiteHoleGravity(new Vector(600, 200)));
		yield return CreateLevel("BlackHole", BlackHoleGravity(new Vector(600, 200), new Vector(200, 500)));
		yield return CreateLevel("BlackAndWhite",
			(size, v) => (WhiteHoleGravity(new Vector(600, 200))(size, v) + 
			              BlackHoleGravity(new Vector(600, 200), new Vector(200, 500))(size, v)) / 2);
	}
}