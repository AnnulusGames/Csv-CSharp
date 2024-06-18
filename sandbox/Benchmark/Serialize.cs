using BenchmarkDotNet.Attributes;
using Csv;
using CsvHelper.Configuration;
using nietras.SeparatedValues;
using System.Globalization;

[Config(typeof(BenchmarkConfig))]
public class Serialize
{
    public static readonly Person[] Data = CsvSerializer.Deserialize<Person>(CsvData.Utf8Text1);

    CsvConfiguration config = default!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        CsvSerializer.DefaultOptions = new()
        {
            AllowComments = false,
        };
        config = new(CultureInfo.InvariantCulture)
        {
            AllowComments = false,
        };
    }

    [Benchmark(Description = "Csv-CSharp")]
    public byte[] Serialize_CsvCSharp()
    {
        return CsvSerializer.Serialize(Data);
    }

    [Benchmark(Description = "ServiceStack.Text")]
    public string Serialize_ServiceStackText()
    {
        return ServiceStack.Text.CsvSerializer.SerializeToCsv(Data);
    }

    [Benchmark(Description = "CsvHelper")]
    public void Serialize_CsvHelper()
    {
        using var writer = new CsvHelper.CsvWriter(new StringWriter(), config);
        writer.WriteRecords(Data);
    }

    [Benchmark(Description = "Sep")]
    public void Serialize_Sep()
    {
        using var writer = Sep.Writer().ToText();
        writer.Header.Add(["Name", "Age"]);
        foreach (var item in Data)
        {
            using var row = writer.NewRow();
            row["Name"].Set(item.Name);
            row["Age"].Set($"{item.Age}");
        }
    }
}