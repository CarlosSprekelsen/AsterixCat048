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


            // Radar values 
            double radarLatitude = 18.0; // Radar latitude
            double radarLongitude = 8.0; // Radar longitude
            byte sac = 0;
            byte sic = 1;

            // Target values
            uint timeOfDay = 123519; // Time of day in seconds
            double latitude = 48.1173; // Target latitude
            double longitude = 11.5167; // Target longitude
            double altitude = 545.4; // Altitude in meters
            ushort squawk = 1200; // Squawk code
            double courseOverGround = 90.0; // Course over ground in degrees    
            double groundSpeed = 100.0; // Ground speed in m/s
            

            var (x, y) = CoordinateConverter.ConvertGeodeticToLocalCartesian(
                latitude,
                longitude,
                radarLatitude,
                radarLongitude
            );

            var (rho, theta) = CoordinateConverter.ConvertLocalCartesianToPolar(x, y);

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

            // Create ASTERIX Cat048 message
            AsterixCat048Message message = new AsterixCat048Message
            {
                Category = 48,
                DataSourceIdentifier = new I048010 { SAC = sac, SIC = sic },
                TimeOfDay = new I048140 { TimeOfDay = timeOfDay },
                CalculatedPositionCartesian = new I048042
                {
                    X = (short)(CoordinateConverter.ToNauticalMiles(x) * 128),
                    Y = (short)(CoordinateConverter.ToNauticalMiles(y) * 128)
                }, // Scale factor for I048042
                MeasuredPositionPolar = new I048040
                {
                    RHO = (ushort)Math.Min(CoordinateConverter.ToNauticalMiles(rho) * 256, ushort.MaxValue),
                    THETA = (ushort)(theta * 65536 / 360.0)
                }, // Scale factor for I048040
                FlightLevel = new I048090
                {
                    FlightLevel = (short)(flightLevel * 4), // Scale factor for I048090
                    V = false,
                    G = false
                },
                Mode3ACode = new I048070 { 
                    Mode3ACode = squawk,
                    V = 0x00,
                    G = 0x00,
                    L = 0x00
                },
                HeightMeasuredBy3DRadar = new I048110 { 
                    Height = (short)(altitudeInFeet / 25.0)
                },
                CalculatedTrackVelocityPolar = new I048200 {
                    GroundSpeed = (ushort)(CoordinateConverter.ToNauticalMiles(groundSpeed) / Math.Pow(2, -14)),
                    Heading = (ushort)(courseOverGround * 65536 / 360.0)
                }
            }; 
            // Encode message
            byte[] encodedMessage = AsterixEncoder.Encode(message);

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

    
}
