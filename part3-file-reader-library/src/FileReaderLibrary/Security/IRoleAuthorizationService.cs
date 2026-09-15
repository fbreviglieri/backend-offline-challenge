namespace FileReaderLibrary.Security;

/// <summary>
/// Decides whether a role may read a given file. Implementations are swapped via constructor
/// injection (see <see cref="FileReaderFactory"/>) — switching to a real role-based security
/// system never requires changing <see cref="RoleSecuredFileReaderDecorator"/> or any reader.
/// </summary>
public interface IRoleAuthorizationService
{
    bool CanRead(string role, string filePath);
}
