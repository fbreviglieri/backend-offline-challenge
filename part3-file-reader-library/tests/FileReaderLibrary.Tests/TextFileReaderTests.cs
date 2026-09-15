using Xunit;

namespace FileReaderLibrary.Tests;

public class TextFileReaderTests
{
    [Fact]
    public void Read_ReturnsFileContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "hello world");

            var content = new TextFileReader().Read(path);

            Assert.Equal("hello world", content);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
