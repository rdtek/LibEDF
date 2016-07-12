using System;
using EDFSharp;
using InoviseCOM;

namespace FileConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = @"C:\temp\snap.RAW";

            //First read the RAW file to get signals metadata
            var hf = new HemoFile();
            hf.FileName = inputFilePath;
            if (hf.Open() != 0) return;
            
            Console.WriteLine("Hemo file info:");
            Console.WriteLine(hf.NumberOfChannels);

            Console.WriteLine(hf.ChannelName[0]);
            short[] ecgChanData = (short[])hf.ChannelData(0, 0, hf.DurationInSamples);
            //Console.WriteLine(string.Join(",", Array.ConvertAll(ecgChanData, x => x.ToString())));

            Console.WriteLine(hf.ChannelName[1]);
            short[] soundChanData = (short[])hf.ChannelData(1, 0, hf.DurationInSamples);
            //Console.WriteLine(string.Join(",", Array.ConvertAll(soundChanData, x => x.ToString())));

            Console.WriteLine(hf.DurationInSamples);

            //Then construct the EDF file
            EDFFile edfFile = new EDFFile();
            EDFSignal[] signals = new EDFSignal[2];

            EDFSignal ecgSignal = new EDFSignal();
            ecgSignal.Label = "Ecg0";
            ecgSignal.NumberOfSamples = hf.DurationInSamples;
            ecgSignal.PhysicalDimension = "mV";
            ecgSignal.DigitalMinimum = "-2048";
            ecgSignal.DigitalMaximum = "2047";
            ecgSignal.PhysicalMinimum = "-10.2325";
            ecgSignal.PhysicalMaximum = "10.2325";
            ecgSignal.TransducerType = "unknown";
            ecgSignal.Prefiltering = "unknown";
            ecgSignal.Samples = ecgChanData;
            ecgSignal.Reserved = "ECG signal reserved";

            EDFSignal soundSignal = new EDFSignal();
            soundSignal.Label = "SV4";
            soundSignal.NumberOfSamples = hf.DurationInSamples;
            soundSignal.PhysicalDimension = "mV";
            soundSignal.DigitalMinimum = "-2048";
            soundSignal.DigitalMaximum = "2047";
            soundSignal.PhysicalMinimum = "-44";
            soundSignal.PhysicalMaximum = "44.0";
            soundSignal.TransducerType = "unknown";
            soundSignal.Prefiltering = "unknown";
            soundSignal.Samples = soundChanData;
            soundSignal.Reserved = "Sound signal reserved.";

            signals[0] = ecgSignal;
            signals[1] = soundSignal;

            edfFile.Signals = signals;

            EDFHeader h = new EDFHeader();
            h.DurationOfDataRecord.Value = 10;
            h.Labels.Value = "Ecg0";
            h.Version.Value = "0";
            h.PatientID.Value = "TEST PATIENT ID";
            h.RecordID.Value = "TEST RECORD ID";
            h.StartDate.Value = "11.11.16"; //dd.mm.yy
            h.StartTime.Value = "12.12.12"; //hh.mm.ss
            h.Reserved.Value = "TEST RESERVED";
            h.NumberOfDataRecords.Value = 1;
            h.NumberOfSignals.Value = (short)signals.Length;
            h.NumberOfSamplesInDataRecord.Value = "5000".PadRight(8, ' ') + "5000".PadRight(8, ' ');
            h.SignalsReserved.Value = "TEST SIGNALS RESERVED".PadRight(h.NumberOfSignals.Value * 32, ' ');
            
            //------ Variable length header part --------
            /*
            h.Labels = ecgSignal.Label.PadRight(16,' ') + soundSignal.Label.PadRight(16, ' ');
            h.TransducerType = ecgSignal.TransducerType.PadRight(80, ' ') + soundSignal.TransducerType.PadRight(80, ' ');
            h.PhysicalDimension = ecgSignal.PhysicalDimension.PadRight(8, ' ') + soundSignal.PhysicalDimension.PadRight(8, ' ');
            h.PhysicalMinimum = ecgSignal.PhysicalMinimum.PadRight(8, ' ') + soundSignal.PhysicalMinimum.PadRight(8, ' ');
            h.PhysicalMaximum = ecgSignal.PhysicalMaximum.PadRight(8, ' ') + soundSignal.PhysicalMaximum.PadRight(8, ' ');
            h.DigitalMinimum = ecgSignal.DigitalMinimum.PadRight(8, ' ') + soundSignal.DigitalMinimum.PadRight(8, ' ');
            h.DigitalMaximum = ecgSignal.DigitalMaximum.PadRight(8, ' ') + soundSignal.DigitalMaximum.PadRight(8, ' ');
            h.Prefiltering = ecgSignal.Prefiltering.PadRight(80, ' ') + soundSignal.Prefiltering.PadRight(80, ' ');
            */

            edfFile.Header = h;
            edfFile.WriteFile(inputFilePath + ".new.EDF");

            Console.ReadLine();
        }
    }
}
