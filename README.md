# EDF-Sharp
Library to read and write EDF signal format files as defined in the [EDF specification](http://www.edfplus.info/specs/edf.html).

This project is provided under the terms of the [MIT license](http://choosealicense.com/licenses/mit/).

### Usage

```cs
//Load an EDF file.
var edf = new EDFFile(@"C:\temp\example.edf");
 
//Print some info
Console.Write(
    "\n" + edf.Header.PatientID.Value
    "\n" + edf.Header.NumberOfSignals.Value
    "\n" + edf.Header.StartDate.Value
    "\n" + edf.Signals[0].Label.Value
    "\n" + String.Join(",", edf.Signals[0].Samples.Skip(0).Take(10).ToArray())
    "\n" + edf.Signals[1].Label.Value
    "\n" + String.Join(",", edf.Signals[1].Samples.Skip(0).Take(10).ToArray())
 );

//Edit the file
edf.Signals[0].Label.Value = "Signal 1";
edf.Signals[0].Label.Value = "Signal 2";
edf.Header.Reserved.Value = "Some extra metadata";
edf.Signals[1].Samples = new short[] { 100, 120, 150, 200, 300, 270, 230, 190, 139, 150 };

//Save the modified file
edf.Save(@"C:\edited.edf");
```

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
