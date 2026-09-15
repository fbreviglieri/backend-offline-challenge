namespace FileReaderLibrary.Encryption;

/// <summary>Pass-through strategy for content that isn't actually encrypted.</summary>
public class NoOpEncryptionStrategy : IEncryptionStrategy
{
    public string Decrypt(string content) => content;
}
