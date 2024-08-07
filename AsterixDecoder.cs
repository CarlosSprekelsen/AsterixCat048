using System;
using System.Collections.Generic;
using System.IO;

namespace AsterixCat048
{
    public static class AsterixDecoder
    {
        public static AsterixCat048Message Decode(byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            byte category = reader.ReadByte();

            // Decode Length (big-endian)
            byte[] lengthBytes = reader.ReadBytes(2);
            ushort length = (ushort)((lengthBytes[0] << 8) | lengthBytes[1]);

            byte[] fspec = DecodeFSPEC(reader);
            List<int> frnList = GenerateFRNList(fspec);

            AsterixCat048Message message = new AsterixCat048Message
            {
                Category = category,
                Length = length,
                FSPEC = fspec,
                FRNList = frnList
            };

            foreach (var frn in frnList)
            {
                switch (frn)
                {
                    case 1:
                        message.DataSourceIdentifier = DecodeI048010(reader);
                        break;
                    case 2:
                        message.TimeOfDay = DecodeI048140(reader);
                        break;
                    case 3:
                        message.TargetReportAndTargetCapabilities = DecodeI048020(reader);
                        break;
                    case 4:
                        message.MeasuredPositionPolar = DecodeI048040(reader);
                        break;
                    case 5:
                        message.Mode3ACode = DecodeI048070(reader);
                        break;
                    case 6:
                        message.FlightLevel = DecodeI048090(reader);
                        break;
                    case 7:
                        message.RadarPlotCharacteristics = DecodeI048130(reader);
                        break;
                    case 8:
                        message.AircraftAddress = DecodeI048220(reader);
                        break;
                    case 9:
                        message.AircraftIdentification = DecodeI048240(reader);
                        break;
                    case 10:
                        message.BDSRegisterData = DecodeI048250(reader);
                        break;
                    case 11:
                        message.TrackNumber = DecodeI048161(reader);
                        break;
                    case 12:
                        message.CalculatedPositionCartesian = DecodeI048042(reader);
                        break;
                    case 13:
                        message.CalculatedTrackVelocityPolar = DecodeI048200(reader);
                        break;
                    case 14:
                        message.TrackStatus = DecodeI048170(reader);
                        break;
                    case 15:
                        message.TrackQuality = DecodeI048210(reader);
                        break;
                    case 16:
                        message.WarningErrorConditionsAndTargetClassification = DecodeI048030(
                            reader
                        );
                        break;
                    case 17:
                        message.Mode3ACodeConfidence = DecodeI048080(reader);
                        break;
                    case 18:
                        message.ModeCCodeAndConfidence = DecodeI048100(reader);
                        break;
                    case 19:
                        message.HeightMeasuredBy3DRadar = DecodeI048110(reader);
                        break;
                    case 20:
                        message.RadialDopplerSpeed = DecodeI048120(reader);
                        break;
                    case 21:
                        message.CommunicationsACASCapabilityAndFlightStatus = DecodeI048230(reader);
                        break;
                    case 22:
                        message.ACASResolutionAdvisoryReport = DecodeI048260(reader);
                        break;
                    case 23:
                        message.Mode1Code = DecodeI048055(reader);
                        break;
                    case 24:
                        message.Mode2Code = DecodeI048050(reader);
                        break;
                    case 25:
                        message.Mode1CodeConfidence = DecodeI048065(reader);
                        break;
                    case 26:
                        message.Mode2CodeConfidence = DecodeI048060(reader);
                        break;
                    case 27:
                        message.SpecialPurposeField = DecodeSpecialPurposeField(reader);
                        break;
                    case 28:
                        message.ReservedExpansionField = DecodeReservedExpansionField(reader);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown FRN: {frn}");
                }
            }

            return message;
        }

        private static byte[] DecodeFSPEC(BinaryReader reader)
        {
            List<byte> fspecList = new List<byte>();
            bool moreFspec = true;

            while (moreFspec)
            {
                byte fspecByte = reader.ReadByte();
                fspecList.Add(fspecByte);
                moreFspec = (fspecByte & 0x01) != 0;
            }

            return fspecList.ToArray();
        }

        private static List<int> GenerateFRNList(byte[] fspec)
        {
            List<int> frnList = new List<int>();
            int frn = 1;

            foreach (byte b in fspec)
            {
                for (int i = 7; i >= 1; i--)
                {
                    if ((b & (1 << i)) != 0)
                    {
                        frnList.Add(frn);
                    }
                    frn++;
                }

                if ((b & 0x01) == 0)
                {
                    break;
                }
            }

            return frnList;
        }

        // Decoding functions for each data item...
        private static I048010 DecodeI048010(BinaryReader reader)
        {
            return new I048010 { SAC = reader.ReadByte(), SIC = reader.ReadByte() };
        }

        private static I048020 DecodeI048020(BinaryReader reader)
        {
            byte firstPart = reader.ReadByte();
            I048020 data = new I048020
            {
                TYP = (byte)((firstPart >> 5) & 0x07),
                SIM = (byte)((firstPart >> 4) & 0x01),
                RDP = (byte)((firstPart >> 3) & 0x01),
                SPI = (byte)((firstPart >> 2) & 0x01),
                RAB = (byte)((firstPart >> 1) & 0x01),
                FX = (byte)(firstPart & 0x01),
                Extensions = new List<I048020Extension>()
            };
            var FollowingExtension = data.FX;
            while (FollowingExtension != 0)
            {
                byte extByte = reader.ReadByte();
                I048020Extension extension = new I048020Extension
                {
                    TST = (byte)((extByte >> 7) & 0x01),
                    ERR = (extByte & 0x40) != 0,
                    XPP = (extByte & 0x20) != 0,
                    ME = (extByte & 0x10) != 0,
                    MI = (extByte & 0x08) != 0,
                    FOE_FRI = (byte)((extByte >> 1) & 0x03),
                    FX = (extByte & 0x01) != 0
                };
                data.Extensions.Add(extension);
                FollowingExtension = (byte)(extByte & 0x01);
            }

            return data;
        }

        private static I048030 DecodeI048030(BinaryReader reader)
        {
            var firstByte = reader.ReadByte();
            var warningErrorConditions = new I048030
            {
                Code = (byte)(firstByte >> 1),
                FX = (firstByte & 0x01) != 0,
                Extensions = new List<I048030Extension>()
            };

            while (warningErrorConditions.FX)
            {
                var extByte = reader.ReadByte();
                var extension = new I048030Extension
                {
                    Code = (byte)(extByte >> 1),
                    FX = (extByte & 0x01) != 0
                };
                warningErrorConditions.Extensions.Add(extension);
            }

            return warningErrorConditions;
        }

        private static I048040 DecodeI048040(BinaryReader reader)
        {
            ushort rho = ReadUInt16BigEndian(reader);
            ushort theta = ReadUInt16BigEndian(reader);

            return new I048040 { RHO = rho, THETA = theta };
        }

        private static ushort ReadUInt16BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(2);
            return (ushort)((bytes[0] << 8) | bytes[1]);
        }

        private static I048042 DecodeI048042(BinaryReader reader)
        {
            short x = ReadInt16BigEndian(reader);
            short y = ReadInt16BigEndian(reader);

            return new I048042 { X = x, Y = y };
        }

        private static short ReadInt16BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        private static I048050 DecodeI048050(BinaryReader reader)
        {
            ushort value = reader.ReadUInt16();
            return new I048050
            {
                V = (value & 0x8000) != 0,
                G = (value & 0x4000) != 0,
                L = (value & 0x2000) != 0,
                Mode2Code = (ushort)(value & 0x0FFF)
            };
        }

        private static I048055 DecodeI048055(BinaryReader reader)
        {
            byte value = reader.ReadByte();
            return new I048055
            {
                V = (value & 0x80) != 0,
                G = (value & 0x40) != 0,
                L = (value & 0x20) != 0,
                Mode1Code = (byte)(value & 0x1F)
            };
        }

        private static I048060 DecodeI048060(BinaryReader reader)
        {
            ushort value = reader.ReadUInt16();
            return new I048060
            {
                QA4 = (value & 0x0800) != 0,
                QA2 = (value & 0x0400) != 0,
                QA1 = (value & 0x0200) != 0,
                QB4 = (value & 0x0100) != 0,
                QB2 = (value & 0x0080) != 0,
                QB1 = (value & 0x0040) != 0,
                QC4 = (value & 0x0020) != 0,
                QC2 = (value & 0x0010) != 0,
                QC1 = (value & 0x0008) != 0,
                QD4 = (value & 0x0004) != 0,
                QD2 = (value & 0x0002) != 0,
                QD1 = (value & 0x0001) != 0
            };
        }

        private static I048065 DecodeI048065(BinaryReader reader)
        {
            byte value = reader.ReadByte();
            return new I048065
            {
                QA4 = (value & 0x10) != 0,
                QA2 = (value & 0x08) != 0,
                QA1 = (value & 0x04) != 0,
                QB2 = (value & 0x02) != 0,
                QB1 = (value & 0x01) != 0
            };
        }

        private static I048070 DecodeI048070(BinaryReader reader)
{
    byte firstByte = reader.ReadByte();
    byte secondByte = reader.ReadByte();

    ushort octalCode = (ushort)(((firstByte & 0x0F) << 8) | secondByte);

    I048070 data = new I048070
    {
        V = (byte)((firstByte >> 7) & 0x01),
        G = (byte)((firstByte >> 6) & 0x01),
        L = (byte)((firstByte >> 5) & 0x01),
        Mode3ACode = (ushort)I048070.ConvertToDecimal(octalCode)
    };

    return data;
}


        private static I048080 DecodeI048080(BinaryReader reader)
        {
            ushort value = reader.ReadUInt16();
            return new I048080
            {
                QA4 = (value & 0x0800) != 0,
                QA2 = (value & 0x0400) != 0,
                QA1 = (value & 0x0200) != 0,
                QB4 = (value & 0x0100) != 0,
                QB2 = (value & 0x0080) != 0,
                QB1 = (value & 0x0040) != 0,
                QC4 = (value & 0x0020) != 0,
                QC2 = (value & 0x0010) != 0,
                QC1 = (value & 0x0008) != 0,
                QD4 = (value & 0x0004) != 0,
                QD2 = (value & 0x0002) != 0,
                QD1 = (value & 0x0001) != 0
            };
        }

        private static I048090 DecodeI048090(BinaryReader reader)
        {
            ushort value = ReadUInt16BigEndian(reader);
            short flightLevel = (short)(value & 0x3FFF);

            // Sign-extend the 14-bit two's complement Flight Level value
            if ((flightLevel & 0x2000) != 0)
            {
                flightLevel |= unchecked((short)0xC000);
            }

            return new I048090
            {
                V = (value & 0x8000) != 0,
                G = (value & 0x4000) != 0,
                FlightLevel = flightLevel
            };
        }

        private static I048100 DecodeI048100(BinaryReader reader)
        {
            uint value = reader.ReadUInt32();
            return new I048100
            {
                V = (value & 0x80000000) != 0,
                G = (value & 0x40000000) != 0,
                ModeCCode = (ushort)((value & 0x0FFF0000) >> 16),
                QC1 = (value & 0x00000800) != 0,
                QA1 = (value & 0x00000400) != 0,
                QC2 = (value & 0x00000200) != 0,
                QA2 = (value & 0x00000100) != 0,
                QC4 = (value & 0x00000080) != 0,
                QA4 = (value & 0x00000040) != 0,
                QB1 = (value & 0x00000020) != 0,
                QD1 = (value & 0x00000010) != 0,
                QB2 = (value & 0x00000008) != 0,
                QD2 = (value & 0x00000004) != 0,
                QB4 = (value & 0x00000002) != 0,
                QD4 = (value & 0x00000001) != 0
            };
        }

        private static I048110 DecodeI048110(BinaryReader reader)
        {
               // Read the height in big-endian format
            ushort encodedHeight = (ushort)((reader.ReadByte() << 8) | reader.ReadByte());
            // Extract the 14-bit height value
            short heightInUnits = (short)(encodedHeight & 0x3FFF);
            return new I048110 { Height = heightInUnits};
        }


        private static I048120 DecodeI048120(BinaryReader reader)
        {
            var primarySubfield = reader.ReadByte();
            var radialDopplerSpeed = new I048120 { PrimarySubfield = primarySubfield };

            if ((primarySubfield & 0x80) != 0)
            {
                var dopplerSpeedByte = reader.ReadByte();
                radialDopplerSpeed.CalculatedDopplerSpeed = new CalculatedDopplerSpeedSubfield
                {
                    IsDoubtful = (dopplerSpeedByte & 0x80) != 0,
                    Speed = reader.ReadInt16()
                };
            }

            if ((primarySubfield & 0x40) != 0)
            {
                radialDopplerSpeed.RawDopplerSpeed = new RawDopplerSpeedSubfield
                {
                    RepetitionFactor = reader.ReadByte(),
                    DopplerSpeed = reader.ReadUInt16(),
                    AmbiguityRange = reader.ReadUInt16(),
                    TransmitterFrequency = reader.ReadUInt16()
                };
            }

            return radialDopplerSpeed;
        }

        private static I048130 DecodeI048130(BinaryReader reader)
        {
            var primarySubfield = reader.ReadByte();
            var radarPlotCharacteristics = new I048130 { PrimarySubfield = primarySubfield };

            if ((primarySubfield & 0x80) != 0)
            {
                radarPlotCharacteristics.SSRPlotRunlength = reader.ReadByte();
            }

            if ((primarySubfield & 0x40) != 0)
            {
                radarPlotCharacteristics.NumberOfReceivedReplies = reader.ReadByte();
            }

            if ((primarySubfield & 0x20) != 0)
            {
                radarPlotCharacteristics.AmplitudeOfReceivedReplies = reader.ReadSByte();
            }

            if ((primarySubfield & 0x10) != 0)
            {
                radarPlotCharacteristics.PSRPlotRunlength = reader.ReadByte();
            }

            if ((primarySubfield & 0x08) != 0)
            {
                radarPlotCharacteristics.PSRAmplitude = reader.ReadSByte();
            }

            if ((primarySubfield & 0x04) != 0)
            {
                radarPlotCharacteristics.DifferenceInRange = reader.ReadInt16();
            }

            if ((primarySubfield & 0x02) != 0)
            {
                radarPlotCharacteristics.DifferenceInAzimuth = reader.ReadInt16();
            }

            return radarPlotCharacteristics;
        }

        private static I048140 DecodeI048140(BinaryReader reader)
        {
            byte[] timeOfDayBytes = reader.ReadBytes(3);
            uint timeOfDay = (uint)(
                (timeOfDayBytes[0] << 16) | (timeOfDayBytes[1] << 8) | timeOfDayBytes[2]
            );

            return new I048140 { TimeOfDay = timeOfDay };
        }

        private static I048161 DecodeI048161(BinaryReader reader)
        {
            return new I048161 { TrackNumber = reader.ReadUInt16() };
        }

        private static I048170 DecodeI048170(BinaryReader reader)
        {
            var firstPart = reader.ReadByte();
            var trackStatus = new I048170
            {
                CNF = (firstPart & 0x80) != 0,
                RAD = (byte)((firstPart & 0x60) >> 5),
                DOU = (firstPart & 0x10) != 0,
                MAH = (firstPart & 0x08) != 0,
                CDM = (byte)((firstPart & 0x06) >> 1),
                FX = (firstPart & 0x01) != 0,
                Extensions = new List<I048170Extension>()
            };

            while (trackStatus.FX)
            {
                var extensionByte = reader.ReadByte();
                var extension = new I048170Extension
                {
                    TRE = (extensionByte & 0x80) != 0,
                    GHO = (extensionByte & 0x40) != 0,
                    SUP = (extensionByte & 0x20) != 0,
                    TCC = (extensionByte & 0x10) != 0,
                    FX = (extensionByte & 0x01) != 0
                };
                trackStatus.Extensions.Add(extension);
            }

            return trackStatus;
        }

        private static I048200 DecodeI048200(BinaryReader reader)
        {
            ushort groundspedd = ReadUInt16BigEndian(reader);
            ushort heading = ReadUInt16BigEndian(reader);
            return new I048200 { GroundSpeed = groundspedd, Heading = heading };
        }

        private static I048210 DecodeI048210(BinaryReader reader)
        {
            return new I048210
            {
                SigmaX = reader.ReadByte(),
                SigmaY = reader.ReadByte(),
                SigmaV = reader.ReadByte(),
                SigmaH = reader.ReadByte()
            };
        }

        private static I048220 DecodeI048220(BinaryReader reader)
        {
            return new I048220 { AircraftAddress = reader.ReadBytes(3) };
        }

        private static I048230 DecodeI048230(BinaryReader reader)
        {
            ushort value = reader.ReadUInt16();
            return new I048230
            {
                COM = (byte)((value & 0xE000) >> 13),
                STAT = (byte)((value & 0x1C00) >> 10),
                SI = (value & 0x0200) != 0,
                MSSC = (value & 0x0100) != 0,
                ARC = (value & 0x0080) != 0,
                AIC = (value & 0x0040) != 0,
                B1A = (value & 0x0020) != 0,
                B1B = (byte)(value & 0x001F)
            };
        }

        private static I048240 DecodeI048240(BinaryReader reader)
        {
            return new I048240 { AircraftID = reader.ReadBytes(6) };
        }

        private static I048250 DecodeI048250(BinaryReader reader)
        {
            var bdsData = new I048250
            {
                REP = reader.ReadByte(),
                BDSRegisters = new List<BDSRegister>()
            };

            for (int i = 0; i < bdsData.REP; i++)
            {
                var register = new BDSRegister
                {
                    BDSData = reader.ReadBytes(7),
                    BDS1 = reader.ReadByte(),
                    BDS2 = reader.ReadByte()
                };
                bdsData.BDSRegisters.Add(register);
            }

            return bdsData;
        }

        private static I048260 DecodeI048260(BinaryReader reader)
        {
            return new I048260 { ACASRA = reader.ReadBytes(7) };
        }

        private static byte[] DecodeSpecialPurposeField(BinaryReader reader)
        {
            byte length = reader.ReadByte();
            byte[] data = reader.ReadBytes(length - 1); // Subtract 1 to account for the length byte itself
            return data;
        }

        private static byte[] DecodeReservedExpansionField(BinaryReader reader)
        {
            // Read the length byte
            byte length = reader.ReadByte();
            // Read the remaining bytes based on the length
            return reader.ReadBytes(length);
        }
    }
}
