using System.Text;
using NUnit.Framework;

namespace TableParser;

[TestFixture]
public class QuotedFieldTaskTests
{
    [TestCase("'", 0, "", 1)]
    [TestCase("'a'", 0, "a", 3)]
    [TestCase("\"abc\"", 0, "abc", 5)]
    [TestCase("b \"a'\"", 2, "a'", 4)]
    [TestCase("'a'b", 0, "a", 3)]
    [TestCase("a'b'", 1, "b", 3)]
    [TestCase(@"'a\' b'", 0, "a' b", 7)]
    [TestCase(@"some_text ""QF \"""" other_text", 10, "QF \"", 7)]
    public void Test(string line, int startIndex, string expectedValue, int expectedLength)
    {
        var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
        Assert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
    }
}

class QuotedFieldTask
{
    public static Token ReadQuotedField(string line, int startIndex)
    {
        var changeCount = startIndex;
        var builder = new StringBuilder();
        var endItem = line[startIndex];
        var tokenLength = 1;
        if (line.Length > 1)
            while (true)
            {
                changeCount++;
                if (changeCount == line.Length) break;
                tokenLength++;
                if (line[changeCount] == endItem) break;
                if (line[changeCount] == '\\')
                {
                    tokenLength++;
                    changeCount++;
                    builder.Append(line[changeCount]);
                    continue;
                }
                builder.Append(line[changeCount]);
            }
        var resultString = builder.ToString();
        return new Token(resultString, startIndex, tokenLength);
    }
}