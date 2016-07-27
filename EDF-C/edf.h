#ifndef HEADER_EDF
#define HEADER_EDF

#include <stdlib.h>
#include <stdio.h>

typedef struct {

	char version[9];
	char patientID[81];
	char recordID[81];
	char startDate[9];
	char startTime[9];
	int numberOfBytesInHeader;
	char reserved[45];
	int numberOfDataRecords;
	int durationOfDataRecord;
	int numberOfSignals;

	//Max 10 signals
	char labels[10][16];

	char transducers[10][80];
	char physicalDimensions[10][8];
	double physicalMinimums[10][8];
	double physicalMaximums[10][8];
	int digitalMinimums[10][8];
	int digitalMaximums[10][8];
	char prefiltering[10][80];
	int numberOfSamplesInDataRecord[10];
	char signalsReserved[10][32];

} EDFHeader;

void edf_readAscii
    (FILE * pFile, size_t length, char * ptr_charBuffOut);

void edf_readMultipleAscii
    (FILE * pFile, size_t numBytes, char * ptr_stringArrayOut, int numberOfItems);

void edf_readInt
    (FILE * pFile, size_t length, int * ptr_intOut);

void edf_readMultipleInt
    (FILE * pFile, size_t numBytes, int * ptr_intArrayOut, int numberOfItems);

void edf_readDouble
    (FILE * pFile, size_t numBytes, double * prt_doubleOut);

void edf_readMultipleDouble
    (FILE * pFile, size_t numBytes, double * ptr_doubleArrayOut, int numberOfItems);

void edf_printHeader
    ();

void edf_stringsToAscii
    (char * ptr_strArray, size_t itemSize, int numberOfItems);

void edf_doublesToAscii
    (double * ptr_doubleArray, size_t itemSize, int numberOfItems);

void edf_intsToAscii
    (int * ptr_intArray, size_t itemSize, int numberOfItems);

#endif