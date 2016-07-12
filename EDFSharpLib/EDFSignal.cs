using System.Linq;

namespace EDFSharp
{
    public class EDFSignal
    {
        public string Label { get; set; }
        public string TransducerType { get; set; }
        public string PhysicalDimension { get; set; }
        public string PhysicalMinimum { get; set; }
        public string PhysicalMaximum { get; set; }
        public string DigitalMinimum { get; set; }
        public string DigitalMaximum { get; set; }
        public string Prefiltering { get; set; }
        public int NumberOfSamples { get; set; }
        public string Reserved { get; set; }
        public short[] Samples { get; set; }

        public override string ToString()
        {
            return Label + " " + NumberOfSamples + " [" + string.Join(",", Samples.Skip(0).Take(10).ToArray()) + "]";
        }
    }
}
