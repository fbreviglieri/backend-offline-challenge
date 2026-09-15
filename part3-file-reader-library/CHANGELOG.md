# Changelog

## v1 — Read a text file

User story: "A user should be able to read a text file"

Added `IFileReader` (the core abstraction: `string Read(string path)`) and `TextFileReader`,
which reads a text file's raw content.

## v2 — Read an XML file

User story: "A user should be able to read an XML file"

Added `XmlFileReader`, which parses the file with `XDocument` and returns its content.
