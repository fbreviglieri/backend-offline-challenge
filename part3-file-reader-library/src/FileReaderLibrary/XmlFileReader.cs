using System.Xml.Linq;

namespace FileReaderLibrary;

// Parses an inner reader's output rather than the file path directly, so decryption/role-security
// decorators can sit between this and the raw file.
public class XmlFileReader : IFileReader
{
    private readonly IFileReader _innerReader;

    public XmlFileReader(IFileReader? innerReader = null)
    {
        _innerReader = innerReader ?? new TextFileReader();
    }

    public string Read(string path) => XDocument.Parse(_innerReader.Read(path)).ToString();
}
