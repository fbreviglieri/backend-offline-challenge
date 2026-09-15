namespace FlightPlanner.Application.Calculators;

public class HaversineDistanceCalculator : IDistanceCalculator
{
    private const double EarthRadiusKm = 6371.0;

    public double CalculateDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        var lat1Rad = ToRadians(latitude1);
        var lat2Rad = ToRadians(latitude2);
        var deltaLatRad = ToRadians(latitude2 - latitude1);
        var deltaLonRad = ToRadians(longitude2 - longitude1);

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
