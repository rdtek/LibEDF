using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EDFInfo
{
    public class EDFFile
    {
        public EDFHeader Header { get; set; }
        public EDFSignal[] Signals { get; set; }

        public EDFFile() { }
        public EDFFile(string edfFilePath) {
            //Read the header and signal data from the EDF file
            ReadFile(edfFilePath);
        }

        public void ReadFile(string edfFilePath)
        {
            ReadHeader(edfFilePath);
            ReadSignals(edfFilePath);

            Console.WriteLine(Signals[0].ToString());
            Console.WriteLine(Signals[1].ToString());
        }

        private void ReadHeader(string edfFilePath)
        {
            Header = new EDFHeader(edfFilePath);
        }

        private void ReadSignals(string edfFilePath)
        {
            char[] splitChar = new char[] { ' ' };
            string[] labels = Header.Labels.Value.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            string[] strNumSamples = Header.NumberOfSamplesInDataRecord.Value.ToString()
                .Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

            //Init signal objects
            Signals = new EDFSignal[Header.NumberOfSignals.Value];
            
            for (int i = 0; i < Signals.Length; i++)
            {
                var sig = new EDFSignal();
                sig.Label = labels[i];
                sig.NumberOfSamples = Convert.ToInt16(strNumSamples[i]);
                Signals[i] = sig;
            }

            //Read the signal sample values
            int readPosition = Header.NumberOfBytesInHeader.Value;

            for (int i = 0; i < Signals.Length; i++)
            {
                Signals[i].Samples = ReadSignalSamples(edfFilePath, readPosition, Signals[i].NumberOfSamples);
                readPosition += Signals[i].Samples.Length * 2; //2 bytes per integer.
            }
        }

        private short[] ReadSignalSamples(string edfFilePath, int startPosition, int numberOfSamples)
        {
            var samples = new List<short>();
            int countBytesRead = 0;

            using (BinaryReader bReader = new BinaryReader(File.Open(edfFilePath, FileMode.Open)))
            {
                bReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);

                while(countBytesRead < numberOfSamples * 2) //2 bytes per integer
                {
                    byte[] intBytes = bReader.ReadBytes(2);
                    short intVal = BitConverter.ToInt16(intBytes, 0);
                    samples.Add(intVal);
                    countBytesRead += intBytes.Length;
                }
            }

            return samples.ToArray();
        }

        public void WriteFile(string edfFilePath)
        {
            if (Header == null) return;
            
            //Write the header first
            Console.WriteLine("Writing header.");

            var hw = new EDFWriter(File.Open(edfFilePath, FileMode.Create));

            Header.NumberOfBytesInHeader.Value = CalcNumOfBytesInHeader();

            //----------------- Fixed length header items -----------------
            hw.WriteAsciiItem(Header.Version.Value, 8);
            hw.WriteAsciiItem(Header.PatientID.Value, 80);
            hw.WriteAsciiItem(Header.RecordID.Value, 80);
            hw.WriteAsciiItem(Header.StartDate.Value, 8);
            hw.WriteAsciiItem(Header.StartTime.Value, 8);
            hw.WriteAsciiItem(Header.NumberOfBytesInHeader.Value.ToString(), 8);
            hw.WriteAsciiItem(Header.Reserved.Value, 44);
            hw.WriteAsciiItem(Header.NumberOfDataRecords.Value.ToString(), 8);
            hw.WriteAsciiItem(Header.DurationOfDataRecord.Value.ToString(), 8);
            hw.WriteAsciiItem(Header.NumberOfSignals.Value.ToString(), 4);

            //----------------- Variable length header items -----------------
            int ns = Signals.Length;

            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.Label.PadRight(16, ' '))), ns * 16);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.TransducerType.PadRight(8, ' '))), ns * 80);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.PhysicalDimension.PadRight(8, ' '))), ns * 8);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.PhysicalMinimum.PadRight(8, ' '))), ns * 8);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.PhysicalMaximum.PadRight(8, ' '))), ns * 8);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.DigitalMinimum.PadRight(8, ' '))), ns * 8);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.DigitalMaximum.PadRight(8, ' '))), ns * 8);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.Prefiltering.PadRight(32, ' '))), ns * 80);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.NumberOfSamples.ToString().PadRight(8, ' '))), ns * 8);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.NumberOfSamples.ToString().PadRight(8, ' '))), ns * 32);

            Console.WriteLine("Writer position after header: " + hw.BaseStream.Position);

            Console.WriteLine("Writing signals.");
            foreach (var sig in Signals) hw.WriteSignal(sig);

            hw.Close();
            Console.WriteLine("File size: " + File.ReadAllBytes(edfFilePath).Length);
        }
        
        private string StrJoin(IEnumerable<string> list)
        {
            string joinedString = "";

            joinedString = String.Join("", list);

            return joinedString;
        }

        private int CalcNumOfBytesInHeader()
        {
            int totalFixedLength = 256;
            int ns = Signals.Length;
            int totalVariableLength = ns * 16 + (ns * 80) * 2 + (ns * 8) * 6 + (ns * 32);
            return totalFixedLength + totalVariableLength;
        }    
    }
}
