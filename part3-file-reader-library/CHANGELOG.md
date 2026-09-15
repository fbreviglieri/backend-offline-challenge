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

## v4 — Read XML files in a role-based security context

User story: "A user should be able to read XML files in role based security context" (e.g.
admin can read everything, other roles can only read a limited set; the actual role system is
out of scope, but switching to a real one must be possible without changing code)

Added `IRoleAuthorizationService` + `SimpleRoleAuthorizationService` (a simplistic in-memory
allow-list: "admin" reads everything, other roles are limited by file extension), and
`RoleSecuredFileReaderDecorator`, which wraps any `IFileReader` and denies the read
(`UnauthorizedAccessException`) if the role isn't authorized. Wired into `FileReaderFactory`
for XML files. The authorization service is constructor-injected, so swapping in a real
role-based security system never touches the decorator or factory code.

## v5 — Read encrypted XML files

User story: "Enable the encrypted reading feature also for XML files"

`FileReaderFactory` now composes encryption for XML the same way it already did for TEXT — no
new decorator or strategy classes. This did require reordering *when* format parsing happens
relative to decryption: `XmlFileReader` now parses an inner `IFileReader`'s output (defaulting
to `TextFileReader`) instead of loading the file path directly, so `EncryptedFileReaderDecorator`
can decrypt the raw bytes *before* they're interpreted as XML (parsing still-encrypted bytes as
XML would otherwise fail). `EncryptedFileReaderDecorator` itself is unchanged.

## v6 — Read TEXT files in a role-based security context

User story: "Enable the role based security reading feature also for TEXT files"

Removed `FileReaderFactory`'s file-type restriction on `roleSecured` — `RoleSecuredFileReaderDecorator`
already worked with any `IFileReader`, so this is a pure composition/config change, no new
decorator or service classes.

## v7 — Read a JSON file

User story: "A user should be able to read JSON files"

Added `JsonFileReader`, mirroring `XmlFileReader`'s shape (parses an inner reader's output,
defaulting to `TextFileReader`). Wired into `FileReaderFactory`; encrypted/role-secured JSON are
explicitly not yet supported (guarded with `NotSupportedException`) until v8/v9.

## v8 — Read encrypted JSON files

User story: "A user should be able to read encrypted JSON files"

Removed `FileReaderFactory`'s encrypted-JSON restriction. `EncryptedFileReaderDecorator` and
`JsonFileReader` (which already parses its *inner reader's* output, not the raw file directly)
needed no changes — pure composition.

## v9 — Read JSON files in a role-based security context

User story: "A user should be able to read JSON files in role based security context"

Removed `FileReaderFactory`'s role-secured-JSON restriction. `RoleSecuredFileReaderDecorator`
needed no changes — pure composition, completing coverage of all three formats (TEXT, XML,
JSON) across both cross-cutting concerns (encryption, role-based security).
