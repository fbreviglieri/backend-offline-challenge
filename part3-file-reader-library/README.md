# Part 3 — File Reader Library

A small file-reading library built incrementally, one user story at a time, with the git
history tagged `v1` through `v9` — one tag per feature. See [`CHANGELOG.md`](CHANGELOG.md) for
what each version added, or `git log --oneline --decorate` / `git tag` in this repository.

## Design

- **`IFileReader.Read(path)`** is the one abstraction everything implements: `TextFileReader`,
  `XmlFileReader`, `JsonFileReader`. `XmlFileReader`/`JsonFileReader` parse an *inner*
  `IFileReader`'s output (defaulting to `TextFileReader`) rather than loading the file path
  directly — this is what lets them sit on top of decryption/role-security decorators.
- **`EncryptedFileReaderDecorator`** wraps any `IFileReader` and decrypts what it reads via an
  injected `IEncryptionStrategy` (`ReverseEncryptionStrategy` here). Swapping the encryption
  algorithm means providing a different `IEncryptionStrategy` — never editing the decorator.
- **`RoleSecuredFileReaderDecorator`** wraps any `IFileReader` and denies the read
  (`UnauthorizedAccessException`) unless an injected `IRoleAuthorizationService` says the role
  may read that path. Swapping in a real role system means providing a different
  `IRoleAuthorizationService` — never editing the decorator.
- **`FileReaderFactory`** composes `raw read -> role gate -> decrypt -> format parse` for a
  requested `(FileType, encrypted, roleSecured, role)` combination. Enabling encryption or
  role-security for an additional format (the v5/v6/v8/v9 stories) only ever required removing
  a guard in this factory — no new decorator or strategy classes.

## Building and testing

```bash
dotnet build FileReaderLibrary.sln
dotnet test FileReaderLibrary.sln
```

## Bonus: interactive CLI

```bash
dotnet run --project src/FileReaderLibrary.Cli
```

Prompts for file type (text/xml/json), whether it's encrypted, whether to use role-based
security (and if so, your role), and the file path — reads and prints the content, then asks if
you'd like to read another file (in a different way) without restarting.
