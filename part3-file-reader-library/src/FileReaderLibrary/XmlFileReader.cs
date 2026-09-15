using System.Xml.Linq;

namespace FileReaderLibrary;

/// <summary>Reads an XML file, returning its parsed content re-serialized as a string.</summary>
public class XmlFileReader : IFileReader
{
    public string Read(string path) => XDocument.Load(path).ToString();
}
