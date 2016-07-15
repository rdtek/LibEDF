using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDFSharp
{
    class EDFWriter : BinaryWriter
    {
        public EDFWriter(FileStream fs) : base(fs) { }

        public void WriteItem(HeaderItem headerItem)
        {
            string strItem = headerItem.ToAscii();
            if (strItem == null) strItem = "";
            byte[] itemBytes = AsciiToBytes(strItem);
            this.Write(itemBytes);
            Console.WriteLine(headerItem.Name + " [" + strItem + "] \n\n-- ** BYTES LENGTH: " + itemBytes.Length 
                + "> Position after write item: " + this.BaseStream.Position + "\n");
        }

        public void WriteItem(IEnumerable<HeaderItem> headerItems)
        {
            string joinedItems = StrJoin(headerItems);
            if (joinedItems == null) joinedItems = "";
            byte[] itemBytes = AsciiToBytes(joinedItems);
            this.Write(itemBytes);
            Console.WriteLine("[" + joinedItems + "] \n\n-- ** BYTES LENGTH: " + itemBytes.Length 
                + " Position after write item: " + this.BaseStream.Position + "\n");
        }

        private string StrJoin(IEnumerable<HeaderItem> list)
        {
            string joinedString = "";

            foreach (var item in list)
            {
                joinedString += item.ToAscii();
            }

            return joinedString;
        }

        private static byte[] AsciiToBytes(string strItem)
        {
            return Encoding.ASCII.GetBytes(strItem);
        }

        private static byte[] AsciiToIntBytes(string strItem, int length)
        {
            string strInt = "";
            string str = strItem.Substring(0, length);
            double val = Convert.ToDouble(str);
            strInt += val.ToString("0").PadRight(length, ' ');
            return Encoding.ASCII.GetBytes(strInt);
        }

        public void WriteSignal(EDFSignal signal)
        {
            Console.WriteLine("Write position before signal: " + this.BaseStream.Position);
            for (int i = 0; i < signal.NumberOfSamples.Value; i++)
            {
                this.Write(BitConverter.GetBytes(signal.Samples[i]));
            }
            Console.WriteLine("Write position after signal: " + this.BaseStream.Position);
        }
    }
}
