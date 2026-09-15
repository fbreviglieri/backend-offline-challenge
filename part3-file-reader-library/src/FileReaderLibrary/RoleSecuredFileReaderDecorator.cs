using FileReaderLibrary.Security;

namespace FileReaderLibrary;

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
