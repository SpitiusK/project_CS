using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace TableParser;

[TestFixture]
public class FieldParserTaskTests
{
	public static void Test(string input, string[] expectedResult)
	{
		var actualResult = FieldsParserTask.ParseLine(input);
		Assert.AreEqual(expectedResult.Length, actualResult.Count);
		for (int i = 0; i < expectedResult.Length; ++i)
		{
			Assert.AreEqual(expectedResult[i], actualResult[i].Value);
		}
	}
	
	[TestCase("text", new[] {"text"})] // Одно поле
	[TestCase("hello world", new[] {"hello", "world"})] // Больше одного поля, разделитель длиной в один пробел
	[TestCase("hello   world", new[] {"hello", "world"})] // Разделитель длинной в несколько пробелов
	[TestCase(@"'hello ""world""'", new[] {"hello \"world\""})] // Двойные кавычки внутри одинарных
	[TestCase(@"""hello 'world'""", new[] {"hello 'world'"})] // Одинарные кавычки внутри двойных 
	[TestCase("''", new[] {""})] // Пустое поле
	[TestCase("", new string[] {})] // Нет полей
	[TestCase("'hello world", new[] {"hello world"})] // Нет закрывающей кавычки
	[TestCase(@"hello""world""", new[] {"hello", "world"})] // Поле в кавычках после простого поля
	[TestCase(@"""hello""world", new[] {"hello", "world"})] // Простое поле после поля в кавычках
	[TestCase(@"'hello \'world\''", new[] {"hello 'world'"})] // Экранированные одинарные кавычки внутри одинарных
	[TestCase(@"""hello \""world\""""", new[] {"hello \"world\""})] // Экранированные двойные кавычки внутри двойных
	[TestCase(@"'hello world\\'", new[] {"hello world\\"})] // Экранированный обратный слэш перед закрывающей кавычкой
	[TestCase(" 'hello world'", new[] {"hello world"})]  // Пробел в начале строки
	[TestCase("'hello world ", new[] {"hello world "})] // Пробел в конце строки с незакрытой кавычкой 

	public static void RunTests(string input, string[] expectedOutput)
	{
		Test(input, expectedOutput);
	}
}

public class FieldsParserTask
{
	public static List<Token> ParseLine(string line)
	{
		var resultTokensList = new List<Token>();
		for (var i = 0; i < line.Length; i++)
		{
			if (line[i] == ' ')
			{
				continue;
			}
			if (line[i] == '\'' || line[i] == '\"')
			{
				resultTokensList.Add(ReadQuotedField(line, i));
				i = resultTokensList[resultTokensList.Count - 1].GetIndexNextToToken() - 1;
			}
			else
			{
				resultTokensList.Add(ReadField(line, i));
				i = resultTokensList[resultTokensList.Count - 1].GetIndexNextToToken() - 1;
				
			}
		}
		return resultTokensList;
	}
        
	private static Token ReadField(string line, int startIndex)
	{
		var builder = new StringBuilder();
		var countIteration = startIndex;
		while (countIteration < line.Length)
		{
			if  (line[countIteration] == ' ' || line[countIteration] == '\'' || line[countIteration] == '\"') break;
			builder.Append(line[countIteration]);
			countIteration++;
		}
		var tokenValue = builder.ToString();
		var tokenLength = countIteration - startIndex;
		return new Token(tokenValue, startIndex, tokenLength);
	}

	private static int GetCountNextIterationAndAddToken(List<Token> resultTokensList, )
	{
		resultTokensList.Add(ReadField(line, i));
		i = resultTokensList[resultTokensList.Count - 1].GetIndexNextToToken() - 1;
	}
	public static Token ReadQuotedField(string line, int startIndex)
	{
		return QuotedFieldTask.ReadQuotedField(line, startIndex);
	}
}