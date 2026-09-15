using FileReaderLibrary.Security;
using Xunit;

namespace FileReaderLibrary.Tests;

public class RoleSecuredFileReaderDecoratorTests
{
    [Fact]
    public void Read_AllowedRole_DelegatesToInnerReader()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "content");

            var reader = new RoleSecuredFileReaderDecorator(new TextFileReader(), new SimpleRoleAuthorizationService(), "admin");

            Assert.Equal("content", reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Read_DisallowedRole_ThrowsAndNeverReadsFile()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "content");
            var reader = new RoleSecuredFileReaderDecorator(new XmlFileReader(), new SimpleRoleAuthorizationService(), "user");

            // "user" is only allowed .txt files (see SimpleRoleAuthorizationService); this is a
            // temp file with no extension, so it must be denied for any non-admin role.
            Assert.Throws<UnauthorizedAccessException>(() => reader.Read(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
