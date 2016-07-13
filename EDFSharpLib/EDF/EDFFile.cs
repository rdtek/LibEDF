using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EDFSharp
{
    public class EDFFile
    {
        public EDFHeader Header { get; set; }
        public EDFSignal[] Signals { get; set; }

        public EDFFile() { }
        public EDFFile(string edfFilePath) {
            Open(edfFilePath);
        }

        public EDFFile(byte[] edfBytes){
            Open(edfBytes);
        }

        public void Open(string edfFilePath)
        {
            using (var r = new EDFReader(File.Open(edfFilePath, FileMode.Open)))
            {
                Header = r.ReadHeader();
                Signals = r.ReadSignals();
            }
        }

        public void Open(byte[] edfBytes)
        {
            using (var r = new EDFReader(edfBytes))
            {
                Header = r.ReadHeader();
                Signals = r.ReadSignals();
            }
        }

        public void Save(string edfFilePath)
        {
            if (Header == null) return;
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

            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.Label.PadRight(16, ' '))), ns * ItemLengths.Label);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.TransducerType.PadRight(80, ' '))), ns * ItemLengths.TransducerType);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.PhysicalDimension.PadRight(8, ' '))), ns * ItemLengths.PhysicalDimension);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.PhysicalMinimum.PadRight(8, ' '))), ns * ItemLengths.PhysicalMinimum);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.PhysicalMaximum.PadRight(8, ' '))), ns * ItemLengths.PhysicalMaximum);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.DigitalMinimum.PadRight(8, ' '))), ns * ItemLengths.DigitalMinimum);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.DigitalMaximum.PadRight(8, ' '))), ns * ItemLengths.DigitalMaximum);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.Prefiltering.PadRight(80, ' '))), ns * ItemLengths.Prefiltering);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.NumberOfSamples.ToString().PadRight(8, ' '))), ns * ItemLengths.NumberOfSamples);
            hw.WriteAsciiItem(StrJoin(Signals.Select(s => s.Reserved.ToString().PadRight(32, ' '))), ns * ItemLengths.SignalsReserved);

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
