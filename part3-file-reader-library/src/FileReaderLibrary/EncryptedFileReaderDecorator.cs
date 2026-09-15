using FileReaderLibrary.Encryption;

namespace FileReaderLibrary;

/// <summary>
/// Wraps any <see cref="IFileReader"/> and decrypts what it reads using the injected
/// <see cref="IEncryptionStrategy"/>. Works for any inner reader/format without modification —
/// enabling encryption for a new file format is purely a matter of composing this decorator
/// around that format's reader (see <see cref="FileReaderFactory"/>).
/// </summary>
public class EncryptedFileReaderDecorator : IFileReader
{
    private readonly IFileReader _innerReader;
    private readonly IEncryptionStrategy _encryptionStrategy;

    public EncryptedFileReaderDecorator(IFileReader innerReader, IEncryptionStrategy encryptionStrategy)
    {
        _innerReader = innerReader;
        _encryptionStrategy = encryptionStrategy;
    }

    public string Read(string path) => _encryptionStrategy.Decrypt(_innerReader.Read(path));
}
