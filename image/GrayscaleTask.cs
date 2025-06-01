namespace Recognizer;

public static class GrayscaleTask
{
	public static double[,] ToGrayscale(Pixel[,] original)
	{
		var row = original.GetLength(0);
		var col = original.GetLength(1);
		var grayscale = new double[row, col];
		for (var x = 0; x < row; x++)
			for (var y = 0; y < col; y++)
			{
				var curentOrdinalVelue = original[x, y];
				grayscale[x, y] = (curentOrdinalVelue.R * 0.299 + curentOrdinalVelue.G * 0.587 
				                                                + curentOrdinalVelue.B * 0.114) / 255.0;
			}
		return grayscale;
	}
}
