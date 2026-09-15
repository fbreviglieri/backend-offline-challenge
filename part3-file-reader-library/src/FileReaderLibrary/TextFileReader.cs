namespace FileReaderLibrary;

public class TextFileReader : IFileReader
{
    public string Read(string path) => File.ReadAllText(path);
}
