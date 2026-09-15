using Xunit;

namespace FileReaderLibrary.Tests;

public class JsonFileReaderTests
{
    [Fact]
    public void Read_ReturnsParsedJsonContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, """{"name":"value"}""");

            var content = new JsonFileReader().Read(path);

            Assert.Contains("\"name\"", content);
            Assert.Contains("\"value\"", content);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
