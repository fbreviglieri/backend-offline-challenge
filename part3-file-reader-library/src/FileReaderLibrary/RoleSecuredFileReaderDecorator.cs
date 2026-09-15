using FileReaderLibrary.Security;

namespace FileReaderLibrary;

/// <summary>
/// Wraps any <see cref="IFileReader"/> and enforces role-based access before delegating to it.
/// Works for any inner reader/format without modification — enabling role-based security for a
/// new file format is purely a matter of composing this decorator around that format's reader
/// (see <see cref="FileReaderFactory"/>).
/// </summary>
public class RoleSecuredFileReaderDecorator : IFileReader
{
    private readonly IFileReader _innerReader;
    private readonly IRoleAuthorizationService _roleAuthorizationService;
    private readonly string _role;

    public RoleSecuredFileReaderDecorator(IFileReader innerReader, IRoleAuthorizationService roleAuthorizationService, string role)
    {
        _innerReader = innerReader;
        _roleAuthorizationService = roleAuthorizationService;
        _role = role;
    }

    public string Read(string path)
    {
        if (!_roleAuthorizationService.CanRead(_role, path))
        {
            throw new UnauthorizedAccessException($"Role '{_role}' is not permitted to read '{path}'.");
        }

        return _innerReader.Read(path);
    }
}
