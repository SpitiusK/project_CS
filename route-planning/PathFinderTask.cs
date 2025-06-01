using System;
using System.Collections.Generic;
using System.Drawing;

namespace RoutePlanning;

public static class PathFinderTask
{
	public static int[] FindBestCheckpointsOrder(Point[] checkpoints)
	{
		var minLength = double.MaxValue;
		var allPermutations = new List<int[]>();
		var minLengthPermutation = new int[checkpoints.Length];
		AddPermutationInListAllPermutations(new int[checkpoints.Length], 0, allPermutations);
		
		
		foreach (var permutation in allPermutations)
		{
			var tryValue = checkpoints.GetPathLength(permutation);
			if (tryValue < minLength)
			{
				minLength = tryValue;
				minLengthPermutation = permutation;
			}
		}
		return minLengthPermutation;
	}
	
	
	static void AddPermutationInListAllPermutations(int[] permutation, int position, List<int[]> allPermutations)
	{
		if (position == permutation.Length)
		{
			allPermutations.Add(permutation.Clone() as int[]);
			return;
		}

		for (int i = 0; i < permutation.Length; i++)
		{
			var index = Array.IndexOf(permutation, i, 0, position);
			if (index != -1)
				continue;
			permutation[position] = i;
			if (permutation[0] == 0)
			{
				AddPermutationInListAllPermutations(permutation, position + 1, allPermutations);
			}
		}
	}
}