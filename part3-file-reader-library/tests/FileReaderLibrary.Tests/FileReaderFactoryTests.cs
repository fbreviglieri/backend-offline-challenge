using FileReaderLibrary.Encryption;
using Xunit;

namespace FileReaderLibrary.Tests;

public class FileReaderFactoryTests
{
    private readonly FileReaderFactory _factory = new(new ReverseEncryptionStrategy());

    [Fact]
    public void Create_EncryptedText_ReturnsDecryptedContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            var original = "secret plan";
            File.WriteAllText(path, new string(original.Reverse().ToArray()));

            var reader = _factory.Create(FileType.Text, encrypted: true);

            Assert.Equal(original, reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_EncryptedXml_NotYetSupported_Throws()
    {
        Assert.Throws<NotSupportedException>(() => _factory.Create(FileType.Xml, encrypted: true));
    }
}
