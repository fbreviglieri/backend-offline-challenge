# Part 1 — Flight Planner

A full-stack ASP.NET Core MVC (.NET 8) app: enter a flight (departure airport, destination
airport, aircraft), it's persisted with calculated great-circle distance, estimated flight time
and estimated fuel burn; flights can be listed, edited (recalculating on save), and viewed on a
summary report page.

## Architecture

Layered, dependency-injected, no business logic in controllers:

- **FlightPlanner.Domain** — `Airport`, `Aircraft`, `Flight` POCOs.
- **FlightPlanner.Application** — `HaversineDistanceCalculator` (great-circle distance),
  `FuelCalculator` (takeoff fuel + cruise burn over flight time), `FlightService` (orchestrates
  calculators + repositories, works only in DTOs), repository interfaces.
- **FlightPlanner.Infrastructure** — EF Core `FlightPlannerDbContext` on SQLite, repository
  implementations, `DbSeeder` (loads `SeedData/airports.json` and `SeedData/aircraft.json` into
  the DB on first run).
- **FlightPlanner.Web** — ASP.NET Core MVC controllers/views. `FlightsController`
  (Index/Create/Edit) and `ReportController` (summary + totals).
- **FlightPlanner.Application.Tests** — xUnit tests for the distance and fuel calculators.

## Assumptions (the PDF left these unspecified)

- **Airports** are picked from a curated, offline seed list of ~30 real major airports (ICAO/
  IATA code, name, city, country, GPS coordinates) rather than free-text entry or a live lookup
  API — this keeps the app fully self-contained and avoids ambiguous/duplicate airport codes.
- **Aircraft**: the PDF asks for fuel calculated from "aircraft fuel consumption per distance/
  flight time + takeoff effort" but doesn't specify which aircraft. Three illustrative profiles
  are seeded (Cessna 172, Airbus A320, Boeing 777), each with a cruise speed, a fuel burn rate
  per hour, and a fixed takeoff/taxi fuel allowance. A flight selects one of these.
- **Distance**: great-circle (haversine) distance between the two airports' GPS coordinates.
- **Fuel formula**: `takeoffFuelKg + (distanceKm / cruiseSpeedKmh) * fuelBurnPerHourKg`.

## Running it

```bash
dotnet run --project src/FlightPlanner.Web
```

On first run, EF Core migrations apply automatically and the airport/aircraft reference data
seeds into a local `flightplanner.db` SQLite file (created next to the running executable,
alongside `FlightPlanner.Web.csproj`). Then browse to the URL printed on startup (e.g.
`http://localhost:5299`) — it opens directly on the Flights list.

## Testing

```bash
dotnet test FlightPlanner.sln
```

Covers the haversine distance calculation (identity, a known city pair, symmetry) and the fuel
calculator (zero-distance edge case, standard case, invalid cruise speed).
