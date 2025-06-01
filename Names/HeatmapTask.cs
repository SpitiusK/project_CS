namespace Names;

internal static class HeatmapTask
{
    public static HeatmapData GetBirthsPerDateHeatmap(NameData[] names)
    {
        var birthRate = new double[30, 12];
        foreach (var name in names)
            if (name.BirthDate.Day > 1)
                birthRate[name.BirthDate.Day - 2, name.BirthDate.Month - 1] += 1;
        return new HeatmapData(
            "Пример карты интенсивностей",
            birthRate,
            GetArrayWithOffset(2, 30),
            GetArrayWithOffset(1, 12));
    }

    private static string[] GetArrayWithOffset(int offset, int length)
    {
        var offsetArray = new string[length];
        for (var i = 0; i < length; i++)
            offsetArray[i] = (i + offset).ToString();
        return offsetArray;
    }
}