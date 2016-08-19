![EDF file icon](Doc/edflib_icon.png?raw=true "EDF file icon")

LibEDF is a library to read and write EDF signal files as defined in the [EDF specification](http://www.edfplus.info/specs/edf.html).

This project is provided under the terms of the [MIT license](http://choosealicense.com/licenses/mit/).

## C# Usage

The project LibEDF_CSharp builds to create a COM visible DLL.  
To use the library in a C# project reference the LibEDF_CSharp project or DLL and use the namespace LibEDF_CSharp as shown in the example code below.

```cs
using LibEDF_CSharp;
...

//Crreate an empty EDF file
var edfFile = new EDFFile();

//Create a signal object
var ecgSig = new EDFSignal();
ecgSig.Label.Value              = "ECG";
ecgSig.NumberOfSamples.Value    = 10;
ecgSig.PhysicalDimension.Value  = "mV";
ecgSig.DigitalMinimum.Value     = -2048;
ecgSig.DigitalMaximum.Value     = 2047;
ecgSig.PhysicalMinimum.Value    = -10.2325;
ecgSig.PhysicalMaximum.Value    = 10.2325;
ecgSig.TransducerType.Value     = "UNKNOWN";
ecgSig.Prefiltering.Value       = "UNKNOWN";
ecgSig.Reserved.Value           = "RESERVED";
ecgSig.Samples = new short[] { 100, 50, 23, 75, 12, 88, 73, 12, 34, 83 };

//Set the signal
edfFile.Signals = new EDFSignal[1] { ecgSig };

//Create the header object
var h = new EDFHeader();
h.DurationOfDataRecord.Value    = 1;
h.Version.Value                 = "0";
h.PatientID.Value               = "TEST PATIENT ID";
h.RecordID.Value                = "TEST RECORD ID";
h.StartDate.Value               = "11.11.16"; //dd.mm.yy
h.StartTime.Value               = "12.12.12"; //hh.mm.ss
h.Reserved.Value                = "RESERVED";
h.NumberOfDataRecords.Value     = 1;
h.NumberOfSignals.Value         = (short)edfFile.Signals.Length;
h.SignalsReserved.Value         = Enumerable.Repeat("RESERVED".PadRight(32, ' '), 
                                    h.NumberOfSignals.Value).ToArray();

//Set the header
edfFile.Header = h;

//Print some info
Console.Write(
    "\nPatient ID:\t\t"      + edfFile.Header.PatientID.Value +
    "\nNumber of signals:\t" + edfFile.Header.NumberOfSignals.Value +
    "\nStart date:\t\t"      + edfFile.Header.StartDate.Value +
    "\nSignal label:\t\t"    + edfFile.Signals[0].Label.Value +
    "\nSignal samples:\t\t" + String.Join(",", edfFile.Signals[0].Samples.Skip(0).Take(10).ToArray())
 );

//Save the file
string fileName = @"C:\temp\example.edf";
edfFile.Save(fileName);

//Read the file
var f = new EDFFile(fileName);
```

![Console example screenshot](Doc/edf_example_console.png?raw=true)

## Win32 usage
The project LibEDF_Win32Dll builds to create an unmanaged .dll and associated .lib file.  
Include those files and the header file libedf.h to call functions from an unmanaged Win32 project."

### Header Record 

| # Chars | File description                               |
|---------|------------------------------------------------|
|8 ascii  | version of this data format (0) |
|80 ascii | local patient identification |
|80 ascii | local recording identification |
|8 ascii  | startdate of recording (dd.mm.yy)|
|8 ascii  | starttime of recording (hh.mm.ss) |
|8 ascii  | number of bytes in header record |
|44 ascii | reserved |
|8 ascii  | number of data records|
|8 ascii  | duration of a data record, in seconds |
|4 ascii  | number of signals (ns) in data record |
|ns * 16 ascii | ns * label (e.g. EEG Fpz-Cz or Body temp)| 
|ns * 80 ascii | ns * transducer type (e.g. AgAgCl electrode) |
|ns * 8 ascii  | ns * physical dimension (e.g. uV or degreeC) |
|ns * 8 ascii  | ns * physical minimum (e.g. -500 or 34) |
|ns * 8 ascii  | ns * physical maximum (e.g. 500 or 40) |
|ns * 8 ascii  | ns * digital minimum (e.g. -2048) |
|ns * 8 ascii  | ns * digital maximum (e.g. 2047) |
|ns * 80 ascii | ns * prefiltering (e.g. HP:0.1Hz LP:75Hz) |
|ns * 8 ascii  | ns * nr of samples in each data record |
|ns * 32 ascii | ns * reserved|

### Data Record 

| # Chars                   | File description                |
|---------------------------|---------------------------------|
|nr of samples[1] * integer | first signal in the data record |
|nr of samples[2] * integer | second signal                   |
|.. | |
|.. | |
|nr of samples[ns] * integer | last signal | 
