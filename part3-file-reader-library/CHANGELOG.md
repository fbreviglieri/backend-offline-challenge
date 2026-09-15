# Changelog

## v1 — Read a text file

User story: "A user should be able to read a text file"

Added `IFileReader` (the core abstraction: `string Read(string path)`) and `TextFileReader`,
which reads a text file's raw content.
