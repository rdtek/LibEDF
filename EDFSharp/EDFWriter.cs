using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EDFSharp
{
    class EDFWriter : BinaryWriter
    {
        public EDFWriter(FileStream fs) : base(fs)
        {

        }

        public void WriteHeaderItem(IHeaderItem item, int requiredLength)
        {
            byte[] itemBytes = AsciiToBytes(item.ToAscii());

            this.Write(itemBytes);
            int numPadding = requiredLength - itemBytes.Length;

            if (numPadding >= 1)
            {
                string strPadding = new string(' ', numPadding);
                byte[] bPadding = Encoding.ASCII.GetBytes(strPadding);
                this.Write(bPadding);
            }
        }

        public void WriteAsciiItem(string asciiItem, int requiredLength)
        {
            if (asciiItem.Length > requiredLength)
                asciiItem = asciiItem.Substring(0, requiredLength);

            byte[] itemBytes = AsciiToBytes(asciiItem);

            this.Write(itemBytes);
            int numPadding = requiredLength - itemBytes.Length;

            if (numPadding >= 1)
            {
                string strPadding = new string(' ', numPadding);
                byte[] bPadding = Encoding.ASCII.GetBytes(strPadding);
                this.Write(bPadding);
            }

            Console.WriteLine("[" + asciiItem + "] \t\t<" + itemBytes .Length + "> Position after write item: " + this.BaseStream.Position);
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
            for (int i = 0; i < signal.NumberOfSamples; i++)
            {
                this.Write(BitConverter.GetBytes(signal.Samples[i]));
            }
            Console.WriteLine("Write position after signal: " + this.BaseStream.Position);
        }
    }
}
