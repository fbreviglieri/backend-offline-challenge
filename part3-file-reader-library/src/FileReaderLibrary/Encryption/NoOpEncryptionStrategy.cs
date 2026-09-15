namespace FileReaderLibrary.Encryption;

public class NoOpEncryptionStrategy : IEncryptionStrategy
{
    public string Decrypt(string content) => content;
}
