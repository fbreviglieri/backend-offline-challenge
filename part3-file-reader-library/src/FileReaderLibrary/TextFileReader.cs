namespace FileReaderLibrary;

/// <summary>Reads a plain text file, returning its raw UTF-8 content.</summary>
public class TextFileReader : IFileReader
{
    public string Read(string path) => File.ReadAllText(path);
}
