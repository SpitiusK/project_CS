namespace Names;

internal static class HistogramTask
{
    public static HistogramData GetBirthsPerDayHistogram(NameData[] names, string name)
    {
        var numberNameByDay = new double[31];
        for (var i = 0; i < numberNameByDay.Length; i++)
            foreach (var nameData in names)
                if (nameData.Name == name && nameData.BirthDate.Day == i + 1 
                                          && nameData.BirthDate.Day > 1)
                    numberNameByDay[i] += 1;
        return new HistogramData(
            $"Рождаемость людей с именем '{name}'",
            GetDayValue(), numberNameByDay
            );
    }

    private static string[] GetDayValue()
    {
        var dayValue = new string[31];
        for (var i = 0; i < dayValue.Length; i++)
            dayValue[i] = (i + 1).ToString();
        return dayValue;
    }
}