using FileReaderLibrary.Encryption;

namespace FileReaderLibrary;

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
