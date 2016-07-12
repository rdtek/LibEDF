using System;
using System.IO;

namespace EDFSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length >= 1)
            {
                string filePath = args[0];
                if (File.Exists(filePath)) {
                    EDFFile edf = new EDFFile(filePath);
                    edf.Save(filePath + "_cleaned.EDF");
                }
                else {
                    Console.WriteLine("File does not exist.");
                }
            }
            else {
                Console.WriteLine("Input file not provided.");
            }
            Console.ReadLine();
        }
    }
}
