using Xunit;

namespace FileReaderLibrary.Tests;

public class XmlFileReaderTests
{
    [Fact]
    public void Read_ReturnsParsedXmlContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "<root><item>value</item></root>");

            var content = new XmlFileReader().Read(path);

            Assert.Contains("<item>value</item>", content);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
