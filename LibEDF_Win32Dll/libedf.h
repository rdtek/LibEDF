#pragma once
#include "definitions.h";
#include <Windows.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>

// Specify exported functions for DLL
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
			  // used by C++ source code
#endif
	DLL_FUNCTION void edf_printLibVersion();
	DLL_FUNCTION int edf_readEDF(const char* edfFileName, bool readSignalSamples);
	DLL_FUNCTION void edf_printHeader();
#ifdef __cplusplus
}
#endif

typedef struct {

	char version[9];
	char patientID[81];
	char recordID[81];
	char startDate[9];
	char startTime[9];
	int  numberOfBytesInHeader;
	char reserved[45];
	int  numberOfDataRecords;
	int  durationOfDataRecord;
	int  numberOfSignals;

	//Max 10 signals
	char labels[10][16];

	char   transducers[10][80];
	char   physicalDimensions[10][8];
	double physicalMinimums[10][8];
	double physicalMaximums[10][8];
	int    digitalMinimums[10][8];
	int    digitalMaximums[10][8];
	char   prefiltering[10][80];
	int    numberOfSamplesInDataRecord[10];
	char   signalsReserved[10][32];

} EDFHeader;

typedef struct {

	char label[16];
	int numberOfSamples;

	//TODO: allow larger array of samples to be loaded.
	short digitalSamples[1000];

} EDFSignal;

void edf_readSignals
(FILE* ptr_file, EDFSignal* ptr_edfSignalsOut, int numberOfSignals, int numberOfSamples);

void edf_readAscii
(FILE* ptr_file, size_t length, char* ptr_charBuffOut);

void edf_readMultipleAscii
(FILE* ptr_file, size_t numBytes, char* ptr_stringArrayOut, int numberOfItems);

void edf_readInt
(FILE* ptr_file, size_t length, int* ptr_intOut);

void edf_readMultipleInt
(FILE* ptr_file, size_t numBytes, int* ptr_intArrayOut, int numberOfItems);

void edf_readDouble
(FILE* ptr_file, size_t numBytes, double* prt_doubleOut);

void edf_readMultipleDouble
(FILE* ptr_file, size_t numBytes, double* ptr_doubleArrayOut, int numberOfItems);

void edf_stringsToAscii
(char* ptr_strArray, size_t itemSize, int numberOfItems);

void edf_doublesToAscii
(double* ptr_doubleArray, size_t itemSize, int numberOfItems);

void edf_intsToAscii
(int* ptr_intArray, size_t itemSize, int numberOfItems);