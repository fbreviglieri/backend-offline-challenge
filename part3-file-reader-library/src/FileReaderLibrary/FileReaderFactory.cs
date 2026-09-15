using FileReaderLibrary.Encryption;

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

    public FileReaderFactory(IEncryptionStrategy encryptionStrategy)
    {
        _encryptionStrategy = encryptionStrategy;
    }

    public IFileReader Create(FileType fileType, bool encrypted)
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

        return reader;
    }

    private static IFileReader CreateBaseReader(FileType fileType) => fileType switch
    {
        FileType.Text => new TextFileReader(),
        FileType.Xml => new XmlFileReader(),
        _ => throw new NotSupportedException($"File type {fileType} is not supported."),
    };
}
