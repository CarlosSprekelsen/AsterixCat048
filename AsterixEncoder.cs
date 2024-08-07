using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AsterixCat048
{
    public static class AsterixEncoder
    {
        public static byte[] Encode(AsterixCat048Message message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    // Write CAT
                    writer.Write((byte)message.Category);
                    // Write LEN placeholder
                    writer.Write((ushort)0); // Placeholder for length, to be updated later

                    // Initialize FSPEC
                    List<byte> fspec = new List<byte>();
                    long fspecPosition = ms.Position;

                    // Write FSPEC placeholder (reserve 1 byte for FSPEC initially)
                    writer.Write((byte)0);

                    // Encode compulsory fields
                    WriteI048010(writer, message.DataSourceIdentifier); // FRN 1
                    AddToFspec(fspec, 1);

                    WriteI048140(writer, message.TimeOfDay); // FRN 2
                    AddToFspec(fspec, 2);

                    // Encode Other bits to comply with  ICD
                    // Check if RHO exceeds the maximum range and set ERR bit if necessary
            if (message.MeasuredPositionPolar.RHO >= 256 )
            {
                if (message.TargetReportAndTargetCapabilities.Extensions == null)
                {
                    message.TargetReportAndTargetCapabilities.Extensions = new List<I048020Extension>();
                }

                // Add an extension and set the ERR bit
                var extension = new I048020Extension
                {
                    ERR = true,
                    FX = false
                };
                message.TargetReportAndTargetCapabilities.Extensions.Add(extension);

                // Set the FX bit in the main data item if it has extensions
                message.TargetReportAndTargetCapabilities.FX = 0x01;
            }

                    // Encode optional fields
                    if (!message.TargetReportAndTargetCapabilities.Equals(default(I048020)))
                    {
                        WriteI048020(writer, message.TargetReportAndTargetCapabilities); // FRN 3
                        AddToFspec(fspec, 3);
                    }
                    if (!message.MeasuredPositionPolar.Equals(default(I048040)))
                    {
                        WriteI048040(writer, message.MeasuredPositionPolar); // FRN 4
                        AddToFspec(fspec, 4);
                    }

                    if (!message.Mode3ACode.Equals(default(I048070)))
                    {
                        WriteI048070(writer, message.Mode3ACode); // FRN 5
                        AddToFspec(fspec, 5);
                    }

                    if (!message.FlightLevel.Equals(default(I048090)))
                    {
                        WriteI048090(writer, message.FlightLevel); // FRN 6
                        AddToFspec(fspec, 6);
                    }

                    if (!message.RadarPlotCharacteristics.Equals(default(I048130)))
                    {
                        WriteI048130(writer, message.RadarPlotCharacteristics); // FRN 7
                        AddToFspec(fspec, 7);
                    }

                    if (!message.AircraftAddress.Equals(default(I048220)))
                    {
                        WriteI048220(writer, message.AircraftAddress); // FRN 8
                        AddToFspec(fspec, 8);
                    }

                    if (!message.AircraftIdentification.Equals(default(I048240)))
                    {
                        WriteI048240(writer, message.AircraftIdentification); // FRN 9
                        AddToFspec(fspec, 9);
                    }

                    if (!message.BDSRegisterData.Equals(default(I048250)))
                    {
                        WriteI048250(writer, message.BDSRegisterData); // FRN 10
                        AddToFspec(fspec, 10);
                    }

                    if (!message.TrackNumber.Equals(default(I048161)))
                    {
                        WriteI048161(writer, message.TrackNumber); // FRN 11
                        AddToFspec(fspec, 11);
                    }

                    if (!message.CalculatedPositionCartesian.Equals(default(I048042)))
                    {
                        WriteI048042(writer, message.CalculatedPositionCartesian); // FRN 12
                        AddToFspec(fspec, 12);
                    }

                    if (!message.CalculatedTrackVelocityPolar.Equals(default(I048200)))
                    {
                        WriteI048200(writer, message.CalculatedTrackVelocityPolar); // FRN 13
                        AddToFspec(fspec, 13);
                    }

                    if (!message.TrackStatus.Equals(default(I048170)))
                    {
                        WriteI048170(writer, message.TrackStatus); // FRN 14
                        AddToFspec(fspec, 14);
                    }

                    if (!message.TrackQuality.Equals(default(I048210)))
                    {
                        WriteI048210(writer, message.TrackQuality); // FRN 15
                        AddToFspec(fspec, 15);
                    }

                    if (
                        !message.WarningErrorConditionsAndTargetClassification.Equals(
                            default(I048030)
                        )
                    )
                    {
                        WriteI048030(writer, message.WarningErrorConditionsAndTargetClassification); // FRN 16
                        AddToFspec(fspec, 16);
                    }

                    if (!message.Mode3ACodeConfidence.Equals(default(I048080)))
                    {
                        WriteI048080(writer, message.Mode3ACodeConfidence); // FRN 17
                        AddToFspec(fspec, 17);
                    }

                    if (!message.ModeCCodeAndConfidence.Equals(default(I048100)))
                    {
                        WriteI048100(writer, message.ModeCCodeAndConfidence); // FRN 18
                        AddToFspec(fspec, 18);
                    }

                    if (!message.HeightMeasuredBy3DRadar.Equals(default(I048110)))
                    {
                        WriteI048110(writer, message.HeightMeasuredBy3DRadar); // FRN 19
                        AddToFspec(fspec, 19);
                    }

                    if (!message.RadialDopplerSpeed.Equals(default(I048120)))
                    {
                        WriteI048120(writer, message.RadialDopplerSpeed); // FRN 20
                        AddToFspec(fspec, 20);
                    }

                    if (
                        !message.CommunicationsACASCapabilityAndFlightStatus.Equals(
                            default(I048230)
                        )
                    )
                    {
                        WriteI048230(writer, message.CommunicationsACASCapabilityAndFlightStatus); // FRN 21
                        AddToFspec(fspec, 21);
                    }

                    if (!message.ACASResolutionAdvisoryReport.Equals(default(I048260)))
                    {
                        WriteI048260(writer, message.ACASResolutionAdvisoryReport); // FRN 22
                        AddToFspec(fspec, 22);
                    }

                    if (!message.Mode1Code.Equals(default(I048055)))
                    {
                        WriteI048055(writer, message.Mode1Code); // FRN 23
                        AddToFspec(fspec, 23);
                    }

                    if (!message.Mode2Code.Equals(default(I048050)))
                    {
                        WriteI048050(writer, message.Mode2Code); // FRN 24
                        AddToFspec(fspec, 24);
                    }

                    if (!message.Mode1CodeConfidence.Equals(default(I048065)))
                    {
                        WriteI048065(writer, message.Mode1CodeConfidence); // FRN 25
                        AddToFspec(fspec, 25);
                    }

                    if (!message.Mode2CodeConfidence.Equals(default(I048060)))
                    {
                        WriteI048060(writer, message.Mode2CodeConfidence); // FRN 26
                        AddToFspec(fspec, 26);
                    }

                    if (message.SpecialPurposeField != null)
                    {
                        WriteSpecialPurposeField(writer, message.SpecialPurposeField); // FRN 27
                        AddToFspec(fspec, 27);
                    }

                    if (message.ReservedExpansionField != null)
                    {
                        writer.Write(message.ReservedExpansionField); // FRN 28
                        AddToFspec(fspec, 28);
                    }

                    // Calculate FSPEC length
                    int fspecBitCount = fspec.Count * 8;
                    int fspecByteCount = (fspecBitCount + 7) / 8;

                    // Insert FSPEC bytes at the reserved position
                    byte[] remainingBytes = ms.ToArray().Skip((int)fspecPosition + 1).ToArray();
                    ms.Seek(fspecPosition, SeekOrigin.Begin);
                    writer.Write(fspec.ToArray(), 0, fspecByteCount);
                    writer.Write(remainingBytes, 0, remainingBytes.Length);

                    // Update length
                    ushort length = (ushort)ms.Length;
                    ms.Seek(1, SeekOrigin.Begin);
                    writer.Write(BitConverter.GetBytes((ushort)((length >> 8) & 0xFF)), 0, 1); // Big-endian high byte
                    writer.Write(BitConverter.GetBytes((ushort)(length & 0xFF)), 0, 1); // Big-endian low byte

                    return ms.ToArray();
                }
            }
        }

        private static void AddToFspec(List<byte> fspec, int frn)
        {
            int byteIndex = (frn - 1) / 7;
            int bitIndex = (frn - 1) % 7;

            // Ensure the FSPEC list has enough bytes
            while (fspec.Count <= byteIndex)
            {
                fspec.Add(0x00);
            }

            // Set the corresponding bit in the FSPEC
            fspec[byteIndex] |= (byte)(0x80 >> bitIndex);

            // If this is not the last byte, set the extension bit of the current byte
            if (byteIndex > 0)
            {
                fspec[byteIndex - 1] |= 0x01;
            }
        }

        private static void WriteI048010(BinaryWriter writer, I048010 data)
        {
            writer.Write(data.SAC);
            writer.Write(data.SIC);
        }

        private static void WriteI048140(BinaryWriter writer, I048140 data)
        {
            writer.Write((byte)(data.TimeOfDay >> 16));
            writer.Write((byte)(data.TimeOfDay >> 8));
            writer.Write((byte)(data.TimeOfDay));
        }

        private static void WriteI048020(BinaryWriter writer, I048020 data)
        {
            byte firstPart = (byte)(
                (data.TYP << 5)
                | (data.SIM << 4)
                | (data.RDP << 3)
                | (data.SPI << 2)
                | (data.RAB << 1)
                | (data.FX)
            );
            writer.Write(firstPart);

            foreach (var ext in data.Extensions)
            {
                byte extByte = (byte)(
                    (ext.TST << 7)
                    | (ext.ERR ? 0x40 : 0x00)
                    | (ext.XPP ? 0x20 : 0x00)
                    | (ext.ME ? 0x10 : 0x00)
                    | (ext.MI ? 0x08 : 0x00)
                    | (ext.FOE_FRI << 1)
                    | (ext.FX ? 0x01 : 0x00)
                );
                writer.Write(extByte);
            }
        }

        private static void WriteI048030(BinaryWriter writer, I048030 data)
        {
            byte firstPart = (byte)((data.Code << 1) | (data.FX ? 0x01 : 0x00));
            writer.Write(firstPart);
            foreach (var ext in data.Extensions)
            {
                byte extByte = (byte)((ext.Code << 1) | (ext.FX ? 0x01 : 0x00));
                writer.Write(extByte);
            }
        }

        private static void WriteI048040(BinaryWriter writer, I048040 data)
        {
            // Convert RHO to big-endian
            writer.Write((byte)(data.RHO >> 8));
            writer.Write((byte)data.RHO);

            // Convert THETA to big-endian
            writer.Write((byte)(data.THETA >> 8));
            writer.Write((byte)data.THETA);
        }

        private static void WriteI048042(BinaryWriter writer, I048042 data)
        {
            // Convert X to big-endian
            writer.Write((byte)(data.X >> 8));
            writer.Write((byte)data.X);

            // Convert Y to big-endian
            writer.Write((byte)(data.Y >> 8));
            writer.Write((byte)data.Y);
        }

        private static void WriteI048050(BinaryWriter writer, I048050 data)
        {
            ushort combined = (ushort)(
                (data.V ? 0x8000 : 0x0000)
                | (data.G ? 0x4000 : 0x0000)
                | (data.L ? 0x2000 : 0x0000)
                | data.Mode2Code
            );
            writer.Write(combined);
        }

        private static void WriteI048055(BinaryWriter writer, I048055 data)
        {
            byte combined = (byte)(
                (data.V ? 0x80 : 0x00)
                | (data.G ? 0x40 : 0x00)
                | (data.L ? 0x20 : 0x00)
                | data.Mode1Code
            );
            writer.Write(combined);
        }

        private static void WriteI048060(BinaryWriter writer, I048060 data)
        {
            ushort combined = (ushort)(
                (data.QA4 ? 0x0800 : 0x0000)
                | (data.QA2 ? 0x0400 : 0x0000)
                | (data.QA1 ? 0x0200 : 0x0000)
                | (data.QB4 ? 0x0100 : 0x0000)
                | (data.QB2 ? 0x0080 : 0x0000)
                | (data.QB1 ? 0x0040 : 0x0000)
                | (data.QC4 ? 0x0020 : 0x0000)
                | (data.QC2 ? 0x0010 : 0x0000)
                | (data.QC1 ? 0x0008 : 0x0000)
                | (data.QD4 ? 0x0004 : 0x0000)
                | (data.QD2 ? 0x0002 : 0x0000)
                | (data.QD1 ? 0x0001 : 0x0000)
            );
            writer.Write(combined);
        }

        private static void WriteI048065(BinaryWriter writer, I048065 data)
        {
            byte combined = (byte)(
                (data.QA4 ? 0x10 : 0x00)
                | (data.QA2 ? 0x08 : 0x00)
                | (data.QA1 ? 0x04 : 0x00)
                | (data.QB2 ? 0x02 : 0x00)
                | (data.QB1 ? 0x01 : 0x00)
            );
            writer.Write(combined);
        }

private static void WriteI048070(BinaryWriter writer, I048070 data)
{
    // Convert Mode3ACode from decimal to octal
    ushort octalCode = (ushort)I048070.ConvertToOctal(data.Mode3ACode);

    byte firstByte = (byte)(
        (data.V << 7) | (data.G << 6) | (data.L << 5) | ((octalCode >> 8) & 0x0F)
    );

    byte secondByte = (byte)(octalCode & 0xFF);

    writer.Write(firstByte);
    writer.Write(secondByte);
}


        private static void WriteI048080(BinaryWriter writer, I048080 data)
        {
            ushort combined = (ushort)(
                (data.QA4 ? 0x0800 : 0x0000)
                | (data.QA2 ? 0x0400 : 0x0000)
                | (data.QA1 ? 0x0200 : 0x0000)
                | (data.QB4 ? 0x0100 : 0x0000)
                | (data.QB2 ? 0x0080 : 0x0000)
                | (data.QB1 ? 0x0040 : 0x0000)
                | (data.QC4 ? 0x0020 : 0x0000)
                | (data.QC2 ? 0x0010 : 0x0000)
                | (data.QC1 ? 0x0008 : 0x0000)
                | (data.QD4 ? 0x0004 : 0x0000)
                | (data.QD2 ? 0x0002 : 0x0000)
                | (data.QD1 ? 0x0001 : 0x0000)
            );
            writer.Write(combined);
        }

        private static void WriteI048090(BinaryWriter writer, I048090 data)
        {
            ushort combined = (ushort)(
                (data.V ? 0x8000 : 0x0000)
                | (data.G ? 0x4000 : 0x0000)
                | (ushort)(data.FlightLevel & 0x3FFF) // Use only 14 bits for flight level
            );
            writer.Write((byte)(combined >> 8)); // Write the high byte (big-endian)
            writer.Write((byte)(combined & 0xFF)); // Write the low byte (big-endian)
        }

        private static void WriteI048100(BinaryWriter writer, I048100 data)
        {
            uint combined = (uint)(
        (data.V ? 0x80000000u : 0x00000000u)
        | (data.G ? 0x40000000u : 0x00000000u)
        | ((uint)data.ModeCCode << 13)
        | (data.QC1 ? 0x00001000u : 0x00000000u)
        | (data.QA1 ? 0x00000800u : 0x00000000u)
        | (data.QC2 ? 0x00000400u : 0x00000000u)
        | (data.QA2 ? 0x00000200u : 0x00000000u)
        | (data.QC4 ? 0x00000100u : 0x00000000u)
        | (data.QA4 ? 0x00000080u : 0x00000000u)
        | (data.QB1 ? 0x00000040u : 0x00000000u)
        | (data.QD1 ? 0x00000020u : 0x00000000u)
        | (data.QB2 ? 0x00000010u : 0x00000000u)
        | (data.QD2 ? 0x00000008u : 0x00000000u)
        | (data.QB4 ? 0x00000004u : 0x00000000u)
        | (data.QD4 ? 0x00000002u : 0x00000000u)
    );
            writer.Write(combined);
        }

        private static void WriteI048110(BinaryWriter writer, I048110 data)
        {
            short heightInUnits = (short)(data.Height);
            ushort encodedHeight = (ushort)(heightInUnits & 0x3FFF); 
            // Write the height in big-endian format
            writer.Write((byte)(encodedHeight >> 8)); // Write the high byte
            writer.Write((byte)(encodedHeight & 0xFF)); // Write the low byte
        }

        private static void WriteI048120(BinaryWriter writer, I048120 data)
        {
            writer.Write(data.PrimarySubfield);

            if (data.CalculatedDopplerSpeed.HasValue)
            {
                var subfield = data.CalculatedDopplerSpeed.Value;
                writer.Write((byte)(subfield.IsDoubtful ? 0x80 : 0x00));
                writer.Write(subfield.Speed);
            }

            if (data.RawDopplerSpeed.HasValue)
            {
                var subfield = data.RawDopplerSpeed.Value;
                writer.Write(subfield.RepetitionFactor);
                writer.Write(subfield.DopplerSpeed);
                writer.Write(subfield.AmbiguityRange);
                writer.Write(subfield.TransmitterFrequency);
            }
        }

        private static void WriteI048130(BinaryWriter writer, I048130 data)
        {
            writer.Write(data.PrimarySubfield);

            if ((data.PrimarySubfield & 0x80) != 0)
            {
                writer.Write(data.SSRPlotRunlength.GetValueOrDefault());
            }

            if ((data.PrimarySubfield & 0x40) != 0)
            {
                writer.Write(data.NumberOfReceivedReplies.GetValueOrDefault());
            }

            if ((data.PrimarySubfield & 0x20) != 0)
            {
                writer.Write(data.AmplitudeOfReceivedReplies.GetValueOrDefault());
            }

            if ((data.PrimarySubfield & 0x10) != 0)
            {
                writer.Write(data.PSRPlotRunlength.GetValueOrDefault());
            }

            if ((data.PrimarySubfield & 0x08) != 0)
            {
                writer.Write(data.PSRAmplitude.GetValueOrDefault());
            }

            if ((data.PrimarySubfield & 0x04) != 0)
            {
                writer.Write(data.DifferenceInRange.GetValueOrDefault());
            }

            if ((data.PrimarySubfield & 0x02) != 0)
            {
                writer.Write(data.DifferenceInAzimuth.GetValueOrDefault());
            }
        }

        private static void WriteI048161(BinaryWriter writer, I048161 data)
        {
            writer.Write(data.TrackNumber);
        }

        private static void WriteI048170(BinaryWriter writer, I048170 data)
        {
            byte firstPart = (byte)(
                (data.CNF ? 0x80 : 0x00)
                | (data.RAD << 5)
                | (data.DOU ? 0x10 : 0x00)
                | (data.MAH ? 0x08 : 0x00)
                | (data.CDM << 1)
                | (data.FX ? 0x01 : 0x00)
            );
            writer.Write(firstPart);

            foreach (var ext in data.Extensions)
            {
                byte extByte = (byte)(
                    (ext.TRE ? 0x80 : 0x00)
                    | (ext.GHO ? 0x40 : 0x00)
                    | (ext.SUP ? 0x20 : 0x00)
                    | (ext.TCC ? 0x10 : 0x00)
                    | (ext.FX ? 0x01 : 0x00)
                );
                writer.Write(extByte);
            }
        }

        private static void WriteI048200(BinaryWriter writer, I048200 data)
        {
            // Convert GroundSpeed to big-endian
            writer.Write((byte)(data.GroundSpeed >> 8));
            writer.Write((byte)data.GroundSpeed);

            // Convert Heading to big-endian
            writer.Write((byte)(data.Heading >> 8));
            writer.Write((byte)data.Heading);
        }

        private static void WriteI048210(BinaryWriter writer, I048210 data)
        {
            writer.Write(data.SigmaX);
            writer.Write(data.SigmaY);
            writer.Write(data.SigmaV);
            writer.Write(data.SigmaH);
        }

        private static void WriteI048220(BinaryWriter writer, I048220 data)
        {
            writer.Write(data.AircraftAddress);
        }

        private static void WriteI048230(BinaryWriter writer, I048230 data)
        {
            byte firstPart = (byte)((data.COM << 5) | (data.STAT << 2) | (data.SI ? 0x02 : 0x00));
            writer.Write(firstPart);

            byte secondPart = (byte)(
                (data.MSSC ? 0x80 : 0x00)
                | (data.ARC ? 0x40 : 0x00)
                | (data.AIC ? 0x20 : 0x00)
                | (data.B1A ? 0x10 : 0x00)
                | data.B1B
            );
            writer.Write(secondPart);
        }

        private static void WriteI048240(BinaryWriter writer, I048240 data)
        {
            writer.Write(data.AircraftID);
        }

        private static void WriteI048250(BinaryWriter writer, I048250 data)
        {
            writer.Write(data.REP);
            foreach (var bds in data.BDSRegisters)
            {
                writer.Write(bds.BDSData);
                writer.Write(bds.BDS1);
                writer.Write(bds.BDS2);
            }
        }

        private static void WriteI048260(BinaryWriter writer, I048260 data)
        {
            writer.Write(data.ACASRA);
        }

        private static void WriteSpecialPurposeField(BinaryWriter writer, byte[] data)
        {
            writer.Write((byte)(data.Length + 1)); // Write the length byte
            writer.Write(data); // Write the data bytes
        }
    }
}
