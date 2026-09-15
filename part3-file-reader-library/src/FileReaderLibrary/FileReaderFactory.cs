using FileReaderLibrary.Encryption;
using FileReaderLibrary.Security;

namespace FileReaderLibrary;

/// <summary>
/// Composes the right <see cref="IFileReader"/> chain for a requested file type plus
/// cross-cutting concerns (encryption, role-based security). The encryption/role-security
/// implementations are constructor-injected, so switching them (e.g. to a real encryption
/// algorithm or a real role system) never requires changing this class or the decorators —
/// only which implementation is passed in.
/// </summary>
public class FileReaderFactory
{
    private readonly IEncryptionStrategy _encryptionStrategy;
    private readonly IRoleAuthorizationService _roleAuthorizationService;

    public FileReaderFactory(IEncryptionStrategy encryptionStrategy, IRoleAuthorizationService roleAuthorizationService)
    {
        _encryptionStrategy = encryptionStrategy;
        _roleAuthorizationService = roleAuthorizationService;
    }

    public IFileReader Create(FileType fileType, bool encrypted, bool roleSecured, string? role = null)
    {
        IFileReader reader = CreateBaseReader(fileType);

        if (encrypted)
        {
            if (fileType != FileType.Text)
            {
                throw new NotSupportedException($"Encrypted reading is not yet supported for {fileType} files.");
            }

            reader = new EncryptedFileReaderDecorator(reader, _encryptionStrategy);
        }

        if (roleSecured)
        {
            if (fileType != FileType.Xml)
            {
                throw new NotSupportedException($"Role-based security is not yet supported for {fileType} files.");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("A role must be provided when roleSecured is true.", nameof(role));
            }

            reader = new RoleSecuredFileReaderDecorator(reader, _roleAuthorizationService, role);
        }

        return reader;
    }

    private static IFileReader CreateBaseReader(FileType fileType) => fileType switch
    {
        FileType.Text => new TextFileReader(),
        FileType.Xml => new XmlFileReader(),
        _ => throw new NotSupportedException($"File type {fileType} is not supported."),
    };
}
