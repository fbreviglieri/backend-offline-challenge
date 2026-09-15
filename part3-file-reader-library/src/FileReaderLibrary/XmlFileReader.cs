using System.Xml.Linq;

namespace FileReaderLibrary;

/// <summary>
/// Parses XML from an inner <see cref="IFileReader"/>'s output (defaulting to
/// <see cref="TextFileReader"/> — i.e. the raw file content). Deferring the raw read to an
/// inner reader — rather than loading the path directly — is what lets this be composed
/// *after* decryption/role-security decorators: those need to run on the raw bytes/gate access
/// before the content is meaningfully parsed as XML.
/// </summary>
public class XmlFileReader : IFileReader
{
    private readonly IFileReader _innerReader;

    public XmlFileReader(IFileReader? innerReader = null)
    {
        _innerReader = innerReader ?? new TextFileReader();
    }

    public string Read(string path) => XDocument.Parse(_innerReader.Read(path)).ToString();
}
