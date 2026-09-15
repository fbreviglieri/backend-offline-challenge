namespace FileReaderLibrary.Encryption;

/// <summary>
/// Decrypts file content. Implementations are swapped via constructor injection
/// (see <see cref="FileReaderFactory"/>) — switching encryption algorithm never
/// requires changing <see cref="EncryptedFileReaderDecorator"/> or any reader.
/// </summary>
public interface IEncryptionStrategy
{
    string Decrypt(string content);
}
