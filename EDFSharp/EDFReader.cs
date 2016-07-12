using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EDFSharp
{
    class EDFReader : BinaryReader
    {
        public EDFReader(FileStream fs) : base(fs) { }

        public EDFHeader ReadHeader()
        {
            EDFHeader h = new EDFHeader();

            this.BaseStream.Seek(0, SeekOrigin.Begin);

            //------ Fixed length header part --------
            h.Version.Value                 = ReadAscii(ItemLengths.Version);
            h.PatientID.Value               = ReadAscii(ItemLengths.PatientID);
            h.RecordID.Value                = ReadAscii(ItemLengths.RecordID);
            h.StartDate.Value               = ReadAscii(ItemLengths.StartDate);
            h.StartTime.Value               = ReadAscii(ItemLengths.StartTime);
            h.NumberOfBytesInHeader.Value   = ReadInt16(ItemLengths.NumberOfBytesInHeader);
            h.Reserved.Value                = ReadAscii(ItemLengths.Reserved);
            h.NumberOfDataRecords.Value     = ReadInt16(ItemLengths.NumberOfDataRecords);
            h.DurationOfDataRecord.Value    = ReadInt16(ItemLengths.DurationOfDataRecord);
            h.NumberOfSignals.Value         = ReadInt16(ItemLengths.NumberOfSignals);

            //------ Variable length header part --------
            int ns = h.NumberOfSignals.Value;
            h.Labels.Value                      = ReadMultipleAscii(16, ns);
            h.TransducerType.Value              = ReadMultipleAscii(80, ns);
            h.PhysicalDimension.Value           = ReadMultipleAscii(8, ns);
            h.PhysicalMinimum.Value             = ReadMultipleDouble(8, ns);
            h.PhysicalMaximum.Value             = ReadMultipleDouble(8, ns);
            h.DigitalMinimum.Value              = ReadMultipleInt(8, ns);
            h.DigitalMaximum.Value              = ReadMultipleInt(8, ns);
            h.Prefiltering.Value                = ReadMultipleAscii(80, ns);
            h.NumberOfSamplesInDataRecord.Value = ReadMultipleInt(8, ns);
            h.SignalsReserved.Value             = ReadMultipleAscii(32, ns);

            return h;
        }

        public EDFSignal[] ReadSignals()
        {
            EDFHeader header = ReadHeader();
            EDFSignal[] signals = new EDFSignal[header.NumberOfSignals.Value];

            for (int i = 0; i < signals.Length; i++) {
                signals[i] = new EDFSignal {
                    Label = header.Labels.Value[i],
                    NumberOfSamples = header.NumberOfSamplesInDataRecord.Value[i]
                };
            }

            //Read the signal sample values
            int readPosition = header.NumberOfBytesInHeader.Value;

            for (int i = 0; i < signals.Length; i++)
            {
                signals[i].Samples = ReadSignalSamples(readPosition, signals[i].NumberOfSamples);
                readPosition += signals[i].Samples.Length * 2; //2 bytes per integer.
            }

            return signals;
        }

        private short[] ReadSignalSamples(int startPosition, int numberOfSamples)
        {
            var samples = new List<short>();
            int countBytesRead = 0;

            this.BaseStream.Seek(startPosition, SeekOrigin.Begin);

            while (countBytesRead < numberOfSamples * 2) //2 bytes per integer
            {
                byte[] intBytes = this.ReadBytes(2);
                short intVal = BitConverter.ToInt16(intBytes, 0);
                samples.Add(intVal);
                countBytesRead += intBytes.Length;
            }

            return samples.ToArray();
        }

        private Int16 ReadInt16(int asciiLength)
        {
            string strInt = ReadAscii(asciiLength).Trim();
            Int16 intResult = -1;
            try { intResult = Convert.ToInt16(strInt); }
            catch (Exception ex) { Console.WriteLine("Error, could not convert string to integer. " + ex.Message); }
            return intResult;
        }

        private double ReadDouble(int asciiLength)
        {
            string strDouble = ReadAscii(asciiLength).Trim();
            return Convert.ToDouble(strDouble);
        }

        private string ReadAscii(int length)
        {
            byte[] bytes = this.ReadBytes(length);
            return AsciiString(bytes);
        }

        private string[] ReadMultipleAscii(int length, int numberOfParts)
        {
            var parts = new List<string>();

            for (int i = 0; i < numberOfParts; i++) {
                byte[] bytes = this.ReadBytes(length);
                parts.Add(AsciiString(bytes));
            }
            
            return parts.ToArray();
        }

        private int[] ReadMultipleInt(int length, int numberOfParts)
        {
            var parts = new List<int>();

            for (int i = 0; i < numberOfParts; i++)
            {
                byte[] bytes = this.ReadBytes(length);
                string ascii = AsciiString(bytes);
                parts.Add(Convert.ToInt32(ascii));
            }

            return parts.ToArray();
        }

        private double[] ReadMultipleDouble(int length, int numberOfParts)
        {
            var parts = new List<double>();

            for (int i = 0; i < numberOfParts; i++)
            {
                byte[] bytes = this.ReadBytes(length);
                string ascii = AsciiString(bytes);
                parts.Add(Convert.ToDouble(ascii));
            }

            return parts.ToArray();
        }

        private static string AsciiString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
