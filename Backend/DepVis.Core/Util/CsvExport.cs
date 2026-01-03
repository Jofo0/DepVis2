using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DepVis.Core.Util;

public static class CsvExport
{
    public static async Task<MemoryStream> WriteToCsvStreamAsync<T>(
        IEnumerable<T> records,
        CsvConfiguration? config = null,
        Action<ClassMap<T>>? map = null
    )
    {
        var ms = new MemoryStream();
        var cfg =
            config ?? new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };

        await using var writer = new StreamWriter(ms, leaveOpen: true);
        await using var csv = new CsvWriter(writer, cfg);

        if (map is not null)
        {
            var m = new DefaultClassMap<T>();
            map(m);
            csv.Context.RegisterClassMap(m);
        }

        await csv.WriteRecordsAsync(records);
        await writer.FlushAsync();

        ms.Position = 0;
        return ms;
    }
}
