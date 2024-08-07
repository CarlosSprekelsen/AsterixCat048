using System;

public class CoordinateConverter
{
    private const double EarthRadius = 6378137.0; // WGS-84 Earth radius in meters
    private const double MetersToNauticalMiles = 0.000539957; // Conversion factor

    // Convert degrees to radians
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    // Convert radians to degrees
    private static double RadiansToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }

    // Convert meters to nautical miles
    public static double ToNauticalMiles(double meters)
    {
        return meters * MetersToNauticalMiles;
    }

    // Convert nautical miles to meters
    public static double ToMeters(double nauticalMiles)
    {
        return nauticalMiles / MetersToNauticalMiles;
    }

    // Convert geodetic coordinates to local Cartesian coordinates
    public static (double X, double Y) ConvertGeodeticToLocalCartesian(
        double latitude,
        double longitude,
        double radarLatitude,
        double radarLongitude
    )
    {
        double latRad = DegreesToRadians(latitude);
        double lonRad = DegreesToRadians(longitude);
        double radarLatRad = DegreesToRadians(radarLatitude);
        double radarLonRad = DegreesToRadians(radarLongitude);

        double k = 2.0 / (1.0 + Math.Sin(radarLatRad) * Math.Sin(latRad) + Math.Cos(radarLatRad) * Math.Cos(latRad) * Math.Cos(lonRad - radarLonRad));

        double x = EarthRadius * k * Math.Cos(latRad) * Math.Sin(lonRad - radarLonRad);
        double y = EarthRadius * k * (Math.Cos(radarLatRad) * Math.Sin(latRad) - Math.Sin(radarLatRad) * Math.Cos(latRad) * Math.Cos(lonRad - radarLonRad));

        return (x, y);
    }

    // Convert local Cartesian coordinates to polar coordinates
    public static (double Range, double Azimuth) ConvertLocalCartesianToPolar(double x, double y)
    {
        double range = Math.Sqrt(x * x + y * y);
        double azimuth = RadiansToDegrees(Math.Atan2(x, y));
        if (azimuth < 0) azimuth += 360.0;

        return (range, azimuth);
    }

    // Convert local polar coordinates to Cartesian coordinates
    public static (double X, double Y, double Z) ConvertLocalPolarToCartesian(
        double range,
        double azimuth,
        double elevation
    )
    {
        double azimuthRad = DegreesToRadians(azimuth);
        double elevationRad = DegreesToRadians(elevation);

        double x = range * Math.Cos(elevationRad) * Math.Sin(azimuthRad);
        double y = range * Math.Cos(elevationRad) * Math.Cos(azimuthRad);
        double z = range * Math.Sin(elevationRad);

        return (x, y, z);
    }

    // Convert local Cartesian coordinates to polar coordinates
    public static (double Range, double Azimuth, double Elevation) ConvertLocalCartesianToPolar(
        double x,
        double y,
        double z
    )
    {
        double range = Math.Sqrt(x * x + y * y + z * z);
        double azimuth = RadiansToDegrees(Math.Atan2(x, y));
        if (azimuth < 0) azimuth += 360.0;
        double elevation = RadiansToDegrees(Math.Asin(z / Math.Sqrt(x * x + y * y + z * z)));

        return (range, azimuth, elevation);
    }

    // Convert local Cartesian coordinates to geodetic coordinates
    public static (double Latitude, double Longitude, double Altitude) ConvertLocalCartesianToGeodetic(
        double x,
        double y,
        double z,
        double radarLatitude,
        double radarLongitude,
        double radarAltitude
    )
    {
        double range = Math.Sqrt(x * x + y * y + z * z);
        double latRad = DegreesToRadians(radarLatitude);
        double lonRad = DegreesToRadians(radarLongitude);

        double sinLat = Math.Sin(latRad);
        double cosLat = Math.Cos(latRad);
        double sinLon = Math.Sin(lonRad);
        double cosLon = Math.Cos(lonRad);

        double altitude = radarAltitude + z;
        double latitude = radarLatitude + RadiansToDegrees(Math.Asin(y / range));
        double longitude = radarLongitude + RadiansToDegrees(Math.Atan2(x, cosLat * range - sinLat * y));

        return (latitude, longitude, altitude);
    }
}
