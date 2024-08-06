using System;
using System.Globalization;

namespace AsterixCat048
{
    class Program
    {
        static void Main(string[] args)
        {
            string hexStream = "30002cfd178144000083766640035bc28800000320000008a4c2880000000000110b5100008376033200001b";
            byte[] data = HexStringToByteArray(hexStream);

            AsterixCat048Message message = AsterixDecoder.Decode(data);

            message.PrintToConsole();

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
