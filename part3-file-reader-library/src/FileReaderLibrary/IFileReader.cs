namespace FileReaderLibrary;

/// <summary>Reads the content of a file at the given path as a string.</summary>
public interface IFileReader
{
    string Read(string path);
}
