namespace FileReaderLibrary.Security;

public interface IRoleAuthorizationService
{
    bool CanRead(string role, string filePath);
}
