# Backend Offline Challenge

Solutions to a 4-part backend take-home challenge, organized as one repository with one folder
per part. All code is C# on .NET 8.

| Part | What it is | Run it |
|------|-----------|--------|
| [`part1-flight-planner/`](part1-flight-planner) | Full-stack ASP.NET Core MVC app: enter a flight, get computed great-circle distance/flight time/fuel, list, edit, and a report page. | `dotnet run --project part1-flight-planner/src/FlightPlanner.Web` |
| [`part2-code-review/`](part2-code-review) | Written review of a message-dispatch code sample, with a polymorphism-based rewrite. | n/a — read the README |
| [`part3-file-reader-library/`](part3-file-reader-library) | A file-reading library built incrementally across 9 user stories (text/XML/JSON, encryption, role-based security), one git tag (`v1`–`v9`) per story, plus a bonus interactive CLI. | `dotnet run --project part3-file-reader-library/src/FileReaderLibrary.Cli` |
| [`part4-azure-apim/`](part4-azure-apim) | Written answers on publishing/securing a .NET API through Azure API Management. | n/a — read the README |

Each folder has its own README with design notes, assumptions, and how to build/test/run that
part specifically.

## Building and testing everything

There are two independent solutions (parts 1 and 3 each have their own `.sln`; parts 2 and 4 are
documentation only):

```bash
dotnet build part1-flight-planner/FlightPlanner.sln
dotnet test  part1-flight-planner/FlightPlanner.sln

dotnet build part3-file-reader-library/FileReaderLibrary.sln
dotnet test  part3-file-reader-library/FileReaderLibrary.sln
```

Both are also built/tested in CI on every push/PR ([`.github/workflows/ci.yml`](.github/workflows/ci.yml)).

## Part 3's git history

The challenge specifically asks for part 3 to be recorded in git with a tag per version. Rather
than a separate repository, this monorepo carries that history directly: every commit tagged
`v1`–`v9` touches only `part3-file-reader-library/`, so the tagged history is a clean,
self-contained record of that library's evolution even though other, unrelated commits (parts 1,
2, 4) sit between them chronologically.

```bash
git log --oneline --decorate   # v1..v9 visible on their own commits
git tag -l                     # v1 v2 v3 v4 v5 v6 v7 v8 v9
```

See [`part3-file-reader-library/CHANGELOG.md`](part3-file-reader-library/CHANGELOG.md) for what
user story each version satisfies and how it was implemented without editing prior versions'
code (composition over modification).

## Assumptions made (requirements the PDF left open)

- **Part 1 airports**: picked from a curated, offline seed list of ~30 real major airports
  (ICAO/IATA code, GPS coordinates) rather than free-text entry or a live lookup API, so the app
  stays fully self-contained.
- **Part 1 aircraft**: the PDF asks for fuel based on "aircraft fuel consumption per distance/
  flight time + takeoff effort" but doesn't name an aircraft, so three illustrative profiles are
  seeded (Cessna 172, Airbus A320, Boeing 777) with a cruise speed, fuel burn rate, and fixed
  takeoff allowance each.
- **Part 1 persistence**: EF Core (code-first, migrations checked in) against a local SQLite
  file — no external database server required to run it.
- **Part 3 encryption algorithm**: a reversible text-reverse, exactly as the PDF suggests
  ("can be for example a simple reverse of the text"), behind a swappable `IEncryptionStrategy`.
- **Part 3 role-based security**: a simplistic in-memory allow-list (`admin` reads everything,
  other roles are limited by file extension) behind a swappable `IRoleAuthorizationService`, per
  the PDF's "simplistic implementation, however make sure the switch to a real ... system should
  be possible without actually changing the code."

## Design credit

Part 1's UI palette and type scale are modeled on [cocus.com](https://www.cocus.com/en/) — see
[`part1-flight-planner/README.md`](part1-flight-planner/README.md#design) for details.
