using FileReaderLibrary.Encryption;
using Xunit;

namespace FileReaderLibrary.Tests;

public class EncryptedFileReaderDecoratorTests
{
    [Fact]
    public void Read_DecryptsInnerReaderContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            var original = "hello world";
            File.WriteAllText(path, new string(original.Reverse().ToArray()));

            var reader = new EncryptedFileReaderDecorator(new TextFileReader(), new ReverseEncryptionStrategy());
            var content = reader.Read(path);

            Assert.Equal(original, content);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
