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
    public void Create_EncryptedXml_ReturnsDecryptedContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            var original = "<root><item>value</item></root>";
            File.WriteAllText(path, new string(original.Reverse().ToArray()));

            var reader = _factory.Create(FileType.Xml, encrypted: true, roleSecured: false);

            Assert.Contains("<item>value</item>", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
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
    public void Create_RoleSecuredText_AllowedRole_ReturnsContent()
    {
        var path = Path.Combine(Path.GetTempPath(), $"frl-test-{Guid.NewGuid():N}.txt");
        try
        {
            File.WriteAllText(path, "hello");

            // "user" is allowed to read .txt files (see SimpleRoleAuthorizationService).
            var reader = _factory.Create(FileType.Text, encrypted: false, roleSecured: true, role: "user");

            Assert.Equal("hello", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_RoleSecuredText_DisallowedRole_Throws()
    {
        var path = Path.Combine(Path.GetTempPath(), $"frl-test-{Guid.NewGuid():N}.txt");
        try
        {
            File.WriteAllText(path, "hello");

            // "guest" has no entry in SimpleRoleAuthorizationService's allow-list at all.
            var reader = _factory.Create(FileType.Text, encrypted: false, roleSecured: true, role: "guest");

            Assert.Throws<UnauthorizedAccessException>(() => reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_Json_ReturnsParsedContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, """{"a":1}""");

            var reader = _factory.Create(FileType.Json, encrypted: false, roleSecured: false);

            Assert.Contains("\"a\"", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_EncryptedJson_ReturnsDecryptedContent()
    {
        var path = Path.GetTempFileName();
        try
        {
            var original = """{"a":1}""";
            File.WriteAllText(path, new string(original.Reverse().ToArray()));

            var reader = _factory.Create(FileType.Json, encrypted: true, roleSecured: false);

            Assert.Contains("\"a\"", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Create_RoleSecuredJson_NotYetSupported_Throws()
    {
        Assert.Throws<NotSupportedException>(() => _factory.Create(FileType.Json, encrypted: false, roleSecured: true, role: "admin"));
    }
}
