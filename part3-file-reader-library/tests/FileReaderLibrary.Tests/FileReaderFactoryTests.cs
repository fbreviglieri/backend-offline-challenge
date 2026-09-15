using FileReaderLibrary.Encryption;
using FileReaderLibrary.Security;
using Xunit;

namespace FileReaderLibrary.Tests;

public class FileReaderFactoryTests
{
    private readonly FileReaderFactory _factory = new(new ReverseEncryptionStrategy(), new SimpleRoleAuthorizationService());

    [Fact]
    public void Create_EncryptedText_ReturnsDecryptedContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            var original = "secret plan";
            File.WriteAllText(path, new string(original.Reverse().ToArray()));

            var reader = _factory.Create(FileType.Text, encrypted: true, roleSecured: false);

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
        Assert.Throws<NotSupportedException>(() => _factory.Create(FileType.Xml, encrypted: true, roleSecured: false));
    }

    [Fact]
    public void Create_RoleSecuredXml_AdminCanRead()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "<root/>");

            var reader = _factory.Create(FileType.Xml, encrypted: false, roleSecured: true, role: "admin");

            Assert.Contains("root", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_RoleSecuredXml_DisallowedRole_Throws()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "<root/>");

            var reader = _factory.Create(FileType.Xml, encrypted: false, roleSecured: true, role: "user");

            Assert.Throws<UnauthorizedAccessException>(() => reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_RoleSecuredText_NotYetSupported_Throws()
    {
        Assert.Throws<NotSupportedException>(() => _factory.Create(FileType.Text, encrypted: false, roleSecured: true, role: "admin"));
    }
}
