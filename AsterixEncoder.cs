using System;
using System.Collections.Generic;
using System.IO;

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

                    // Write FSPEC placeholder
                    byte[] fspec = new byte[4];
                    int fspecIndex = 0;
                    long fspecPosition = ms.Position;
                    writer.Write(fspec);

                    // Encode mandatory fields
                    WriteI048010(writer, message.DataSourceIdentifier);
                    fspec[fspecIndex] |= 0x80;
                    WriteI048140(writer, message.TimeOfDay);
                    fspec[fspecIndex] |= 0x40;

                    // Encode optional fields based on FSPEC
                    if (message.TargetReportAndTargetCapabilities != default(I048020))
                    {
                        fspec[fspecIndex] |= 0x20;
                        WriteI048020(writer, message.TargetReportAndTargetCapabilities);
                    }
                    if (message.WarningErrorConditionsAndTargetClassification != default(I048030))
                    {
                        fspec[fspecIndex] |= 0x10;
                        WriteI048030(writer, message.WarningErrorConditionsAndTargetClassification);
                    }
                    if (message.MeasuredPositionPolar != default(I048040))
                    {
                        fspec[fspecIndex] |= 0x08;
                        WriteI048040(writer, message.MeasuredPositionPolar);
                    }
                    if (message.CalculatedPositionCartesian != default(I048042))
                    {
                        fspec[fspecIndex] |= 0x04;
                        WriteI048042(writer, message.CalculatedPositionCartesian);
                    }
                    if (message.Mode2Code != default(I048050))
                    {
                        fspec[fspecIndex] |= 0x02;
                        WriteI048050(writer, message.Mode2Code);
                    }
                    if (message.Mode1Code != default(I048055))
                    {
                        fspec[fspecIndex] |= 0x01;
                        WriteI048055(writer, message.Mode1Code);
                    }
                    fspecIndex++;
                    if (message.Mode2CodeConfidence != default(I048060))
                    {
                        fspec[fspecIndex] |= 0x80;
                        WriteI048060(writer, message.Mode2CodeConfidence);
                    }
                    if (message.Mode1CodeConfidence != default(I048065))
                    {
                        fspec[fspecIndex] |= 0x40;
                        WriteI048065(writer, message.Mode1CodeConfidence);
                    }
                    if (message.Mode3ACode != default(I048070))
                    {
                        fspec[fspecIndex] |= 0x20;
                        WriteI048070(writer, message.Mode3ACode);
                    }
                    if (message.Mode3ACodeConfidence != default(I048080))
                    {
                        fspec[fspecIndex] |= 0x10;
                        WriteI048080(writer, message.Mode3ACodeConfidence);
                    }
                    if (message.FlightLevel != default(I048090))
                    {
                        fspec[fspecIndex] |= 0x08;
                        WriteI048090(writer, message.FlightLevel);
                    }
                    if (message.ModeCCodeAndConfidence != default(I048100))
                    {
                        fspec[fspecIndex] |= 0x04;
                        WriteI048100(writer, message.ModeCCodeAndConfidence);
                    }
                    if (message.HeightMeasuredBy3DRadar != default(I048110))
                    {
                        fspec[fspecIndex] |= 0x02;
                        WriteI048110(writer, message.HeightMeasuredBy3DRadar);
                    }
                    if (message.RadialDopplerSpeed != default(I048120))
                    {
                        fspec[fspecIndex] |= 0x01;
                        WriteI048120(writer, message.RadialDopplerSpeed);
                    }
                    fspecIndex++;
                    if (message.RadarPlotCharacteristics != default(I048130))
                    {
                        fspec[fspecIndex] |= 0x80;
                        WriteI048130(writer, message.RadarPlotCharacteristics);
                    }
                    if (message.TrackNumber != default(I048161))
                    {
                        fspec[fspecIndex] |= 0x40;
                        WriteI048161(writer, message.TrackNumber);
                    }
                    if (message.TrackStatus != default(I048170))
                    {
                        fspec[fspecIndex] |= 0x20;
                        WriteI048170(writer, message.TrackStatus);
                    }
                    if (message.CalculatedTrackVelocityPolar != default(I048200))
                    {
                        fspec[fspecIndex] |= 0x10;
                        WriteI048200(writer, message.CalculatedTrackVelocityPolar);
                    }
                    if (message.TrackQuality != default(I048210))
                    {
                        fspec[fspecIndex] |= 0x08;
                        WriteI048210(writer, message.TrackQuality);
                    }
                    if (message.AircraftAddress != default(I048220))
                    {
                        fspec[fspecIndex] |= 0x04;
                        WriteI048220(writer, message.AircraftAddress);
                    }
                    if (message.CommunicationsACASCapabilityAndFlightStatus != default(I048230))
                    {
                        fspec[fspecIndex] |= 0x02;
                        WriteI048230(writer, message.CommunicationsACASCapabilityAndFlightStatus);
                    }
                    if (message.AircraftIdentification != default(I048240))
                    {
                        fspec[fspecIndex] |= 0x01;
                        WriteI048240(writer, message.AircraftIdentification);
                    }
                    fspecIndex++;
                    if (message.BDSRegisterData != default(I048250))
                    {
                        fspec[fspecIndex] |= 0x80;
                        WriteI048250(writer, message.BDSRegisterData);
                    }
                    if (message.ACASResolutionAdvisoryReport != default(I048260))
                    {
                        fspec[fspecIndex] |= 0x40;
                        WriteI048260(writer, message.ACASResolutionAdvisoryReport);
                    }

                    // Write actual FSPEC
                    ms.Seek(fspecPosition, SeekOrigin.Begin);
                    writer.Write(fspec, 0, fspecIndex + 1);
                    ms.Seek(0, SeekOrigin.End);

                    // Encode additional fields
                    foreach (var field in message.AdditionalFields)
                    {
                        writer.Write(field.FieldNumber);
                        // Write the field value based on its type
                        if (field.Value is int)
                        {
                            writer.Write((int)field.Value);
                        }
                        else if (field.Value is double)
                        {
                            writer.Write((double)field.Value);
                        }
                        // Add other types as needed
                    }

                    // Update length
                    ushort length = (ushort)ms.Length;
                    ms.Seek(1, SeekOrigin.Begin);
                    writer.Write(length);

                    return ms.ToArray();
                }
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
    byte firstPart = (byte)((data.TYP << 5) |
                             (data.SIM << 4) |
                             (data.RDP << 3) |
                             (data.SPI << 2) |
                             (data.RAB << 1) |
                             (data.FX));
    writer.Write(firstPart);

    foreach (var ext in data.Extensions)
    {
        byte extByte = (byte)((ext.TST << 7) |
                              (ext.ERR ? 0x40 : 0x00) |
                              (ext.XPP ? 0x20 : 0x00) |
                              (ext.ME ? 0x10 : 0x00) |
                              (ext.MI ? 0x08 : 0x00) |
                              (ext.FOE_FRI << 1) |
                              (ext.FX ? 0x01 : 0x00));
        writer.Write(extByte);
    }
}



        private static void WriteI048030(BinaryWriter writer, I048030 data)
        {
            byte firstPart = (byte)((data.Code << 1) |
                                     (data.FX ? 0x01 : 0x00));
            writer.Write(firstPart);
            foreach (var ext in data.Extensions)
            {
                byte extByte = (byte)((ext.Code << 1) |
                                      (ext.FX ? 0x01 : 0x00));
                writer.Write(extByte);
            }
        }

        private static void WriteI048040(BinaryWriter writer, I048040 data)
        {
            writer.Write(data.RHO);
            writer.Write(data.THETA);
        }

        private static void WriteI048042(BinaryWriter writer, I048042 data)
        {
            writer.Write(data.X);
            writer.Write(data.Y);
        }

        private static void WriteI048050(BinaryWriter writer, I048050 data)
        {
            ushort combined = (ushort)((data.V ? 0x8000 : 0x0000) |
                                       (data.G ? 0x4000 : 0x0000) |
                                       (data.L ? 0x2000 : 0x0000) |
                                       data.Mode2Code);
            writer.Write(combined);
        }

        private static void WriteI048055(BinaryWriter writer, I048055 data)
        {
            byte combined = (byte)((data.V ? 0x80 : 0x00) |
                                   (data.G ? 0x40 : 0x00) |
                                   (data.L ? 0x20 : 0x00) |
                                   data.Mode1Code);
            writer.Write(combined);
        }

        private static void WriteI048060(BinaryWriter writer, I048060 data)
        {
            ushort combined = (ushort)((data.QA4 ? 0x0800 : 0x0000) |
                                       (data.QA2 ? 0x0400 : 0x0000) |
                                       (data.QA1 ? 0x0200 : 0x0000) |
                                       (data.QB4 ? 0x0100 : 0x0000) |
                                       (data.QB2 ? 0x0080 : 0x0000) |
                                       (data.QB1 ? 0x0040 : 0x0000) |
                                       (data.QC4 ? 0x0020 : 0x0000) |
                                       (data.QC2 ? 0x0010 : 0x0000) |
                                       (data.QC1 ? 0x0008 : 0x0000) |
                                       (data.QD4 ? 0x0004 : 0x0000) |
                                       (data.QD2 ? 0x0002 : 0x0000) |
                                       (data.QD1 ? 0x0001 : 0x0000));
            writer.Write(combined);
        }

        private static void WriteI048065(BinaryWriter writer, I048065 data)
        {
            byte combined = (byte)((data.QA4 ? 0x10 : 0x00) |
                                   (data.QA2 ? 0x08 : 0x00) |
                                   (data.QA1 ? 0x04 : 0x00) |
                                   (data.QB2 ? 0x02 : 0x00) |
                                   (data.QB1 ? 0x01 : 0x00));
            writer.Write(combined);
        }

        private static void WriteI048070(BinaryWriter writer, I048070 data)
{
    // Convert Mode3ACode from decimal to octal
    ushort octalCode = (ushort)I048070.ConvertToOctal(data.Mode3ACode);

    byte firstByte = (byte)((data.V << 7) |
                            (data.G << 6) |
                            (data.L << 5) |
                            ((octalCode >> 12) & 0x0F));

    byte secondByte = (byte)(octalCode & 0xFF);

    writer.Write(firstByte);
    writer.Write(secondByte);
}


        private static void WriteI048080(BinaryWriter writer, I048080 data)
        {
            ushort combined = (ushort)((data.QA4 ? 0x0800 : 0x0000) |
                                       (data.QA2 ? 0x0400 : 0x0000) |
                                       (data.QA1 ? 0x0200 : 0x0000) |
                                       (data.QB4 ? 0x0100 : 0x0000) |
                                       (data.QB2 ? 0x0080 : 0x0000) |
                                       (data.QB1 ? 0x0040 : 0x0000) |
                                       (data.QC4 ? 0x0020 : 0x0000) |
                                       (data.QC2 ? 0x0010 : 0x0000) |
                                       (data.QC1 ? 0x0008 : 0x0000) |
                                       (data.QD4 ? 0x0004 : 0x0000) |
                                       (data.QD2 ? 0x0002 : 0x0000) |
                                       (data.QD1 ? 0x0001 : 0x0000));
            writer.Write(combined);
        }

        private static void WriteI048090(BinaryWriter writer, I048090 data)
        {
            ushort combined = (ushort)((data.V ? 0x8000 : 0x0000) |
                                       (data.G ? 0x4000 : 0x0000) |
                                       (ushort)(data.FlightLevel & 0x3FFF));
            writer.Write(combined);
        }

        private static void WriteI048100(BinaryWriter writer, I048100 data)
        {
            uint combined = (uint)((data.V ? 0x80000000 : 0x00000000) |
                                   (data.G ? 0x40000000 : 0x00000000) |
                                   (data.ModeCCode << 13) |
                                   (data.QC1 ? 0x00001000 : 0x00000000) |
                                   (data.QA1 ? 0x00000800 : 0x00000000) |
                                   (data.QC2 ? 0x00000400 : 0x00000000) |
                                   (data.QA2 ? 0x00000200 : 0x00000000) |
                                   (data.QC4 ? 0x00000100 : 0x00000000) |
                                   (data.QA4 ? 0x00000080 : 0x00000000) |
                                   (data.QB1 ? 0x00000040 : 0x00000000) |
                                   (data.QD1 ? 0x00000020 : 0x00000000) |
                                   (data.QB2 ? 0x00000010 : 0x00000000) |
                                   (data.QD2 ? 0x00000008 : 0x00000000) |
                                   (data.QB4 ? 0x00000004 : 0x00000000) |
                                   (data.QD4 ? 0x00000002 : 0x00000000));
            writer.Write(combined);
        }

        private static void WriteI048110(BinaryWriter writer, I048110 data)
        {
            writer.Write(data.Height);
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
        (data.CNF ? 0x80 : 0x00) |
        (data.RAD << 5) |
        (data.DOU ? 0x10 : 0x00) |
        (data.MAH ? 0x08 : 0x00) |
        (data.CDM << 1) |
        (data.FX ? 0x01 : 0x00)
    );
    writer.Write(firstPart);

    foreach (var ext in data.Extensions)
    {
        byte extByte = (byte)(
            (ext.TRE ? 0x80 : 0x00) |
            (ext.GHO ? 0x40 : 0x00) |
            (ext.SUP ? 0x20 : 0x00) |
            (ext.TCC ? 0x10 : 0x00) |
            (ext.FX ? 0x01 : 0x00)
        );
        writer.Write(extByte);
    }
}


        private static void WriteI048200(BinaryWriter writer, I048200 data)
        {
            writer.Write(data.GroundSpeed);
            writer.Write(data.Heading);
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
            byte firstPart = (byte)((data.COM << 5) |
                                     (data.STAT << 2) |
                                     (data.SI ? 0x02 : 0x00));
            writer.Write(firstPart);

            byte secondPart = (byte)((data.MSSC ? 0x80 : 0x00) |
                                     (data.ARC ? 0x40 : 0x00) |
                                     (data.AIC ? 0x20 : 0x00) |
                                     (data.B1A ? 0x10 : 0x00) |
                                     data.B1B);
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
    }
}
