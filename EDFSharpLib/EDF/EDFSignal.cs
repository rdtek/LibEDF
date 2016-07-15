using System.Linq;

namespace EDFSharp
{
    public class EDFSignal
    {
        public FixedLengthString Label              { get; } = new FixedLengthString(HeaderItems.Label);
        public FixedLengthString TransducerType     { get; } = new FixedLengthString(HeaderItems.TransducerType);
        public FixedLengthString PhysicalDimension  { get; } = new FixedLengthString(HeaderItems.PhysicalDimension);
        public FixedLengthDouble PhysicalMinimum    { get; } = new FixedLengthDouble(HeaderItems.PhysicalMinimum);
        public FixedLengthDouble PhysicalMaximum    { get; } = new FixedLengthDouble(HeaderItems.PhysicalMaximum);
        public FixedLengthInt DigitalMinimum     { get; } = new FixedLengthInt(HeaderItems.DigitalMinimum);
        public FixedLengthInt DigitalMaximum     { get; } = new FixedLengthInt(HeaderItems.DigitalMaximum);
        public FixedLengthString Prefiltering       { get; } = new FixedLengthString(HeaderItems.Prefiltering);
        public FixedLengthInt NumberOfSamples       { get; } = new FixedLengthInt(HeaderItems.NumberOfSamplesInDataRecord);
        public FixedLengthString Reserved           { get; } = new FixedLengthString(HeaderItems.SignalsReserved);

        public short[] Samples { get; set; } = new short[] { };

        public override string ToString()
        {
            return Label + " " + NumberOfSamples + " [" 
                + string.Join(",", Samples.Skip(0).Take(10).ToArray()) + " ...]";
        }
    }
}
