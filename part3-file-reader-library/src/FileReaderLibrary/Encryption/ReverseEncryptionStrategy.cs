namespace FileReaderLibrary.Encryption;

/// <summary>
/// Illustrative "encryption": the encrypted form of a text is simply its characters reversed.
/// The PDF explicitly states the algorithm is of no concern.
/// </summary>
public class ReverseEncryptionStrategy : IEncryptionStrategy
{
    public string Decrypt(string content) => new(content.Reverse().ToArray());
}
