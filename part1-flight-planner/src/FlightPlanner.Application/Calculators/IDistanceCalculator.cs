namespace FlightPlanner.Application.Calculators;

public interface IDistanceCalculator
{
    /// <summary>Great-circle distance, in kilometers, between two GPS coordinates.</summary>
    double CalculateDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2);
}
