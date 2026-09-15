namespace FlightPlanner.Application.Calculators;

public interface IDistanceCalculator
{
    double CalculateDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2);
}
