namespace FileReaderLibrary.Encryption;

public interface IEncryptionStrategy
{
    string Decrypt(string content);
}
