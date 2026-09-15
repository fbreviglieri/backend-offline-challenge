using System.Text.Json;

namespace FileReaderLibrary;

// Parses an inner reader's output rather than the file path directly, so decryption/role-security
// decorators can sit between this and the raw file.
public class JsonFileReader : IFileReader
{
    private readonly IFileReader _innerReader;

    public JsonFileReader(IFileReader? innerReader = null)
    {
        _innerReader = innerReader ?? new TextFileReader();
    }

    public string Read(string path)
    {
        var raw = _innerReader.Read(path);
        using var document = JsonDocument.Parse(raw);
        return JsonSerializer.Serialize(document.RootElement);
    }
}
