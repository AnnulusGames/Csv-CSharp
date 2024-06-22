namespace Csv.Tests;

public class CsvDocumentTests
{
    [Test]
    public void Test_CsvDocument_Index()
    {
        var csv =
@"Name,Age
Alex,21  
Bob,35
Charles,17"u8;

        var document = CsvSerializer.ConvertToDocument(csv.ToArray());

        Assert.That(document.Header.Length, Is.EqualTo(2));
        Assert.That(document.Header[0].GetValue<string>(), Is.EqualTo("Name"));
        Assert.That(document.Header[1].GetValue<string>(), Is.EqualTo("Age"));

        Assert.That(document.Rows[0].Length, Is.EqualTo(2));
        Assert.That(document.Rows[0][0].GetValue<string>(), Is.EqualTo("Alex"));
        Assert.That(document.Rows[0][1].GetValue<int>(), Is.EqualTo(21));

        Assert.That(document.Rows[1].Length, Is.EqualTo(2));
        Assert.That(document.Rows[1][0].GetValue<string>(), Is.EqualTo("Bob"));
        Assert.That(document.Rows[1][1].GetValue<int>(), Is.EqualTo(35));

        Assert.That(document.Rows[2].Length, Is.EqualTo(2));
        Assert.That(document.Rows[2][0].GetValue<string>(), Is.EqualTo("Charles"));
        Assert.That(document.Rows[2][1].GetValue<int>(), Is.EqualTo(17));
    }

    [Test]
    public void Test_CsvDocument_Key()
    {
        var csv =
@"Name,Age
Alex,21  
Bob,35
Charles,17"u8;

        var document = CsvSerializer.ConvertToDocument(csv.ToArray());

        Assert.That(document.Rows[0].Length, Is.EqualTo(2));
        Assert.That(document.Rows[0]["Name"].GetValue<string>(), Is.EqualTo("Alex"));
        Assert.That(document.Rows[0]["Age"].GetValue<int>(), Is.EqualTo(21));

        Assert.That(document.Rows[1].Length, Is.EqualTo(2));
        Assert.That(document.Rows[1]["Name"].GetValue<string>(), Is.EqualTo("Bob"));
        Assert.That(document.Rows[1]["Age"].GetValue<int>(), Is.EqualTo(35));

        Assert.That(document.Rows[2].Length, Is.EqualTo(2));
        Assert.That(document.Rows[2]["Name"].GetValue<string>(), Is.EqualTo("Charles"));
        Assert.That(document.Rows[2]["Age"].GetValue<int>(), Is.EqualTo(17));
    }
}