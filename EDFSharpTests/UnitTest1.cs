using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDFSharp;
using System.Linq;

namespace EDFSharpTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_WriteReadEDF_ShouldReturnSameData()
        {
            //Write an EDF file with two signals then read it and check the data is correct
            var edf1 = new EDFFile();

            var ecgSignal = new EDFSignal()
            {
                Label = "ECG",
                NumberOfSamples = 10, //Small number of samples for testing
                PhysicalDimension = "mV",
                DigitalMinimum = "-2048",
                DigitalMaximum = "2047",
                PhysicalMinimum = "-10.2325",
                PhysicalMaximum = "10.2325",
                TransducerType = "UNKNOWN",
                Prefiltering = "UNKNOWN",
                Samples = new short[] { 100, 50, 23, 75, 12, 88, 73, 12, 34, 83 },
                Reserved = "RESERVED"
            };

            var soundSignal = new EDFSignal()
            {
                Label = "SOUND",
                NumberOfSamples = 10, //Small number of samples for testing
                PhysicalDimension = "mV",
                DigitalMinimum = "-2048",
                DigitalMaximum = "2047",
                PhysicalMinimum = "-44",
                PhysicalMaximum = "44.0",
                TransducerType = "UNKNOWN",
                Prefiltering = "UNKNOWN",
                Samples = new short[] { 11, 200, 300, 123, 87, 204, 145, 234, 222, 75 },
                Reserved = "RESERVED"
            };

            edf1.Signals = new EDFSignal[2] { ecgSignal, soundSignal };

            var h = new EDFHeader();
            h.DurationOfDataRecord.Value = 1;
            h.Version.Value = "0";
            h.PatientID.Value = "TEST PATIENT ID";
            h.RecordID.Value = "TEST RECORD ID";
            h.StartDate.Value = "11.11.16"; //dd.mm.yy
            h.StartTime.Value = "12.12.12"; //hh.mm.ss
            h.Reserved.Value = "RESERVED";
            h.NumberOfDataRecords.Value = 1;
            h.NumberOfSignals.Value = (short)edf1.Signals.Length;
            h.SignalsReserved.Value = Enumerable.Repeat("RESERVED".PadRight(32, ' '), h.NumberOfSignals.Value).ToArray();

            edf1.Header = h;

            string edfFilePath = @"C:\temp\test1.EDF";
            edf1.Save(edfFilePath);

            //Read the file back
            var edf2 = new EDFFile(edfFilePath);

            Assert.AreEqual(edf2.Header.Version.ToAscii(),              edf1.Header.Version.ToAscii());
            Assert.AreEqual(edf2.Header.PatientID.ToAscii(),            edf1.Header.PatientID.ToAscii());
            Assert.AreEqual(edf2.Header.RecordID.ToAscii(),             edf1.Header.RecordID.ToAscii());
            Assert.AreEqual(edf2.Header.StartDate.ToAscii(),            edf1.Header.StartDate.ToAscii());
            Assert.AreEqual(edf2.Header.StartTime.ToAscii(),            edf1.Header.StartTime.ToAscii());
            Assert.AreEqual(edf2.Header.Reserved.ToAscii(),             edf1.Header.Reserved.ToAscii());
            Assert.AreEqual(edf2.Header.NumberOfDataRecords.ToAscii(),  edf1.Header.NumberOfDataRecords.ToAscii());
            Assert.AreEqual(edf2.Header.NumberOfSignals.ToAscii(),      edf1.Header.NumberOfSignals.ToAscii());
            Assert.AreEqual(edf2.Header.SignalsReserved.ToAscii(),      edf1.Header.SignalsReserved.ToAscii());
        }
    }
}
