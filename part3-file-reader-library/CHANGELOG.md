# Changelog

## v1 — Read a text file

User story: "A user should be able to read a text file"

Added `IFileReader` (the core abstraction: `string Read(string path)`) and `TextFileReader`,
which reads a text file's raw content.

## v2 — Read an XML file

User story: "A user should be able to read an XML file"

Added `XmlFileReader`, which parses the file with `XDocument` and returns its content.

## v3 — Read an encrypted text file

User story: "A user should be able to read an encrypted TEXT file" (algorithm is irrelevant;
must be swappable without changing code)

Added `IEncryptionStrategy` + `ReverseEncryptionStrategy` (reverses the text — a simple
stand-in, per the user story) + `NoOpEncryptionStrategy`, and `EncryptedFileReaderDecorator`,
which wraps any `IFileReader` and decrypts what it reads. Introduced `FileReaderFactory` /
`FileType`, wiring the decorator for TEXT files. The strategy is constructor-injected, so
swapping the algorithm never touches the decorator or factory code.
