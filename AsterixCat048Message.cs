using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AsterixCat048
{
    // Main structure for I048/010 Data Item
    public struct I048010
    {
        public byte SAC; // System Area Code
        public byte SIC; // System Identification Code

        // Method to convert DataSourceIdentifier to byte array
        public byte[] ToByteArray()
        {
            return new byte[] { SAC, SIC };
        }

        // Method to create DataSourceIdentifier from byte array
        public static I048010 FromByteArray(byte[] data)
        {
            if (data.Length != 2)
                throw new ArgumentException("DataSourceIdentifier must be 2 bytes long");

            return new I048010 { SAC = data[0], SIC = data[1] };
        }

        public override bool Equals(object obj)
        {
            if (obj is I048010 other)
            {
                return SAC == other.SAC && SIC == other.SIC;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SAC, SIC);
        }

        public static bool operator ==(I048010 left, I048010 right) => left.Equals(right);

        public static bool operator !=(I048010 left, I048010 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/010 Data Source Identifier
            Console.WriteLine("Data Item I048/010: Data Source Identifier");
            Console.WriteLine($"  SAC: {SAC}");
            Console.WriteLine($"  SIC: {SIC}");
        }
    }

    // Main structure for I048/020 Data Item
    public struct I048020
    {
        public byte TYP; // Bits 8-6
        public byte SIM; // Bit 5
        public byte RDP; // Bit 4
        public byte SPI; // Bit 3
        public byte RAB; // Bit 2
        public byte FX; // Bit 1

        // List of extensions
        public List<I048020Extension> Extensions;

        public override bool Equals(object obj)
        {
            if (obj is I048020 other)
            {
                return TYP == other.TYP
                    && SIM == other.SIM
                    && RDP == other.RDP
                    && SPI == other.SPI
                    && RAB == other.RAB
                    && FX == other.FX
                    && EqualityComparer<List<I048020Extension>>.Default.Equals(
                        Extensions,
                        other.Extensions
                    );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TYP, SIM, RDP, SPI, RAB, FX, Extensions);
        }

        public static bool operator ==(I048020 left, I048020 right) => left.Equals(right);

        public static bool operator !=(I048020 left, I048020 right) => !(left == right);

        public void PrintToConsole()
        {
            Console.WriteLine("Data Item I048/020: Target Report Descriptor");
            Console.WriteLine($"  TYP: {TYP}\t{GetTYPDescription(TYP)}");
            Console.WriteLine(
                $"  SIM: {SIM}\t{(SIM == 0 ? "Actual Target Report" : "Simulated Target Report")}"
            );
            Console.WriteLine(
                $"  RDP: {RDP}\t{(RDP == 0 ? "Report from RDP chain 1" : "Report from RDP chain 2")}"
            );
            Console.WriteLine(
                $"  SPI: {SPI}\t{(SPI == 0 ? "Absence of SPI" : "Special Position Identification")}"
            );
            Console.WriteLine(
                $"  RAB: {RAB}\t{(RAB == 0 ? "Report from Aircraft transponder" : "Report from Field monitor (fixed transponder)")}"
            );
            Console.WriteLine(
                $"  FX: {FX}\t{(FX == 0 ? "End of Data Item" : "Extension follows")}"
            );

            if (FX != 0)
            {
                foreach (var extension in Extensions)
                {
                    Console.WriteLine("  Extension:");
                    Console.WriteLine($"    TST: {extension.TST}");
                    Console.WriteLine($"    ERR: {extension.ERR}");
                    Console.WriteLine($"    XPP: {extension.XPP}");
                    Console.WriteLine($"    ME: {extension.ME}");
                    Console.WriteLine($"    MI: {extension.MI}");
                    Console.WriteLine($"    FOE_FRI: {extension.FOE_FRI}");
                    Console.WriteLine($"    FX: {extension.FX}");
                }
            }
        }

        private static string GetTYPDescription(byte TYP)
        {
            return TYP switch
            {
                0 => "No detection",
                1 => "Single PSR detection",
                2 => "Single SSR detection",
                3 => "SSR + PSR detection",
                4 => "Single Mode S All-Call",
                5 => "Single Mode S Roll-Call",
                6 => "Mode S All-Call + PSR",
                7 => "Mode S Roll-Call + PSR",
                _ => "Unknown"
            };
        }
    }

    // Structure for I048/020 Extensions
    public struct I048020Extension
    {
        public byte TST; // Bit 8
        public bool ERR; // Bit 7
        public bool XPP; // Bit 6
        public bool ME; // Bit 5
        public bool MI; // Bit 4
        public byte FOE_FRI; // Bits 3-2
        public bool FX; // Bit 1

        public override bool Equals(object obj)
        {
            if (obj is I048020Extension other)
            {
                return TST == other.TST
                    && ERR == other.ERR
                    && XPP == other.XPP
                    && ME == other.ME
                    && MI == other.MI
                    && FOE_FRI == other.FOE_FRI
                    && FX == other.FX;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TST, ERR, XPP, ME, MI, FOE_FRI, FX);
        }

        public static bool operator ==(I048020Extension left, I048020Extension right) =>
            left.Equals(right);

        public static bool operator !=(I048020Extension left, I048020Extension right) =>
            !(left == right);
    }

    public struct I048030
    {
        public byte Code; // Bits 8-2
        public bool FX; // Bit 1
        public List<I048030Extension> Extensions; // List of extensions

        public override bool Equals(object obj)
        {
            if (obj is I048030 other)
            {
                return Code == other.Code
                    && FX == other.FX
                    && EqualityComparer<List<I048030Extension>>.Default.Equals(
                        Extensions,
                        other.Extensions
                    );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, FX, Extensions);
        }

        public static bool operator ==(I048030 left, I048030 right) => left.Equals(right);

        public static bool operator !=(I048030 left, I048030 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/030 Warning/Error Conditions/Target Classification
            foreach (var code in Extensions)
            {
                Console.WriteLine(
                    "Data Item I048/030: Warning/Error Conditions/Target Classification"
                );
                Console.WriteLine($"  Code: {Code}");
                Console.WriteLine($"  FX: {FX}");
            }
        }
    }

    public struct I048030Extension
    {
        public byte Code; // Bits 8-2
        public bool FX; // Bit 1

        public override bool Equals(object obj)
        {
            if (obj is I048030Extension other)
            {
                return Code == other.Code && FX == other.FX;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code, FX);
        }

        public static bool operator ==(I048030Extension left, I048030Extension right) =>
            left.Equals(right);

        public static bool operator !=(I048030Extension left, I048030Extension right) =>
            !(left == right);
    }

    public struct I048040
    {
        public ushort RHO;
        public ushort THETA;

        public override bool Equals(object obj)
        {
            if (obj is I048040 other)
            {
                return RHO == other.RHO && THETA == other.THETA;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RHO, THETA);
        }

        public static bool operator ==(I048040 left, I048040 right) => left.Equals(right);

        public static bool operator !=(I048040 left, I048040 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/040 Measured Position in Slant Polar Coordinates
            Console.WriteLine("Data Item I048/040: Measured Position in Slant Polar Coordinates");
            Console.WriteLine($"  RHO [NM]: {RHO * (1.0 / 256.0)}");
            Console.WriteLine($"  THETA [°]: {THETA * (360.0 / 65536.0)}");
        }
    }

    public struct I048042
    {
        public short X;
        public short Y;

        public override bool Equals(object obj)
        {
            if (obj is I048042 other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(I048042 left, I048042 right) => left.Equals(right);

        public static bool operator !=(I048042 left, I048042 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/042 Calculated Position in Cartesian Coordinates
            Console.WriteLine("Data Item I048/042: Calculated Position in Cartesian Coordinates");
            Console.WriteLine($"  X: {X}");
            Console.WriteLine($"  Y: {Y}");
        }
    }

    public struct I048050
    {
        public bool V;
        public bool G;
        public bool L;
        public ushort Mode2Code;

        public override bool Equals(object obj)
        {
            if (obj is I048050 other)
            {
                return V == other.V && G == other.G && L == other.L && Mode2Code == other.Mode2Code;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(V, G, L, Mode2Code);
        }

        public static bool operator ==(I048050 left, I048050 right) => left.Equals(right);

        public static bool operator !=(I048050 left, I048050 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/050 Mode 2 Code
            Console.WriteLine("Data Item I048/050: Mode-2 Code in Octal Representation");
            Console.WriteLine($"  V: {V}");
            Console.WriteLine($"  G: {G}");
            Console.WriteLine($"  L: {L}");
            Console.WriteLine($"  Mode 2 Code (octal): {Mode2Code}");
        }
    }

    public struct I048055
    {
        public bool V;
        public bool G;
        public bool L;
        public byte Mode1Code;

        public override bool Equals(object obj)
        {
            if (obj is I048055 other)
            {
                return V == other.V && G == other.G && L == other.L && Mode1Code == other.Mode1Code;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(V, G, L, Mode1Code);
        }

        public static bool operator ==(I048055 left, I048055 right) => left.Equals(right);

        public static bool operator !=(I048055 left, I048055 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/055 Mode 1 Code
            Console.WriteLine("Data Item I048/055: Mode-1 Code in Octal Representation");
            Console.WriteLine($"  V: {V}");
            Console.WriteLine($"  G: {G}");
            Console.WriteLine($"  L: {L}");
            Console.WriteLine($"  Mode 1 Code (octal): {Mode1Code}");
        }
    }

    public struct I048060
    {
        public bool QA4;
        public bool QA2;
        public bool QA1;
        public bool QB4;
        public bool QB2;
        public bool QB1;
        public bool QC4;
        public bool QC2;
        public bool QC1;
        public bool QD4;
        public bool QD2;
        public bool QD1;

        public override bool Equals(object obj)
        {
            if (obj is I048060 other)
            {
                return QA4 == other.QA4
                    && QA2 == other.QA2
                    && QA1 == other.QA1
                    && QB4 == other.QB4
                    && QB2 == other.QB2
                    && QB1 == other.QB1
                    && QC4 == other.QC4
                    && QC2 == other.QC2
                    && QC1 == other.QC1
                    && QD4 == other.QD4
                    && QD2 == other.QD2
                    && QD1 == other.QD1;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash1 = HashCode.Combine(QA4, QA2, QA1, QB4, QB2, QB1, QC4, QC2);
            int hash2 = HashCode.Combine(QC1, QD4, QD2, QD1);
            return HashCode.Combine(hash1, hash2);
        }

        public static bool operator ==(I048060 left, I048060 right) => left.Equals(right);

        public static bool operator !=(I048060 left, I048060 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/060 Mode 3/A Code
            Console.WriteLine("Data Item I048/060: Mode-3/A Code in Octal Representation");
            Console.WriteLine($"  QA4: {QA4}");
            Console.WriteLine($"  QA2: {QA2}");
            Console.WriteLine($"  QA1: {QA1}");
            Console.WriteLine($"  QB4: {QB4}");
            Console.WriteLine($"  QB2: {QB2}");
            Console.WriteLine($"  QB1: {QB1}");
            Console.WriteLine($"  QC4: {QC4}");
            Console.WriteLine($"  QC2: {QC2}");
            Console.WriteLine($"  QC1: {QC1}");
            Console.WriteLine($"  QD4: {QD4}");
            Console.WriteLine($"  QD2: {QD2}");
            Console.WriteLine($"  QD1: {QD1}");
        }
    }

    public struct I048065
    {
        public bool QA4;
        public bool QA2;
        public bool QA1;
        public bool QB2;
        public bool QB1;

        public override bool Equals(object obj)
        {
            if (obj is I048065 other)
            {
                return QA4 == other.QA4
                    && QA2 == other.QA2
                    && QA1 == other.QA1
                    && QB2 == other.QB2
                    && QB1 == other.QB1;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(QA4, QA2, QA1, QB2, QB1);
        }

        public static bool operator ==(I048065 left, I048065 right) => left.Equals(right);

        public static bool operator !=(I048065 left, I048065 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/065 Mode 1 Code
            Console.WriteLine("Data Item I048/065: Mode-1 Code in Octal Representation");
            Console.WriteLine($"  QA4: {QA4}");
            Console.WriteLine($"  QA2: {QA2}");
            Console.WriteLine($"  QA1: {QA1}");
            Console.WriteLine($"  QB2: {QB2}");
            Console.WriteLine($"  QB1: {QB1}");
        }
    }

    public struct I048070
    {
        public byte V; // 0 or 1
        public byte G; // 0 or 1
        public byte L; // 0 or 1
        public ushort Mode3ACode; // Decimal representation

        public override bool Equals(object obj)
        {
            if (obj is I048070 other)
            {
                return V == other.V
                    && G == other.G
                    && L == other.L
                    && Mode3ACode == other.Mode3ACode;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(V, G, L, Mode3ACode);
        }

        public static bool operator ==(I048070 left, I048070 right) => left.Equals(right);

        public static bool operator !=(I048070 left, I048070 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/070 Mode 3/A Code
            Console.WriteLine("Data Item I048/070: Mode-3/A Code in Octal Representation");
            Console.WriteLine($"  V: {V}");
            Console.WriteLine($"  G: {G}");
            Console.WriteLine($"  L: {L}");
            Console.WriteLine($"  Mode 3/A Code (decimal): {Mode3ACode}");
            Console.WriteLine($"  Mode 3/A Code (octal): {ConvertToOctal(Mode3ACode)}");
        }

        // Convert decimal to octal
        public static int ConvertToOctal(int decimalNumber)
        {
            int octalNumber = 0,
                placeValue = 1;
            while (decimalNumber != 0)
            {
                int remainder = decimalNumber % 8;
                decimalNumber /= 8;
                octalNumber += remainder * placeValue;
                placeValue *= 10;
            }
            return octalNumber;
        }

        // Convert octal to decimal
        public static int ConvertToDecimal(int octalNumber)
        {
            int decimalNumber = 0,
                placeValue = 1;
            while (octalNumber != 0)
            {
                int remainder = octalNumber % 10;
                octalNumber /= 10;
                decimalNumber += remainder * placeValue;
                placeValue *= 8;
            }
            return decimalNumber;
        }
    }

    public struct I048080
    {
        public bool QA4;
        public bool QA2;
        public bool QA1;
        public bool QB4;
        public bool QB2;
        public bool QB1;
        public bool QC4;
        public bool QC2;
        public bool QC1;
        public bool QD4;
        public bool QD2;
        public bool QD1;

        public override bool Equals(object obj)
        {
            if (obj is I048080 other)
            {
                return QA4 == other.QA4
                    && QA2 == other.QA2
                    && QA1 == other.QA1
                    && QB4 == other.QB4
                    && QB2 == other.QB2
                    && QB1 == other.QB1
                    && QC4 == other.QC4
                    && QC2 == other.QC2
                    && QC1 == other.QC1
                    && QD4 == other.QD4
                    && QD2 == other.QD2
                    && QD1 == other.QD1;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash1 = HashCode.Combine(QA4, QA2, QA1, QB4, QB2, QB1, QC4, QC2);
            int hash2 = HashCode.Combine(QC1, QD4, QD2, QD1);
            return HashCode.Combine(hash1, hash2);
        }

        public static bool operator ==(I048080 left, I048080 right) => left.Equals(right);

        public static bool operator !=(I048080 left, I048080 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/080 Aircraft Address
            Console.WriteLine("Data Item I048/080: Aircraft Address");
            Console.WriteLine($"  QA4: {QA4}");
            Console.WriteLine($"  QA2: {QA2}");
            Console.WriteLine($"  QA1: {QA1}");
            Console.WriteLine($"  QB4: {QB4}");
            Console.WriteLine($"  QB2: {QB2}");
            Console.WriteLine($"  QB1: {QB1}");
            Console.WriteLine($"  QC4: {QC4}");
            Console.WriteLine($"  QC2: {QC2}");
            Console.WriteLine($"  QC1: {QC1}");
            Console.WriteLine($"  QD4: {QD4}");
            Console.WriteLine($"  QD2: {QD2}");
            Console.WriteLine($"  QD1: {QD1}");
        }
    }

    public struct I048090
    {
        public bool V;
        public bool G;
        public short FlightLevel;

        public override bool Equals(object obj)
        {
            if (obj is I048090 other)
            {
                return V == other.V && G == other.G && FlightLevel == other.FlightLevel;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(V, G, FlightLevel);
        }

        public static bool operator ==(I048090 left, I048090 right) => left.Equals(right);

        public static bool operator !=(I048090 left, I048090 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/090 Flight Level
            Console.WriteLine("Data Item I048/090: Flight Level");
            Console.WriteLine($"  V: {V}");
            Console.WriteLine($"  G: {G}");
            Console.WriteLine($"  Flight Level:  {FlightLevel / 4.0:F2}");
        }
    }

    public struct I048100
    {
        public bool V;
        public bool G;
        public ushort ModeCCode;
        public bool QC1;
        public bool QA1;
        public bool QC2;
        public bool QA2;
        public bool QC4;
        public bool QA4;
        public bool QB1;
        public bool QD1;
        public bool QB2;
        public bool QD2;
        public bool QB4;
        public bool QD4;

        public override bool Equals(object obj)
        {
            if (obj is I048100 other)
            {
                return V == other.V
                    && G == other.G
                    && ModeCCode == other.ModeCCode
                    && QC1 == other.QC1
                    && QA1 == other.QA1
                    && QC2 == other.QC2
                    && QA2 == other.QA2
                    && QC4 == other.QC4
                    && QA4 == other.QA4
                    && QB1 == other.QB1
                    && QD1 == other.QD1
                    && QB2 == other.QB2
                    && QD2 == other.QD2
                    && QB4 == other.QB4
                    && QD4 == other.QD4;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash1 = HashCode.Combine(V, G, ModeCCode, QC1, QA1, QC2, QA2, QC4);
            int hash2 = HashCode.Combine(QA4, QB1, QD1, QB2, QD2, QB4, QD4);
            return HashCode.Combine(hash1, hash2);
        }

        public static bool operator ==(I048100 left, I048100 right) => left.Equals(right);

        public static bool operator !=(I048100 left, I048100 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/100 Mode C Code and Code Confidence Indicator
            Console.WriteLine("Data Item I048/100: Mode C Code and Code Confidence Indicator");
            Console.WriteLine($"  V: {V}");
            Console.WriteLine($"  G: {G}");
            Console.WriteLine($"  Mode C Code: {ModeCCode}");
            Console.WriteLine($"  QC1: {QC1}");
            Console.WriteLine($"  QA1: {QA1}");
            Console.WriteLine($"  QC2: {QC2}");
            Console.WriteLine($"  QA2: {QA2}");
            Console.WriteLine($"  QC4: {QC4}");
            Console.WriteLine($"  QA4: {QA4}");
            Console.WriteLine($"  QB1: {QB1}");
            Console.WriteLine($"  QD1: {QD1}");
            Console.WriteLine($"  QB2: {QB2}");
            Console.WriteLine($"  QD2: {QD2}");
            Console.WriteLine($"  QB4: {QB4}");
            Console.WriteLine($"  QD4: {QD4}");
        }
    }

    public struct I048110
    {
        public short Height;

        public override bool Equals(object obj)
        {
            if (obj is I048110 other)
            {
                return Height == other.Height;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Height);
        }

        public static bool operator ==(I048110 left, I048110 right) => left.Equals(right);

        public static bool operator !=(I048110 left, I048110 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/110 Mode C Code and Code Confidence Indicator
            Console.WriteLine("Data Item I048/110: Mode C Code and Code Confidence Indicator");
            Console.WriteLine($"  Height: {Height}");
        }
    }

    public struct I048120
    {
        public byte PrimarySubfield;
        public CalculatedDopplerSpeedSubfield? CalculatedDopplerSpeed;
        public RawDopplerSpeedSubfield? RawDopplerSpeed;

        public override bool Equals(object obj)
        {
            if (obj is I048120 other)
            {
                return PrimarySubfield == other.PrimarySubfield
                    && EqualityComparer<CalculatedDopplerSpeedSubfield?>.Default.Equals(
                        CalculatedDopplerSpeed,
                        other.CalculatedDopplerSpeed
                    )
                    && EqualityComparer<RawDopplerSpeedSubfield?>.Default.Equals(
                        RawDopplerSpeed,
                        other.RawDopplerSpeed
                    );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PrimarySubfield, CalculatedDopplerSpeed, RawDopplerSpeed);
        }

        public static bool operator ==(I048120 left, I048120 right) => left.Equals(right);

        public static bool operator !=(I048120 left, I048120 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/120 Radial Doppler Speed
            Console.WriteLine("Data Item I048/120: Radial Doppler Speed");
            Console.WriteLine($"  Primary Subfield: {PrimarySubfield}");
            if (CalculatedDopplerSpeed.HasValue)
            {
                var doppler = CalculatedDopplerSpeed.Value;
                Console.WriteLine(
                    $"  Calculated Doppler Speed: {(doppler.IsDoubtful ? "Doubtful" : "Valid")}, Speed: {doppler.Speed}"
                );
            }
            if (RawDopplerSpeed.HasValue)
            {
                var rawDoppler = RawDopplerSpeed.Value;
                Console.WriteLine($"  Repetition Factor: {rawDoppler.RepetitionFactor}");
                Console.WriteLine($"  Doppler Speed: {rawDoppler.DopplerSpeed}");
                Console.WriteLine($"  Ambiguity Range: {rawDoppler.AmbiguityRange}");
                Console.WriteLine($"  Transmitter Frequency: {rawDoppler.TransmitterFrequency}");
            }
        }
    }

    public struct CalculatedDopplerSpeedSubfield
    {
        public bool IsDoubtful;
        public short Speed;

        public override bool Equals(object obj)
        {
            if (obj is CalculatedDopplerSpeedSubfield other)
            {
                return IsDoubtful == other.IsDoubtful && Speed == other.Speed;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsDoubtful, Speed);
        }

        public static bool operator ==(
            CalculatedDopplerSpeedSubfield left,
            CalculatedDopplerSpeedSubfield right
        ) => left.Equals(right);

        public static bool operator !=(
            CalculatedDopplerSpeedSubfield left,
            CalculatedDopplerSpeedSubfield right
        ) => !(left == right);
    }

    public struct RawDopplerSpeedSubfield
    {
        public byte RepetitionFactor;
        public ushort DopplerSpeed;
        public ushort AmbiguityRange;
        public ushort TransmitterFrequency;

        public override bool Equals(object obj)
        {
            if (obj is RawDopplerSpeedSubfield other)
            {
                return RepetitionFactor == other.RepetitionFactor
                    && DopplerSpeed == other.DopplerSpeed
                    && AmbiguityRange == other.AmbiguityRange
                    && TransmitterFrequency == other.TransmitterFrequency;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                RepetitionFactor,
                DopplerSpeed,
                AmbiguityRange,
                TransmitterFrequency
            );
        }

        public static bool operator ==(
            RawDopplerSpeedSubfield left,
            RawDopplerSpeedSubfield right
        ) => left.Equals(right);

        public static bool operator !=(
            RawDopplerSpeedSubfield left,
            RawDopplerSpeedSubfield right
        ) => !(left == right);
    }

    public struct I048130
    {
        public byte PrimarySubfield;
        public byte? SSRPlotRunlength;
        public byte? NumberOfReceivedReplies;
        public sbyte? AmplitudeOfReceivedReplies;
        public byte? PSRPlotRunlength;
        public sbyte? PSRAmplitude;
        public short? DifferenceInRange;
        public short? DifferenceInAzimuth;

        public override bool Equals(object obj)
        {
            if (obj is I048130 other)
            {
                return PrimarySubfield == other.PrimarySubfield
                    && SSRPlotRunlength == other.SSRPlotRunlength
                    && NumberOfReceivedReplies == other.NumberOfReceivedReplies
                    && AmplitudeOfReceivedReplies == other.AmplitudeOfReceivedReplies
                    && PSRPlotRunlength == other.PSRPlotRunlength
                    && PSRAmplitude == other.PSRAmplitude
                    && DifferenceInRange == other.DifferenceInRange
                    && DifferenceInAzimuth == other.DifferenceInAzimuth;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                PrimarySubfield,
                SSRPlotRunlength,
                NumberOfReceivedReplies,
                AmplitudeOfReceivedReplies,
                PSRPlotRunlength,
                PSRAmplitude,
                DifferenceInRange,
                DifferenceInAzimuth
            );
        }

        public static bool operator ==(I048130 left, I048130 right) => left.Equals(right);

        public static bool operator !=(I048130 left, I048130 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/130 Radar Plot Characteristics
            Console.WriteLine("Data Item I048/130: Radar Plot Characteristics");
            Console.WriteLine($"  Primary Subfield: {PrimarySubfield}");
            Console.WriteLine($"  SSR Plot Runlength: {SSRPlotRunlength}");
            Console.WriteLine($"  Number of Received Replies: {NumberOfReceivedReplies}");
            Console.WriteLine($"  Amplitude of Received Replies: {AmplitudeOfReceivedReplies}");
            Console.WriteLine($"  PSR Plot Runlength: {PSRPlotRunlength}");
            Console.WriteLine($"  PSR Amplitude: {PSRAmplitude}");
            Console.WriteLine($"  Difference in Range: {DifferenceInRange}");
            Console.WriteLine($"  Difference in Azimuth: {DifferenceInAzimuth}");
        }
    }

    public struct I048140
    {
        public uint TimeOfDay;

        public override bool Equals(object obj)
        {
            if (obj is I048140 other)
            {
                return TimeOfDay == other.TimeOfDay;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TimeOfDay);
        }

        public static bool operator ==(I048140 left, I048140 right) => left.Equals(right);

        public static bool operator !=(I048140 left, I048140 right) => !(left == right);

        public TimeSpan ToTimeSpan()
        {
            // Convert TimeOfDay to TimeSpan (1/128 seconds)
            double totalSeconds = TimeOfDay / 128.0;
            return TimeSpan.FromSeconds(totalSeconds);
        }

        public static uint FromTimeSpan(TimeSpan timeSpan)
        {
            // Convert TimeSpan to TimeOfDay (1/128 seconds)
            double totalSeconds = timeSpan.TotalSeconds;
            return (uint)(totalSeconds * 128);
        }

        public void PrintToConsole()
        {
            // Print I048/140 Time of Day
            Console.WriteLine("Data Item I048/140: Time-of-Day");
            TimeSpan timeSpan = ToTimeSpan();
            Console.WriteLine($"  Time of Day: {timeSpan:hh\\:mm\\:ss}");
        }
    }

    public struct I048161
    {
        public ushort TrackNumber;

        public override bool Equals(object obj)
        {
            if (obj is I048161 other)
            {
                return TrackNumber == other.TrackNumber;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TrackNumber);
        }

        public static bool operator ==(I048161 left, I048161 right) => left.Equals(right);

        public static bool operator !=(I048161 left, I048161 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/161 Track Number
            Console.WriteLine("Data Item I048/161: Track Number");
            Console.WriteLine($"  Track Number: {TrackNumber}");
        }
    }

    public struct I048170
    {
        public bool CNF; // Bit 8
        public byte RAD; // Bits 7-6
        public bool DOU; // Bit 5
        public bool MAH; // Bit 4
        public byte CDM; // Bits 3-2
        public bool FX; // Bit 1

        // List of extensions
        public List<I048170Extension> Extensions;

        public override bool Equals(object obj)
        {
            if (obj is I048170 other)
            {
                return CNF == other.CNF
                    && RAD == other.RAD
                    && DOU == other.DOU
                    && MAH == other.MAH
                    && CDM == other.CDM
                    && FX == other.FX
                    && EqualityComparer<List<I048170Extension>>.Default.Equals(
                        Extensions,
                        other.Extensions
                    );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CNF, RAD, DOU, MAH, CDM, FX, Extensions);
        }

        public static bool operator ==(I048170 left, I048170 right) => left.Equals(right);

        public static bool operator !=(I048170 left, I048170 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/170 Track Status
            Console.WriteLine("Data Item I048/170: Track Status");
            Console.WriteLine($"  CNF: {CNF}");
            Console.WriteLine($"  RAD: {RAD}");
            Console.WriteLine($"  DOU: {DOU}");
            Console.WriteLine($"  MAH: {MAH}");
            Console.WriteLine($"  CDM: {CDM}");
            Console.WriteLine($"  FX: {FX}");

            if (FX)
            {
                foreach (var ext in Extensions)
                {
                    Console.WriteLine("  Extension:");
                    Console.WriteLine($"    TRE: {ext.TRE}");
                    Console.WriteLine($"    GHO: {ext.GHO}");
                    Console.WriteLine($"    SUP: {ext.SUP}");
                    Console.WriteLine($"    TCC: {ext.TCC}");
                    Console.WriteLine($"    FX: {ext.FX}");
                }
            }
        }
    }

    public struct I048170Extension
    {
        public bool TRE; // Bit 8
        public bool GHO; // Bit 7
        public bool SUP; // Bit 6
        public bool TCC; // Bit 5
        public bool FX; // Bit 1

        public override bool Equals(object obj)
        {
            if (obj is I048170Extension other)
            {
                return TRE == other.TRE
                    && GHO == other.GHO
                    && SUP == other.SUP
                    && TCC == other.TCC
                    && FX == other.FX;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TRE, GHO, SUP, TCC, FX);
        }

        public static bool operator ==(I048170Extension left, I048170Extension right) =>
            left.Equals(right);

        public static bool operator !=(I048170Extension left, I048170Extension right) =>
            !(left == right);
    }

    public struct I048200
    {
        public ushort GroundSpeed;
        public ushort Heading;

        public override bool Equals(object obj)
        {
            if (obj is I048200 other)
            {
                return GroundSpeed == other.GroundSpeed && Heading == other.Heading;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GroundSpeed, Heading);
        }

        public static bool operator ==(I048200 left, I048200 right) => left.Equals(right);

        public static bool operator !=(I048200 left, I048200 right) => !(left == right);

        public void PrintToConsole()
        {
            // Apply scaling factors
            double scaledGroundSpeed = GroundSpeed * Math.Pow(2, -14);
            double scaledHeading = Heading * (360.0 / 65536.0);
            Console.WriteLine(
                "Data Item I048/200: Calculated Track Velocity in Polar Representation"
            );
            Console.WriteLine($"  Ground Speed [Nm/s]: {scaledGroundSpeed}");
            Console.WriteLine($"  Heading [°]: {scaledHeading}");
        }
    }

    public struct I048210
    {
        public byte SigmaX;
        public byte SigmaY;
        public byte SigmaV;
        public byte SigmaH;

        public override bool Equals(object obj)
        {
            if (obj is I048210 other)
            {
                return SigmaX == other.SigmaX
                    && SigmaY == other.SigmaY
                    && SigmaV == other.SigmaV
                    && SigmaH == other.SigmaH;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SigmaX, SigmaY, SigmaV, SigmaH);
        }

        public static bool operator ==(I048210 left, I048210 right) => left.Equals(right);

        public static bool operator !=(I048210 left, I048210 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/210 Track Quality
            Console.WriteLine("Data Item I048/210: Track Quality");
            Console.WriteLine($"  Sigma X: {SigmaX}");
            Console.WriteLine($"  Sigma Y: {SigmaY}");
            Console.WriteLine($"  Sigma V: {SigmaV}");
            Console.WriteLine($"  Sigma H: {SigmaH}");
        }
    }

    public struct I048220
    {
        public byte[] AircraftAddress;

        public override bool Equals(object obj)
        {
            if (obj is I048220 other)
            {
                return EqualityComparer<byte[]>.Default.Equals(
                    AircraftAddress,
                    other.AircraftAddress
                );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AircraftAddress);
        }

        public static bool operator ==(I048220 left, I048220 right) => left.Equals(right);

        public static bool operator !=(I048220 left, I048220 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/220 Aircraft Address
            if (AircraftAddress != null)
            {
                Console.WriteLine("Data Item I048/220: Aircraft Address");
                Console.WriteLine($"  Aircraft Address: {BitConverter.ToString(AircraftAddress)}");
            }
        }
    }

    public struct I048230
    {
        public byte COM;
        public byte STAT;
        public bool SI;
        public bool MSSC;
        public bool ARC;
        public bool AIC;
        public bool B1A;
        public byte B1B;

        public override bool Equals(object obj)
        {
            if (obj is I048230 other)
            {
                return COM == other.COM
                    && STAT == other.STAT
                    && SI == other.SI
                    && MSSC == other.MSSC
                    && ARC == other.ARC
                    && AIC == other.AIC
                    && B1A == other.B1A
                    && B1B == other.B1B;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(COM, STAT, SI, MSSC, ARC, AIC, B1A, B1B);
        }

        public static bool operator ==(I048230 left, I048230 right) => left.Equals(right);

        public static bool operator !=(I048230 left, I048230 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/230 Communications/ACAS Capability and Flight Status
            Console.WriteLine(
                "Data Item I048/230: Communications/ACAS Capability and Flight Status"
            );
            Console.WriteLine($"  COM: {COM}");
            Console.WriteLine($"  STAT: {STAT}");
            Console.WriteLine($"  SI: {SI}");
            Console.WriteLine($"  MSSC: {MSSC}");
            Console.WriteLine($"  ARC: {ARC}");
            Console.WriteLine($"  AIC: {AIC}");
            Console.WriteLine($"  B1A: {B1A}");
            Console.WriteLine($"  B1B: {B1B}");
        }
    }

    public struct I048240
    {
        public byte[] AircraftID;

        public override bool Equals(object obj)
        {
            if (obj is I048240 other)
            {
                return EqualityComparer<byte[]>.Default.Equals(AircraftID, other.AircraftID);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AircraftID);
        }

        public static bool operator ==(I048240 left, I048240 right) => left.Equals(right);

        public static bool operator !=(I048240 left, I048240 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/240 Aircraft Identification
            Console.WriteLine("Data Item I048/240: Aircraft Identification");
            Console.WriteLine($"  Aircraft Identification: {BitConverter.ToString(AircraftID)}");
        }
    }

    public struct I048250
    {
        public byte REP;
        public List<BDSRegister> BDSRegisters;

        public override bool Equals(object obj)
        {
            if (obj is I048250 other)
            {
                return REP == other.REP
                    && EqualityComparer<List<BDSRegister>>.Default.Equals(
                        BDSRegisters,
                        other.BDSRegisters
                    );
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(REP, BDSRegisters);
        }

        public static bool operator ==(I048250 left, I048250 right) => left.Equals(right);

        public static bool operator !=(I048250 left, I048250 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/250 Mode S MB Data
            Console.WriteLine("Data Item I048/250: Mode S MB Data");
            foreach (var data in BDSRegisters)
            {
                Console.WriteLine($"  BDS Register Data: {BitConverter.ToString(data.BDSData)}");
                Console.WriteLine($"  BDS1: {data.BDS1}, BDS2: {data.BDS2}");
            }
        }
    }

    public struct BDSRegister
    {
        public byte[] BDSData;
        public byte BDS1;
        public byte BDS2;

        public override bool Equals(object obj)
        {
            if (obj is BDSRegister other)
            {
                return EqualityComparer<byte[]>.Default.Equals(BDSData, other.BDSData)
                    && BDS1 == other.BDS1
                    && BDS2 == other.BDS2;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BDSData, BDS1, BDS2);
        }

        public static bool operator ==(BDSRegister left, BDSRegister right) => left.Equals(right);

        public static bool operator !=(BDSRegister left, BDSRegister right) => !(left == right);
    }

    public struct I048260
    {
        public byte[] ACASRA;

        public override bool Equals(object obj)
        {
            if (obj is I048260 other)
            {
                return EqualityComparer<byte[]>.Default.Equals(ACASRA, other.ACASRA);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ACASRA);
        }

        public static bool operator ==(I048260 left, I048260 right) => left.Equals(right);

        public static bool operator !=(I048260 left, I048260 right) => !(left == right);

        public void PrintToConsole()
        {
            // Print I048/260 ACAS Resolution Advisory Report
            Console.WriteLine("Data Item I048/260: ACAS Resolution Advisory Report");
            Console.WriteLine($"  ACAS RA: {BitConverter.ToString(ACASRA)}");
        }
    }

    // Structure for Asterix Category 048 Messages

    public struct AsterixCat048Message
    {
        public byte Category;
        public uint Length;
        public byte[] FSPEC;
        public List<int> FRNList;
        public I048010 DataSourceIdentifier;
        public I048020 TargetReportAndTargetCapabilities;
        public I048030 WarningErrorConditionsAndTargetClassification;
        public I048040 MeasuredPositionPolar;
        public I048042 CalculatedPositionCartesian;
        public I048050 Mode2Code;
        public I048055 Mode1Code;
        public I048060 Mode2CodeConfidence;
        public I048065 Mode1CodeConfidence;
        public I048070 Mode3ACode;
        public I048080 Mode3ACodeConfidence;
        public I048090 FlightLevel;
        public I048100 ModeCCodeAndConfidence;
        public I048110 HeightMeasuredBy3DRadar;
        public I048120 RadialDopplerSpeed;
        public I048130 RadarPlotCharacteristics;
        public I048140 TimeOfDay;
        public I048161 TrackNumber;
        public I048170 TrackStatus;
        public I048200 CalculatedTrackVelocityPolar;
        public I048210 TrackQuality;
        public I048220 AircraftAddress;
        public I048230 CommunicationsACASCapabilityAndFlightStatus;
        public I048240 AircraftIdentification;
        public I048250 BDSRegisterData;
        public I048260 ACASResolutionAdvisoryReport;
        public List<AdditionalField> AdditionalFields;
        public byte[]? SpecialPurposeField; // Placeholder for special purpose fields
        public byte[]? ReservedExpansionField; // Placeholder for reserved expansion fields

        public void PrintToConsole()
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Decoded Asterix Cat048 Message:");
            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Category: {Category}");
            Console.WriteLine($"Length: {Length}");

            if (FRNList.Contains(1))
                DataSourceIdentifier.PrintToConsole();
            if (FRNList.Contains(2))
                TimeOfDay.PrintToConsole();
            if (FRNList.Contains(3))
                TargetReportAndTargetCapabilities.PrintToConsole();
            if (FRNList.Contains(4))
                MeasuredPositionPolar.PrintToConsole();
            if (FRNList.Contains(5))
                Mode3ACode.PrintToConsole();
            if (FRNList.Contains(6))
                FlightLevel.PrintToConsole();
            if (FRNList.Contains(7))
                RadarPlotCharacteristics.PrintToConsole();
            if (FRNList.Contains(8))
                AircraftAddress.PrintToConsole();
            if (FRNList.Contains(9))
                AircraftIdentification.PrintToConsole();
            if (FRNList.Contains(10))
                BDSRegisterData.PrintToConsole();
            if (FRNList.Contains(11))
                TrackNumber.PrintToConsole();
            if (FRNList.Contains(12))
                CalculatedPositionCartesian.PrintToConsole();
            if (FRNList.Contains(13))
                CalculatedTrackVelocityPolar.PrintToConsole();
            if (FRNList.Contains(14))
                TrackStatus.PrintToConsole();
            if (FRNList.Contains(15))
                TrackQuality.PrintToConsole();
            if (FRNList.Contains(16))
                WarningErrorConditionsAndTargetClassification.PrintToConsole();
            if (FRNList.Contains(17))
                Mode3ACodeConfidence.PrintToConsole();
            if (FRNList.Contains(18))
                ModeCCodeAndConfidence.PrintToConsole();
            if (FRNList.Contains(19))
                HeightMeasuredBy3DRadar.PrintToConsole();
            if (FRNList.Contains(20))
                RadialDopplerSpeed.PrintToConsole();
            if (FRNList.Contains(21))
                CommunicationsACASCapabilityAndFlightStatus.PrintToConsole();
            if (FRNList.Contains(22))
                ACASResolutionAdvisoryReport.PrintToConsole();
            if (FRNList.Contains(23))
                Mode1Code.PrintToConsole();
            if (FRNList.Contains(24))
                Mode2Code.PrintToConsole();
            if (FRNList.Contains(25))
                Mode1CodeConfidence.PrintToConsole();
            if (FRNList.Contains(26))
                Mode2CodeConfidence.PrintToConsole();
            if (FRNList.Contains(27))
            {
                // Print Special Purpose Field
                if (SpecialPurposeField != null)
                {
                    Console.WriteLine("Data Item I048/SP: Special Purpose Field");
                    Console.WriteLine(
                        $"  Data (hex): {BitConverter.ToString(SpecialPurposeField).Replace("-", " ")}"
                    );
                }
            }
            if (FRNList.Contains(28))
            {
                // Print Reserved Expansion Field
                if (ReservedExpansionField != null)
                {
                    Console.WriteLine("Data Item I048/RE: Reserved Expansion Field");
                    Console.WriteLine(
                        $"  Data (hex): {BitConverter.ToString(ReservedExpansionField).Replace("-", " ")}"
                    );
                }
            }
            Console.WriteLine("-------------------------------");
        }

        public override bool Equals(object obj)
        {
            if (obj is AsterixCat048Message other)
            {
                return Category == other.Category
                    && DataSourceIdentifier.Equals(other.DataSourceIdentifier)
                    && TargetReportAndTargetCapabilities.Equals(
                        other.TargetReportAndTargetCapabilities
                    )
                    && WarningErrorConditionsAndTargetClassification.Equals(
                        other.WarningErrorConditionsAndTargetClassification
                    )
                    && MeasuredPositionPolar.Equals(other.MeasuredPositionPolar)
                    && CalculatedPositionCartesian.Equals(other.CalculatedPositionCartesian)
                    && Mode2Code.Equals(other.Mode2Code)
                    && Mode1Code.Equals(other.Mode1Code)
                    && Mode2CodeConfidence.Equals(other.Mode2CodeConfidence)
                    && Mode1CodeConfidence.Equals(other.Mode1CodeConfidence)
                    && Mode3ACode.Equals(other.Mode3ACode)
                    && Mode3ACodeConfidence.Equals(other.Mode3ACodeConfidence)
                    && FlightLevel.Equals(other.FlightLevel)
                    && ModeCCodeAndConfidence.Equals(other.ModeCCodeAndConfidence)
                    && HeightMeasuredBy3DRadar.Equals(other.HeightMeasuredBy3DRadar)
                    && RadialDopplerSpeed.Equals(other.RadialDopplerSpeed)
                    && RadarPlotCharacteristics.Equals(other.RadarPlotCharacteristics)
                    && TimeOfDay.Equals(other.TimeOfDay)
                    && TrackNumber.Equals(other.TrackNumber)
                    && TrackStatus.Equals(other.TrackStatus)
                    && CalculatedTrackVelocityPolar.Equals(other.CalculatedTrackVelocityPolar)
                    && TrackQuality.Equals(other.TrackQuality)
                    && AircraftAddress.Equals(other.AircraftAddress)
                    && CommunicationsACASCapabilityAndFlightStatus.Equals(
                        other.CommunicationsACASCapabilityAndFlightStatus
                    )
                    && AircraftIdentification.Equals(other.AircraftIdentification)
                    && BDSRegisterData.Equals(other.BDSRegisterData)
                    && ACASResolutionAdvisoryReport.Equals(other.ACASResolutionAdvisoryReport)
                    && EqualityComparer<List<AdditionalField>>.Default.Equals(
                        AdditionalFields,
                        other.AdditionalFields
                    );
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash1 = HashCode.Combine(
                DataSourceIdentifier,
                TargetReportAndTargetCapabilities,
                WarningErrorConditionsAndTargetClassification,
                MeasuredPositionPolar,
                CalculatedPositionCartesian,
                Mode2Code,
                Mode1Code
            );
            int hash2 = HashCode.Combine(
                Mode2CodeConfidence,
                Mode1CodeConfidence,
                Mode3ACode,
                Mode3ACodeConfidence,
                FlightLevel,
                ModeCCodeAndConfidence,
                HeightMeasuredBy3DRadar,
                RadialDopplerSpeed
            );
            int hash3 = HashCode.Combine(
                RadarPlotCharacteristics,
                TimeOfDay,
                TrackNumber,
                TrackStatus,
                CalculatedTrackVelocityPolar,
                TrackQuality,
                AircraftAddress,
                CommunicationsACASCapabilityAndFlightStatus
            );
            int hash4 = HashCode.Combine(
                AircraftIdentification,
                BDSRegisterData,
                ACASResolutionAdvisoryReport,
                AdditionalFields
            );
            return HashCode.Combine(hash1, hash2, hash3, hash4);
        }

        public static bool operator ==(AsterixCat048Message left, AsterixCat048Message right) =>
            left.Equals(right);

        public static bool operator !=(AsterixCat048Message left, AsterixCat048Message right) =>
            !(left == right);
    }

    // Structure for Additional Fields
    public struct AdditionalField
    {
        public int FieldNumber;
        public object Value;

        public override bool Equals(object obj)
        {
            if (obj is AdditionalField other)
            {
                return FieldNumber == other.FieldNumber
                    && EqualityComparer<object>.Default.Equals(Value, other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FieldNumber, Value);
        }

        public static bool operator ==(AdditionalField left, AdditionalField right) =>
            left.Equals(right);

        public static bool operator !=(AdditionalField left, AdditionalField right) =>
            !(left == right);
    }
}
