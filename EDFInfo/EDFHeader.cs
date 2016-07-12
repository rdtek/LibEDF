using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDFInfo
{
    public interface IHeaderItem
    {
        string ToAscii();
    }

    public class StringItem : IHeaderItem
    {
        public string Value { get; set; }

        public string ToAscii()
        {
            throw new NotImplementedException();
        }
    }

    public class IntegerItem : IHeaderItem
    {
        public int Value { get; set; }

        public string ToAscii()
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleItem : IHeaderItem
    {
        public double Value { get; set; }

        public string ToAscii()
        {
            throw new NotImplementedException();
        }
    }

    public class EDFHeader
    {
        public StringItem Version { get; set; } = new StringItem();
        public StringItem PatientID { get; set; } = new StringItem();
        public StringItem RecordID { get; set; } = new StringItem();
        public StringItem StartDate { get; set; } = new StringItem();
        public StringItem StartTime { get; set; } = new StringItem();
        public IntegerItem NumberOfBytesInHeader { get; set; } = new IntegerItem();
        public StringItem Reserved { get; set; } = new StringItem();
        public IntegerItem NumberOfDataRecords { get; set; } = new IntegerItem();
        public IntegerItem DurationOfDataRecord { get; set; } = new IntegerItem();
        public IntegerItem NumberOfSignals { get; set; } = new IntegerItem();
        public StringItem Labels { get; set; } = new StringItem();
        public StringItem TransducerType { get; set; } = new StringItem();
        public StringItem PhysicalDimension { get; set; } = new StringItem();
        public DoubleItem PhysicalMinimum { get; set; } = new DoubleItem();
        public DoubleItem PhysicalMaximum { get; set; } = new DoubleItem();
        public IntegerItem DigitalMinimum { get; set; } = new IntegerItem();
        public IntegerItem DigitalMaximum { get; set; } = new IntegerItem();
        public StringItem Prefiltering { get; set; } = new StringItem();
        public StringItem NumberOfSamplesInDataRecord { get; set; } = new StringItem();
        public StringItem SignalsReserved { get; set; } = new StringItem();

        public EDFHeader() { }

        public EDFHeader(string edfFilePath)
        {
            ReadFromFile(edfFilePath);
        }

        public void ReadFromFile(string edfFilePath)
        {
            Console.WriteLine("EDF file path: " + edfFilePath);
            Console.WriteLine("Number of bytes in file: " + File.ReadAllBytes(edfFilePath).Length);

            //Print the header
            using (BinaryReader b = new BinaryReader(File.Open(edfFilePath, FileMode.Open)))
            {
                //------ Fixed length header part --------
                Version.Value               = ReadAscii(b, 8);
                PatientID.Value             = ReadAscii(b, 80);
                RecordID.Value              = ReadAscii(b, 80);
                StartDate.Value             = ReadAscii(b, 8);
                StartTime.Value             = ReadAscii(b, 8);
                NumberOfBytesInHeader.Value = ReadInt16(b, 8);
                Reserved.Value              = ReadAscii(b, 44);
                NumberOfDataRecords.Value   = ReadInt16(b, 8);
                DurationOfDataRecord.Value  = ReadInt16(b, 8);
                NumberOfSignals.Value       = ReadInt16(b, 4);

                //------ Variable length header part --------
                int ns = NumberOfSignals.Value;
                Labels.Value                        = ReadAscii(b, ns * 16);
                TransducerType.Value                = ReadAscii(b, ns * 80);
                PhysicalDimension.Value             = ReadAscii(b, ns * 8);
                PhysicalMinimum.Value               = ReadDouble(b, ns * 8);
                PhysicalMaximum.Value               = ReadDouble(b, ns * 8);
                DigitalMinimum.Value                = ReadInt16(b, ns * 8);
                DigitalMaximum.Value                = ReadInt16(b, ns * 8);
                Prefiltering.Value                  = ReadAscii(b, ns * 80);
                NumberOfSamplesInDataRecord.Value   = ReadAscii(b, ns * 8);
                SignalsReserved.Value               = ReadAscii(b, ns * 32);
            }

            File.WriteAllText(edfFilePath + ".txt", this.ToString());
        }

        private Int16 ReadInt16(BinaryReader bReader, int asciiLength)
        {
            string strInt = ReadAscii(bReader, asciiLength).Trim();
            Int16 intResult = -1;
            try { intResult = Convert.ToInt16(strInt); }
            catch (Exception ex) { Console.WriteLine("Error, could not convert string to intger. " + ex.Message); }
            return intResult;
        }

        private Int64 ReadInt64(BinaryReader bReader, int asciiLength)
        {
            string strInt = ReadAscii(bReader, asciiLength).Trim();
            Int64 intResult = -1;
            try { intResult = Convert.ToInt64(strInt); }
            catch (Exception ex) { Console.WriteLine("Error, could not convert string to intger. " + ex.Message); }
            return intResult;
        }

        private double ReadDouble(BinaryReader bReader, int asciiLength)
        {
            string strDouble = ReadAscii(bReader, asciiLength).Trim();
            return Convert.ToDouble(strDouble);
        }

        private string ReadAscii(BinaryReader bReader, int length)
        {
            byte[] bytes = bReader.ReadBytes(length);
            return AsciiString(bytes);
        }

        private static string AsciiString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public override string ToString()
        {
            string strOutput = "";

            strOutput += "8b\tVersion [" + Version + "]\n";
            strOutput += "80b\tPatient ID [" + PatientID + "]\n";
            strOutput += "80b\tRecording ID [" + RecordID + "]\n";
            strOutput += "8b\tStart Date [" + StartDate + "]\n";
            strOutput += "8b\tStart Time [" + StartTime + "\n]";
            strOutput += "8b\tNumber of bytes in header [" + NumberOfBytesInHeader + "]\n";
            strOutput += "44b\tReserved [" + Reserved + "]\n";
            strOutput += "8b\tNumber of data records [" + NumberOfDataRecords + "]\n";
            strOutput += "8b\tDuration of data record [" + DurationOfDataRecord + "]\n";
            strOutput += "4b\tNumber of signals [" + NumberOfSignals + "]\n";

            strOutput += "\tLabels [" + Labels + "]\n";
            strOutput += "\tTransducer type [" + TransducerType + "]\n";
            strOutput += "\tPhysical dimension [" + PhysicalDimension + "]\n";
            strOutput += "\tPhysical minimum [" + PhysicalMinimum + "]\n";
            strOutput += "\tPhysical maximum [" + PhysicalMaximum + "]\n";
            strOutput += "\tDigital minimum [" + DigitalMinimum + "]\n";
            strOutput += "\tDigital maximum [" + DigitalMaximum + "]\n";
            strOutput += "\tPrefiltering [" + Prefiltering + "]\n";
            strOutput += "\tNumber of samples in data record [" + NumberOfSamplesInDataRecord + "]\n";
            strOutput += "\tSignals reserved [" + SignalsReserved + "]\n";

            Console.WriteLine("\n---------- EDF File Header ---------\n" + strOutput);

            return strOutput;
        }
    }
}
