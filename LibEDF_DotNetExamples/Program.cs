using System;
using System.Linq;
using LibEDF_DotNet;
using System.IO;

namespace LibEDF_DotNetExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Example1_Create_And_Save_EDF();

            if(args.Length >= 1)
                Example2_Read_EDF_From_Base64(args[0]);
        }

        private static void Example1_Create_And_Save_EDF() {

            //Crreate an empty EDF file
            var edfFile = new EDFFile();

            //Create a signal object
            var ecgSig = new EDFSignal();
            ecgSig.Label.Value = "ECG";
            ecgSig.NumberOfSamples.Value = 10;
            ecgSig.PhysicalDimension.Value = "mV";
            ecgSig.DigitalMinimum.Value = -2048;
            ecgSig.DigitalMaximum.Value = 2047;
            ecgSig.PhysicalMinimum.Value = -10.2325;
            ecgSig.PhysicalMaximum.Value = 10.2325;
            ecgSig.TransducerType.Value = "UNKNOWN";
            ecgSig.Prefiltering.Value = "UNKNOWN";
            ecgSig.Reserved.Value = "RESERVED";
            ecgSig.Samples = new short[] { 100, 50, 23, 75, 12, 88, 73, 12, 34, 83 };

            //Set the signal
            edfFile.Signals = new EDFSignal[1] { ecgSig };

            //Create the header object
            var h = new EDFHeader();
            h.DurationOfDataRecord.Value = 1;
            h.Version.Value = "0";
            h.PatientID.Value = "TEST PATIENT ID";
            h.RecordID.Value = "TEST RECORD ID";
            h.StartDate.Value = "11.11.16"; //dd.mm.yy
            h.StartTime.Value = "12.12.12"; //hh.mm.ss
            h.Reserved.Value = "RESERVED";
            h.NumberOfDataRecords.Value = 1;
            h.NumberOfSignals.Value = (short)edfFile.Signals.Length;
            h.SignalsReserved.Value = Enumerable.Repeat("RESERVED".PadRight(32, ' '),
                                                h.NumberOfSignals.Value).ToArray();

            //Set the header
            edfFile.Header = h;

            //Print some info
            Console.Write(
                "\nPatient ID:\t\t" + edfFile.Header.PatientID.Value +
                "\nNumber of signals:\t" + edfFile.Header.NumberOfSignals.Value +
                "\nStart date:\t\t" + edfFile.Header.StartDate.Value +
                "\nSignal label:\t\t" + edfFile.Signals[0].Label.Value +
                "\nSignal samples:\t\t"
                    + String.Join(",", edfFile.Signals[0].Samples.Skip(0).Take(10).ToArray())
                    + "\n\n"
             );

            //Save the file
            string fileName = @"C:\temp\example.edf";
            edfFile.Save(fileName);

            //Read the file
            var f = new EDFFile(fileName);

            Console.ReadLine();
        }

        private static void Example2_Read_EDF_From_Base64(string edfBase64FilePath) {
            var edfBase64 = File.ReadAllText(edfBase64FilePath);
            var edfFile = new EDFFile();
            edfFile.ReadBase64(edfBase64);
            edfFile.Save(@"C:\temp\edf_bytes.edf");
        }
    }
}
