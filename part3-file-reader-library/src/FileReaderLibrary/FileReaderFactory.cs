using FileReaderLibrary.Encryption;
using FileReaderLibrary.Security;

namespace FileReaderLibrary;

// Composition order (innermost first): raw read -> role gate -> decryption -> format parsing.
// Role-security and decryption must run before a format parser, since it needs already-decrypted
// plaintext to parse.
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
            FileType.Json => new JsonFileReader(reader),
            _ => throw new NotSupportedException($"File type {fileType} is not supported."),
        };
    }
}
