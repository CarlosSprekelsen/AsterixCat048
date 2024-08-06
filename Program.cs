using System;
using System.Globalization;

namespace AsterixCat048
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            //Valid Asterix CAT048 strem from Wireshark used for testing the decdoer
            string hexStream = "30002cfd178144000083766640035bc28800000320000008a4c2880000000000110b5100008376033200001b";
            byte[] data = HexStringToByteArray(hexStream);
            AsterixCat048Message message = AsterixDecoder.Decode(data);
            message.PrintToConsole();
            */

            // Sample NMEA sentences
            string[] nmeaSentences = new string[]
            {
                "$GPGGA,123519,4807.038,N,01131.000,E,1,08,0.9,545.4,M,46.9,M,,*47",
                "$GPRMC,123519,A,4807.038,N,01131.000,E,022.4,084.4,230394,003.1,W*6A",
                "$GPGSV,3,1,12,04,77,068,46,05,05,033,,07,11,097,42,08,17,196,45*75",
                "$GPGSA,A,3,04,05,,07,08,,09,10,,12,,14,3.8,1.2,2.9*3D"
            };

            // Required program paramenters

            double radarLatitude = 48.0; // Radar latitude
            double radarLongitude = 11.0; // Radar longitude
            byte sac = 0;
            byte sic = 1;
            // Extract data from NMEA sentences
            ExtractDataFromNMEA(
                nmeaSentences,
                out uint timeOfDay,
                out double latitude,
                out double longitude,
                out double altitude
            );

            var (x, y) = CoordinateConverter.ConvertToLocal2D(
                latitude,
                longitude,
                radarLatitude,
                radarLongitude
            );
            Console.Write($"X: {x} ");
            Console.Write($"Y: {y} ");

            var (rho, theta) = CoordinateConverter.ConvertToPolar(x, y);
            Console.Write($"RHO: {rho} ");
            Console.Write($"THETA: {theta} ");

            // Convert altitude to flight level
            // Disclaimer: The flight level should be based on barometric altitude rather than GPS altitude.
            // Flight levels are standardized vertical altitudes used in aviation to ensure safe separation
            // between aircraft. They are expressed in hundreds of feet and are derived from the barometric
            // pressure setting of 1013.25 hPa (29.92 inHg). The difference between GPS altitude and barometric
            // altitude can be significant due to variations in atmospheric pressure and weather conditions.
            // Therefore, using GPS altitude for flight level calculation may not be valid for Air Traffic Control (ATC) purposes.
            // For instance, FL350 refers to a pressure altitude of 35,000 feet based on the standard pressure setting.
            // Always use barometric altitude when calculating flight levels for ATC.
            double altitudeInFeet = altitude * 3.28084; // Convert meters to feet
            int flightLevel = (int)(altitudeInFeet / 100.0); // Flight level in units of 100 feet
            Console.Write($"FL: {flightLevel} ");

            // Create ASTERIX Cat048 message
            AsterixCat048Message messageNMEA = new AsterixCat048Message
            {
                Category = 48,
                DataSourceIdentifier = new I048010 { SAC = sac, SIC = sic },
                TimeOfDay = new I048140 { TimeOfDay = timeOfDay },
                CalculatedPositionCartesian = new I048042
                {
                    X = (short)(x * 128),
                    Y = (short)(y * 128)
                }, // Scale factor for I048042
                MeasuredPositionPolar = new I048040
                {
                    RHO = (ushort)(rho * 256),
                    THETA = (ushort)(theta * 65536 / 360.0)
                }, // Scale factor for I048040
                FlightLevel = new I048090
                {
                    FlightLevel = (short)(flightLevel * 4), // Scale factor for I048090
                    V = false,
                    G = false
                }
            };
            // Encode message
            byte[] encodedMessage = AsterixEncoder.Encode(messageNMEA);

            // Print encoded message
            Console.WriteLine("--------------------");
            Console.WriteLine("Encoded ASTERIX Cat048 Message:");
            foreach (byte b in encodedMessage)
            {
                Console.Write($"{b:X2} ");
            }
            Console.WriteLine();
            Console.WriteLine("--------------------");

            AsterixCat048Message messageReverse = AsterixDecoder.Decode(encodedMessage);
            messageReverse.PrintToConsole();
        }

        private static void ExtractDataFromNMEA(
            string[] nmeaSentences,
            out uint timeOfDay,
            out double latitude,
            out double longitude,
            out double altitude
        )
        {
            timeOfDay = 0;
            latitude = 0;
            longitude = 0;
            altitude = 0;

            foreach (string sentence in nmeaSentences)
            {
                if (sentence.StartsWith("$GPGGA"))
                {
                    // Extract data from GPGGA sentence
                    string[] parts = sentence.Split(',');
                    timeOfDay = ConvertTimeOfDay(parts[1]);
                    latitude = ConvertLatitude(parts[2], parts[3]);
                    longitude = ConvertLongitude(parts[4], parts[5]);
                    altitude = double.Parse(parts[9], CultureInfo.InvariantCulture);
                }
            }
        }

        private static uint ConvertTimeOfDay(string timeString)
        {
            int hours = int.Parse(timeString.Substring(0, 2));
            int minutes = int.Parse(timeString.Substring(2, 2));
            int seconds = int.Parse(timeString.Substring(4, 2));
            uint totalSeconds = (uint)(hours * 3600 + minutes * 60 + seconds);
            return totalSeconds << 7; // Scale factor 2^-7
        }

        private static double ConvertLatitude(string latitudeString, string direction)
        {
            double latitude =
                double.Parse(latitudeString.Substring(0, 2), CultureInfo.InvariantCulture)
                + double.Parse(latitudeString.Substring(2), CultureInfo.InvariantCulture) / 60.0;
            if (direction == "S")
                latitude = -latitude;
            return latitude;
        }

        private static double ConvertLongitude(string longitudeString, string direction)
        {
            double longitude =
                double.Parse(longitudeString.Substring(0, 3), CultureInfo.InvariantCulture)
                + double.Parse(longitudeString.Substring(3), CultureInfo.InvariantCulture) / 60.0;
            if (direction == "W")
                longitude = -longitude;
            return longitude;
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber);
            }
            return bytes;
        }
    }

    public class CoordinateConverter
    {
        private const double EarthRadius = 6378137.0; // WGS-84 Earth radius in meters

        public static (double x, double y) ConvertToLocal2D(
            double latitude,
            double longitude,
            double radarLatitude,
            double radarLongitude
        )
        {
            const double metersToNauticalMiles = 0.000539957;
            // Convert degrees to radians
            double latRad = DegreesToRadians(latitude);
            double lonRad = DegreesToRadians(longitude);
            double radarLatRad = DegreesToRadians(radarLatitude);
            double radarLonRad = DegreesToRadians(radarLongitude);

            // Calculate the stereographic projection
            double k =
                2.0
                / (
                    1.0
                    + Math.Sin(radarLatRad) * Math.Sin(latRad)
                    + Math.Cos(radarLatRad) * Math.Cos(latRad) * Math.Cos(lonRad - radarLonRad)
                );

            double x = EarthRadius * k * Math.Cos(latRad) * Math.Sin(lonRad - radarLonRad);
            double y =
                EarthRadius
                * k
                * (
                    Math.Cos(radarLatRad) * Math.Sin(latRad)
                    - Math.Sin(radarLatRad) * Math.Cos(latRad) * Math.Cos(lonRad - radarLonRad)
                );
            // Convert from meters to Nautical Miles
            x *= metersToNauticalMiles;
            y *= metersToNauticalMiles;
            return (x, y);
        }

        public static (double rho, double theta) ConvertToPolar(double x, double y)
        {
            double rho = Math.Sqrt(x * x + y * y) / 1852.0; // Convert meters to nautical miles
            double theta = RadiansToDegrees(Math.Atan2(x, y)); // Calculate azimuth angle in degrees
            if (theta < 0)
                theta += 360.0; // Normalize angle to [0, 360]

            return (rho, theta);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }
}
