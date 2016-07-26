#include "edf.h"

void edf_readEDF(const char * edfFileName) {
	printf("Reading EDF file header...");

	char version[9] = { 0 };
	char patientID[81] = { 0 };
	char recordID[81] = { 0 };
	char startDate[9] = { 0 };
	char startTime[9] = { 0 };
	int numberOfBytesInHeader;
	char reserved[45] = { 0 };
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

	FILE *pFile = fopen(edfFileName, "r");
	if (pFile == NULL) {
		printf("\nError opening file.");
		return 2;
	}

	//------ Fixed length header part --------
	edf_readAscii(pFile, 8, version);
	edf_readAscii(pFile, 80, patientID);
	edf_readAscii(pFile, 80, recordID);
	edf_readAscii(pFile, 8, startDate);
	edf_readAscii(pFile, 8, startTime);
	edf_readInt(pFile, 8, &numberOfBytesInHeader);
	edf_readAscii(pFile, 44, reserved);
	edf_readInt(pFile, 8, &numberOfDataRecords);
	edf_readInt(pFile, 8, &durationOfDataRecord);
	edf_readInt(pFile, 4, &numberOfSignals);

	//------ Variable length header part -------

	int ns = numberOfSignals;
	if (ns <= 0) return 2; //No signal data to read

	edf_readMultipleAscii(pFile, 16, labels, ns);
	edf_readMultipleAscii(pFile, 80, transducers, ns);
	edf_readMultipleAscii(pFile, 8, physicalDimensions, ns);
	edf_readMultipleDouble(pFile, 8, physicalMinimums, ns);
	edf_readMultipleDouble(pFile, 8, physicalMaximums, ns);
	edf_readMultipleInt(pFile, 8, digitalMinimums, ns);
	edf_readMultipleInt(pFile, 8, digitalMaximums, ns);
	edf_readMultipleAscii(pFile, 80, prefiltering, ns);
	edf_readMultipleInt(pFile, 8, numberOfSamplesInDataRecord, ns);
	edf_readMultipleAscii(pFile, 32, signalsReserved, ns);

	//TODO: read signal sample values into arrays if necessary.
}

void edf_readAscii(FILE * pFile, size_t numBytes, char * ptr_charBuffOut) {
	int numBytesRead = 0;
	char readBuff[201] = { 0 };
	if (numBytes > 200) numBytes = 200;
	numBytesRead = fread(readBuff, 1, numBytes, pFile);
	strncpy(ptr_charBuffOut, readBuff, numBytes);
	printf("\n# bytes: %d \t", numBytesRead);
	printf("[%.*s]", numBytes, ptr_charBuffOut);
}

void edf_readMultipleAscii(FILE * pFile, size_t numBytes, char * ptr_stringArrayOut, int numberOfItems)
{
	char * ptr_nextString = ptr_stringArrayOut;

	for (int i = 0; i < numberOfItems; i++) {
		edf_readAscii(pFile, numBytes, ptr_nextString);
		if (i >= numberOfItems - 1) break;
		ptr_nextString += numBytes;
	}
}

void edf_readDouble(FILE * pFile, size_t numBytes, double * prt_doubleOut) {
	char strNumber[10] = { 0 };
	edf_readAscii(pFile, numBytes, strNumber);
	*prt_doubleOut = strtod(strNumber, NULL);
}

void edf_readMultipleDouble(FILE * pFile, size_t numBytes, double * ptr_doubleArrayOut, int numberOfItems)
{
	double * ptr_nextDouble = ptr_doubleArrayOut;

	for (int i = 0; i < numberOfItems; i++) {
		edf_readDouble(pFile, numBytes, ptr_nextDouble);
		if (i >= numberOfItems - 1) break;
		ptr_nextDouble++;
	}
}

void edf_readInt(FILE * pFile, size_t numBytes, int * ptr_intOut) {
	char strNumber[10] = { 0 };
	edf_readAscii(pFile, numBytes, strNumber);
	*ptr_intOut = (int)strtol(strNumber, NULL, 10);
}

void edf_readMultipleInt(FILE * pFile, size_t numBytes, int * ptr_intArrayOut, int numberOfItems)
{
	int * ptr_nextInt = ptr_intArrayOut;

	for (int i = 0; i < numberOfItems; i++) {
		edf_readInt(pFile, numBytes, ptr_nextInt);
		if (i >= numberOfItems - 1) break;
		ptr_nextInt++;
	}
}