using System;

namespace EDFSharp
{
    public class EDFField
    {
        public string Name { get; set; }
        public int AsciiLength { get; set; }

        public EDFField() { }

        public EDFField(string name, int asciiLength) {
            Name = name;
            AsciiLength = asciiLength;
        }
    }

    public class HeaderItems
    {
        //Fixed length header items

        public static EDFField Version { get; } = new EDFField("Version", 8);
        public static EDFField PatientID { get; } = new EDFField("PatientID", 80);
        public static EDFField RecordID { get; private set; } = new EDFField("RecordID", 80);
        public static EDFField StartDate { get; private set; } = new EDFField("StartDate", 8);
        public static EDFField StartTime { get; private set; } = new EDFField("StartTime", 8);
        public static EDFField NumberOfBytesInHeader { get; private set; } = new EDFField("NumberOfBytesInHeader", 8);
        public static EDFField Reserved { get; private set; }  = new EDFField("Reserved", 44);
        public static EDFField NumberOfDataRecords { get; private set; } = new EDFField("NumberOfDataRecords", 8);
        public static EDFField DurationOfDataRecord { get; private set; } = new EDFField("DurationOfDataRecord", 8);
        public static EDFField NumberOfSignals { get; private set; } = new EDFField("NumberOfSignals", 4);

        //Variable length header items

        public static EDFField Label { get; private set; } = new EDFField("Labels", 16);
        public static EDFField TransducerType { get; private set; } = new EDFField("TransducerType", 80);
        public static EDFField PhysicalDimension { get; private set; } = new EDFField("PhysicalDimension", 8);
        public static EDFField PhysicalMinimum { get; private set; } = new EDFField("PhysicalMinimum", 8);
        public static EDFField PhysicalMaximum { get; private set; } = new EDFField("PhysicalMaximum", 8);
        public static EDFField DigitalMinimum { get; private set; } = new EDFField("DigitalMinimum", 8);
        public static EDFField DigitalMaximum { get; private set; } = new EDFField("DigitalMaximum", 8);
        public static EDFField Prefiltering { get; private set; } = new EDFField("Prefiltering", 80);
        public static EDFField NumberOfSamplesInDataRecord { get; private set; } = new EDFField("NumberOfSamplesInDataRecord", 8);
        public static EDFField SignalsReserved { get; private set; } = new EDFField("SignalsReserved", 32);
    }

    public abstract class HeaderItem
    {
        public HeaderItem(EDFField info) {
            Name = info.Name;
            AsciiLength = info.AsciiLength;
        }
        public string Name { get; set; }
        public int AsciiLength { get; set; }
        public abstract string ToAscii();
    }

    public class FixedLengthString : HeaderItem
    {
        public string Value { get; set; }
        public FixedLengthString(EDFField info) : base(info) { }

        public override string ToAscii() {
            string asciiString = "";
            if (Value != null)
                asciiString = Value.PadRight(AsciiLength, ' ');
            else
                asciiString = asciiString.PadRight(AsciiLength, ' ');

            return asciiString;
        }
    }

    public class FixedLengthInt : HeaderItem
    {
        public int Value { get; set; }
        public FixedLengthInt(EDFField info) : base(info) { }

        public override string ToAscii()
        {
            string asciiString = "";
            if (Value != null)
                asciiString = Value.ToString().PadRight(AsciiLength, ' ');
            else
                asciiString = asciiString.PadRight(AsciiLength, ' ');

            return asciiString;
        }
    }

    public class FixedLengthDouble : HeaderItem
    {
        public double Value { get; set; }
        public FixedLengthDouble(EDFField info) : base(info) { }

        public override string ToAscii()
        {
            string asciiString = "";
            if (Value != null)
            {
                asciiString = Value.ToString();
                if (asciiString.Length >= AsciiLength)
                    asciiString = asciiString.Substring(0, AsciiLength);
                else
                    asciiString = Value.ToString().PadRight(AsciiLength, ' ');
            }
                
            else
                asciiString = asciiString.PadRight(AsciiLength, ' ');

            return asciiString;
        }
    }

    public class VariableLengthString : HeaderItem
    {
        public string[] Value { get; set; }
        public VariableLengthString(EDFField info) : base(info) { }

        public override string ToAscii() {
            string ascii = "";
            foreach (var strVal in Value)
            {
                string temp = strVal.ToString();
                if (strVal.Length > AsciiLength)
                    temp = temp.Substring(0, AsciiLength);
                ascii += temp;
            }
            return ascii;
        }
    }

    public class VariableLengthInt : HeaderItem
    {
        public int[] Value { get; set; }
        public VariableLengthInt(EDFField info) : base(info) { }

        public override string ToAscii() {
            string ascii = "";
            foreach (var intVal in Value)
            {
                string temp = intVal.ToString();
                if (temp.Length > AsciiLength)
                    temp = temp.Substring(0, AsciiLength);
                ascii += temp;
            }
            return ascii;
        }
    }

    public class VariableLengthDouble : HeaderItem
    {
        public double[] Value { get; set; }
        public VariableLengthDouble(EDFField info) : base(info) { }

        public override string ToAscii() {
            string ascii = "";
            foreach (var doubleVal in Value)
            {
                string temp = doubleVal.ToString();
                if (temp.Length > AsciiLength)
                    temp = temp.Substring(0, AsciiLength);
                ascii += temp;
            }
            return ascii;
        }
    }

    public class EDFHeader
    {
        public FixedLengthString Version { get; private set; } = new FixedLengthString(HeaderItems.Version);
        public FixedLengthString PatientID { get; private set; } = new FixedLengthString(HeaderItems.PatientID);
        public FixedLengthString RecordID { get; private set; } = new FixedLengthString(HeaderItems.RecordID);
        public FixedLengthString StartDate { get; private set; } = new FixedLengthString(HeaderItems.StartDate);
        public FixedLengthString StartTime { get; private set; } = new FixedLengthString(HeaderItems.StartTime);
        public FixedLengthInt NumberOfBytesInHeader { get; private set; } = new FixedLengthInt(HeaderItems.NumberOfBytesInHeader);
        public FixedLengthString Reserved { get; private set; } = new FixedLengthString(HeaderItems.Reserved);
        public FixedLengthInt NumberOfDataRecords { get; private set; } = new FixedLengthInt(HeaderItems.NumberOfDataRecords);
        public FixedLengthInt DurationOfDataRecord { get; private set; } = new FixedLengthInt(HeaderItems.DurationOfDataRecord);
        public FixedLengthInt NumberOfSignals { get; private set; } = new FixedLengthInt(HeaderItems.NumberOfSignals);

        public VariableLengthString Labels { get; private set; } = new VariableLengthString(HeaderItems.Label);
        public VariableLengthString TransducerType { get; private set; } = new VariableLengthString(HeaderItems.TransducerType);
        public VariableLengthString PhysicalDimension { get; private set; } = new VariableLengthString(HeaderItems.PhysicalDimension);
        public VariableLengthDouble PhysicalMinimum { get; private set; } = new VariableLengthDouble(HeaderItems.PhysicalMinimum);
        public VariableLengthDouble PhysicalMaximum { get; private set; } = new VariableLengthDouble(HeaderItems.PhysicalMaximum);
        public VariableLengthInt DigitalMinimum { get; private set; } = new VariableLengthInt(HeaderItems.DigitalMinimum);
        public VariableLengthInt DigitalMaximum { get; private set; } = new VariableLengthInt(HeaderItems.DigitalMaximum);
        public VariableLengthString Prefiltering { get; private set; } = new VariableLengthString(HeaderItems.Prefiltering);
        public VariableLengthInt NumberOfSamplesInDataRecord { get; private set; } = new VariableLengthInt(HeaderItems.NumberOfSamplesInDataRecord);
        public VariableLengthString SignalsReserved { get; private set; } = new VariableLengthString(HeaderItems.SignalsReserved);

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
