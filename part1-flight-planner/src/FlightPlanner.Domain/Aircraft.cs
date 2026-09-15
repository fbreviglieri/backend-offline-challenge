namespace FlightPlanner.Domain;

public class Aircraft
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double CruiseSpeedKmh { get; set; }
    public double FuelBurnPerHourKg { get; set; }
    public double TakeoffFuelKg { get; set; }
}
