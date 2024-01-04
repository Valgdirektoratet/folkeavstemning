using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Shared.Export;

public static class ExportData
{
    public static string ExportCsv<T>(this IEnumerable<T> genericList)
    {
        var csvConfig = new CsvConfiguration(CultureInfo.CreateSpecificCulture("nb-NO"))
        {
            Delimiter = ";",
        };
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, csvConfig, true);
        csv.WriteRecords(genericList);
        csv.Flush();
        var newRecord = writer.ToString();
        return newRecord;
    }
}
