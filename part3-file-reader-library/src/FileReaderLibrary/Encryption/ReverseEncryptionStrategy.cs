namespace FileReaderLibrary.Encryption;

// Intentionally trivial — a stand-in encryption algorithm, not a real one.
public class ReverseEncryptionStrategy : IEncryptionStrategy
{
    public string Decrypt(string content) => new(content.Reverse().ToArray());
}
