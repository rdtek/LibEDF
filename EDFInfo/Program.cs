using System;
using System.IO;

namespace EDFInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length >= 1)
            {
                string filePath = args[0];
                if (File.Exists(filePath))
                {
                    EDFFile edf = new EDFFile(filePath);
                    edf.WriteFile(filePath + "_cleaned.EDF");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Input file not provided.");
            }
        }
    }
}
