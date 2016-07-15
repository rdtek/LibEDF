using System;
using System.IO;
using System.Linq;

namespace EDFSharp
{
    public class EDFFile
    {
        public EDFHeader Header { get; set; } = new EDFHeader();
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
            hw.WriteItem(Header.Version);
            hw.WriteItem(Header.PatientID);
            hw.WriteItem(Header.RecordID);
            hw.WriteItem(Header.StartDate);
            hw.WriteItem(Header.StartTime);
            hw.WriteItem(Header.NumberOfBytesInHeader);
            hw.WriteItem(Header.Reserved);
            hw.WriteItem(Header.NumberOfDataRecords);
            hw.WriteItem(Header.DurationOfDataRecord);
            hw.WriteItem(Header.NumberOfSignals);

            //----------------- Variable length header items -----------------
            hw.WriteItem(Signals.Select(s => s.Label));
            hw.WriteItem(Signals.Select(s => s.TransducerType));
            hw.WriteItem(Signals.Select(s => s.PhysicalDimension));
            hw.WriteItem(Signals.Select(s => s.PhysicalMinimum));
            hw.WriteItem(Signals.Select(s => s.PhysicalMaximum));
            hw.WriteItem(Signals.Select(s => s.DigitalMinimum));
            hw.WriteItem(Signals.Select(s => s.DigitalMaximum));
            hw.WriteItem(Signals.Select(s => s.Prefiltering));
            hw.WriteItem(Signals.Select(s => s.NumberOfSamples));
            hw.WriteItem(Signals.Select(s => s.Reserved));

            Console.WriteLine("Writer position after header: " + hw.BaseStream.Position);
            Console.WriteLine("Writing signals.");
            foreach (var sig in Signals) hw.WriteSignal(sig);

            hw.Close();
            Console.WriteLine("File size: " + File.ReadAllBytes(edfFilePath).Length);
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
