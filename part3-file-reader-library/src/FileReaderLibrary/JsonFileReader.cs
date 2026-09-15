using System.Text.Json;

namespace FileReaderLibrary;

/// <summary>
/// Parses JSON from an inner <see cref="IFileReader"/>'s output (defaulting to
/// <see cref="TextFileReader"/>), mirroring <see cref="XmlFileReader"/> so it can be composed
/// after decryption/role-security decorators.
/// </summary>
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
