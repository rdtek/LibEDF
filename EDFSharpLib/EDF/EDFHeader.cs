using System;

namespace EDFSharp
{
    public class ItemLengths
    {
        //Fixed length items
        public const int Version = 8;
        public const int PatientID = 80;
        public const int RecordID = 80;
        public const int StartDate = 8;
        public const int StartTime = 8;
        public const int NumberOfBytesInHeader = 8;
        public const int Reserved = 44;
        public const int NumberOfDataRecords = 8;
        public const int DurationOfDataRecord = 8;
        public const int NumberOfSignals = 4;

        //Variable length items
        public const int Label = 16;
        public const int TransducerType = 80;
        public const int PhysicalDimension = 8;
        public const int PhysicalMinimum = 8;
        public const int PhysicalMaximum = 8;
        public const int DigitalMinimum = 8;
        public const int DigitalMaximum = 8;
        public const int Prefiltering = 80;
        public const int NumberOfSamples = 8;
        public const int SignalsReserved = 32;
    }

    public interface IHeaderItem
    {
        string ToAscii();
    }

    public class FixedLengthString : IHeaderItem
    {
        public string Value { get; set; }
        public int AsciiLength { get; private set; } = 0;
        public FixedLengthString(int asciiLength){ AsciiLength = asciiLength; }
        public string ToAscii() { return Value.PadRight(AsciiLength, ' '); }
    }

    public class FixedLengthInt : IHeaderItem
    {
        public int Value { get; set; }
        public int AsciiLength { get; private set; } = 0;
        public FixedLengthInt(int asciiLength) { AsciiLength = asciiLength; }
        public string ToAscii() { return Value.ToString().PadRight(AsciiLength, ' '); }
    }

    public class FixedLengthDouble : IHeaderItem
    {
        public double Value { get; set; }
        public int AsciiLength { get; private set; } = 0;
        public FixedLengthDouble(int asciiLength) { AsciiLength = asciiLength; }
        public string ToAscii() { return Value.ToString().PadRight(AsciiLength, ' '); }
    }

    public class VariableLengthString : IHeaderItem
    {
        public string[] Value { get; set; }
        public VariableLengthString() {  }
        public string ToAscii() { return String.Join("", Value); }
    }

    public class VariableLengthInt : IHeaderItem
    {
        public int[] Value { get; set; }
        public VariableLengthInt() { }
        public string ToAscii() { return String.Join("", Value); }
    }

    public class VariableLengthDouble : IHeaderItem
    {
        public double[] Value { get; set; }
        public VariableLengthDouble() { }
        public string ToAscii() { return String.Join("", Value); }
    }

    public class EDFHeader
    {
        public FixedLengthString Version { get; private set; } = new FixedLengthString(ItemLengths.Version);
        public FixedLengthString PatientID { get; private set; } = new FixedLengthString(ItemLengths.PatientID);
        public FixedLengthString RecordID { get; private set; } = new FixedLengthString(ItemLengths.RecordID);
        public FixedLengthString StartDate { get; private set; } = new FixedLengthString(ItemLengths.StartDate);
        public FixedLengthString StartTime { get; private set; } = new FixedLengthString(ItemLengths.StartTime);
        public FixedLengthInt NumberOfBytesInHeader { get; private set; } = new FixedLengthInt(ItemLengths.NumberOfBytesInHeader);
        public FixedLengthString Reserved { get; private set; } = new FixedLengthString(ItemLengths.Reserved);
        public FixedLengthInt NumberOfDataRecords { get; private set; } = new FixedLengthInt(ItemLengths.NumberOfDataRecords);
        public FixedLengthInt DurationOfDataRecord { get; private set; } = new FixedLengthInt(ItemLengths.DurationOfDataRecord);
        public FixedLengthInt NumberOfSignals { get; private set; } = new FixedLengthInt(ItemLengths.NumberOfSignals);

        public VariableLengthString Labels { get; private set; } = new VariableLengthString();
        public VariableLengthString TransducerType { get; private set; } = new VariableLengthString();
        public VariableLengthString PhysicalDimension { get; private set; } = new VariableLengthString();
        public VariableLengthDouble PhysicalMinimum { get; private set; } = new VariableLengthDouble();
        public VariableLengthDouble PhysicalMaximum { get; private set; } = new VariableLengthDouble();
        public VariableLengthInt DigitalMinimum { get; private set; } = new VariableLengthInt();
        public VariableLengthInt DigitalMaximum { get; private set; } = new VariableLengthInt();
        public VariableLengthString Prefiltering { get; private set; } = new VariableLengthString();
        public VariableLengthInt NumberOfSamplesInDataRecord { get; private set; } = new VariableLengthInt();
        public VariableLengthString SignalsReserved { get; private set; } = new VariableLengthString();

        public EDFHeader() { }

        public override string ToString()
        {
            string strOutput = "";

            strOutput += "8b\tVersion [" + Version.Value + "]\n";
            strOutput += "80b\tPatient ID [" + PatientID.Value + "]\n";
            strOutput += "80b\tRecording ID [" + RecordID.Value + "]\n";
            strOutput += "8b\tStart Date [" + StartDate.Value + "]\n";
            strOutput += "8b\tStart Time [" + StartTime.Value + "\n]";
            strOutput += "8b\tNumber of bytes in header [" + NumberOfBytesInHeader.Value + "]\n";
            strOutput += "44b\tReserved [" + Reserved.Value + "]\n";
            strOutput += "8b\tNumber of data records [" + NumberOfDataRecords.Value + "]\n";
            strOutput += "8b\tDuration of data record [" + DurationOfDataRecord.Value + "]\n";
            strOutput += "4b\tNumber of signals [" + NumberOfSignals.Value + "]\n";

            strOutput += "\tLabels [" + Labels.Value + "]\n";
            strOutput += "\tTransducer type [" + TransducerType.Value + "]\n";
            strOutput += "\tPhysical dimension [" + PhysicalDimension.Value + "]\n";
            strOutput += "\tPhysical minimum [" + PhysicalMinimum.Value + "]\n";
            strOutput += "\tPhysical maximum [" + PhysicalMaximum.Value + "]\n";
            strOutput += "\tDigital minimum [" + DigitalMinimum.Value + "]\n";
            strOutput += "\tDigital maximum [" + DigitalMaximum.Value + "]\n";
            strOutput += "\tPrefiltering [" + Prefiltering.Value + "]\n";
            strOutput += "\tNumber of samples in data record [" + NumberOfSamplesInDataRecord.Value + "]\n";
            strOutput += "\tSignals reserved [" + SignalsReserved.Value + "]\n";

            Console.WriteLine("\n---------- EDF File Header ---------\n" + strOutput);

            return strOutput;
        }
    }
}
