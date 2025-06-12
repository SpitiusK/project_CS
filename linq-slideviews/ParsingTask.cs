using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace linq_slideviews;

public class ParsingTask
{
	/// <param name="lines">все строки файла, которые нужно распарсить. Первая строка заголовочная.</param>
	/// <returns>Словарь: ключ — идентификатор слайда, значение — информация о слайде</returns>
	/// <remarks>Метод должен пропускать некорректные строки, игнорируя их</remarks>
	public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
	{
		return lines
			.Skip(1)
			.Select(line => line.Split(';'))
			.Where(lineArray => lineArray.Length == 3)
			.Select(lineArray =>
			{
				var  firstItemIsNumber = int.TryParse(lineArray[0], out var number);
				var isEnum = Enum.TryParse<SlideType>(lineArray[1], true, out var slideType);
				return new
				{
					hasNumber = firstItemIsNumber, 
					hasEnum = isEnum, valueNumber = number, 
					valueEnum = slideType, text = lineArray[2]
				};
			})
			.Where(lineValue => lineValue.hasNumber && lineValue.hasEnum)
			.ToDictionary(lineValue => lineValue.valueNumber, 
				lineValue => new SlideRecord(lineValue.valueNumber, 
					lineValue.valueEnum, lineValue.text));
	}

	/// <param name="lines">все строки файла, которые нужно распарсить. Первая строка — заголовочная.</param>
	/// <param name="slides">Словарь информации о слайдах по идентификатору слайда. 
	/// Такой словарь можно получить методом ParseSlideRecords</param>
	/// <returns>Список информации о посещениях</returns>
	/// <exception cref="FormatException">Если среди строк есть некорректные</exception>
	public static IEnumerable<VisitRecord> ParseVisitRecords(
		IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
	{
		return lines
			.Skip(1)
			.Select(line =>
			{
				var lineArray = line.Split(';');
				if (lineArray.Length != 4 || string.IsNullOrEmpty(lineArray[2]) || string.IsNullOrEmpty(lineArray[3])) 
					throw new FormatException($"Wrong line [{string.Join(";", lineArray)}]");
				var userIdIsNumber = int.TryParse(lineArray[0], out var userId);
				var slideIdIsNumber = int.TryParse(lineArray[1], out var slideId);
				var isDateTimeValid = DateTime.TryParseExact(string.Concat(lineArray[2], " ", lineArray[3]), "yyyy-MM-dd HH:mm:ss", 
																CultureInfo.InvariantCulture, DateTimeStyles.None, out var entryDateTime);
				if (!userIdIsNumber || !(slideIdIsNumber && slides.TryGetValue(slideId, out var slide)) || !isDateTimeValid)
					throw new FormatException($"Wrong line [{string.Join(";", lineArray)}]");
				return new VisitRecord(userId, slideId, entryDateTime, slide.SlideType);
			});
	}
}