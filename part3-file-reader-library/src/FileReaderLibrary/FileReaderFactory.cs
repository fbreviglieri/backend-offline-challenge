using FileReaderLibrary.Encryption;
using FileReaderLibrary.Security;

namespace FileReaderLibrary;

/// <summary>
/// Composes the right <see cref="IFileReader"/> chain for a requested file type plus
/// cross-cutting concerns (encryption, role-based security). The encryption/role-security
/// implementations are constructor-injected, so switching them (e.g. to a real encryption
/// algorithm or a real role system) never requires changing this class or the decorators —
/// only which implementation is passed in.
///
/// Composition order (innermost first): raw text read -> role-based gate -> decryption ->
/// format-specific parsing. Role-security and decryption operate on the file's raw bytes, so
/// they must run before a format parser (e.g. XML) attempts to interpret the content.
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
        IFileReader reader = new TextFileReader();

        if (roleSecured)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("A role must be provided when roleSecured is true.", nameof(role));
            }

            reader = new RoleSecuredFileReaderDecorator(reader, _roleAuthorizationService, role);
        }

        if (encrypted)
        {
            reader = new EncryptedFileReaderDecorator(reader, _encryptionStrategy);
        }

        return fileType switch
        {
            FileType.Text => reader,
            FileType.Xml => new XmlFileReader(reader),
            _ => throw new NotSupportedException($"File type {fileType} is not supported."),
        };
    }
}
