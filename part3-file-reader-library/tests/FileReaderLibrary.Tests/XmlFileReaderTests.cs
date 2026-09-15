using FileReaderLibrary.Encryption;
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

    [Fact]
    public void Read_ParsesInnerReaderOutput_NotTheRawFileDirectly()
    {
        var path = Path.GetTempFileName();
        try
        {
            var xml = "<root><item>value</item></root>";
            File.WriteAllText(path, new string(xml.Reverse().ToArray())); // encrypted on disk

            var reader = new XmlFileReader(new EncryptedFileReaderDecorator(new TextFileReader(), new ReverseEncryptionStrategy()));

            Assert.Contains("<item>value</item>", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
