#include "edf.h"

static EDFHeader edfHead;

void edf_readEDF(const char * edfFileName) {

	int readResult = 0;
	printf("\n%s", edfFileName);

	FILE *pFile = fopen(edfFileName, "r");
	if (pFile == NULL) {
		printf("\nError opening EDF file.");
		return 2;
	}
	printf("\n\nReading EDF file header...\n");

	//------ Fixed length header part --------
	edf_readAscii  ( pFile, 8, edfHead.version );
	edf_readAscii  ( pFile, 80, edfHead.patientID );
	edf_readAscii  ( pFile, 80, edfHead.recordID );
	edf_readAscii  ( pFile, 8, edfHead.startDate );
	edf_readAscii  ( pFile, 8, edfHead.startTime );
	edf_readInt    ( pFile, 8, &(edfHead.numberOfBytesInHeader) );
	edf_readAscii  ( pFile, 44, edfHead.reserved );
	edf_readInt    ( pFile, 8, &(edfHead.numberOfDataRecords) );
	edf_readInt    ( pFile, 8, &(edfHead.durationOfDataRecord) );
	edf_readInt    ( pFile, 4, &(edfHead.numberOfSignals) );

	//------ Variable length header part -------

	int ns = edfHead.numberOfSignals;
	if (ns <= 0) return 2; //No signal data to read

	edf_readMultipleAscii  ( pFile, 16, edfHead.labels, ns );
	edf_readMultipleAscii  ( pFile, 80, edfHead.transducers, ns );
	edf_readMultipleAscii  ( pFile, 8, edfHead.physicalDimensions, ns );
	edf_readMultipleDouble ( pFile, 8, edfHead.physicalMinimums, ns );
	edf_readMultipleDouble ( pFile, 8, edfHead.physicalMaximums, ns );
	edf_readMultipleInt    ( pFile, 8, edfHead.digitalMinimums, ns );
	edf_readMultipleInt    ( pFile, 8, edfHead.digitalMaximums, ns );
	edf_readMultipleAscii  ( pFile, 80, edfHead.prefiltering, ns );
	edf_readMultipleInt    ( pFile, 8, edfHead.numberOfSamplesInDataRecord, ns );
	edf_readMultipleAscii  ( pFile, 32, edfHead.signalsReserved, ns );

	readResult = 1; //Everything OK

	//TODO: read signal sample values into arrays if necessary.

	return readResult;
}

void edf_printHeader() {
	
	EDFHeader h = edfHead;

	printf("\n==== Begin header ====");

	edf_stringsToAscii ( h.version, 8, 1 );
	edf_stringsToAscii ( h.patientID, 80, 1 );
	edf_stringsToAscii ( h.recordID, 80, 1 );
	edf_stringsToAscii ( h.startDate, 8, 1 );
	edf_stringsToAscii ( h.startTime, 8, 1 );
	edf_intsToAscii    ( &(h.numberOfBytesInHeader), 8, 1 );
	edf_stringsToAscii ( h.reserved, 44, 1);
	edf_intsToAscii    ( &(h.numberOfDataRecords), 8, 1 );
	edf_intsToAscii    ( &(h.durationOfDataRecord), 8, 1 );
	edf_intsToAscii    ( &(h.numberOfSignals), 4, 1 );

	int ns = h.numberOfSignals;
	if (ns <= 0) return;
	
	edf_stringsToAscii  ( h.labels, 16, ns );
	edf_stringsToAscii  ( h.transducers, 80, ns );
	edf_stringsToAscii  ( h.physicalDimensions, 8, ns );
	edf_doublesToAscii  ( h.physicalMinimums, 8, ns );
	edf_doublesToAscii  ( h.physicalMaximums, 8, ns );
	edf_intsToAscii     ( h.digitalMinimums, 8, ns );
	edf_intsToAscii     ( h.digitalMaximums, 8, ns );
	edf_stringsToAscii  ( h.prefiltering, 80, ns );
	edf_intsToAscii     ( h.numberOfSamplesInDataRecord, 8, ns );
	edf_stringsToAscii  ( h.signalsReserved, 32, ns );

	printf("\n==== End header ====");

	return;
}

void edf_stringsToAscii(char * ptr_strArray, size_t itemSize, int numberOfItems) {
	
	char * ptr_string = ptr_strArray;

	printf("\n[");
	for (size_t i = 0; i < numberOfItems; i++)
	{
		for (size_t i = 0; i < itemSize; i++) {
			printf("%c", ptr_string[i]);
		}
		if (i >= numberOfItems - 1) break;
		ptr_string += itemSize;
	}
	printf("]");
}

void edf_doublesToAscii(double * ptr_doubleArray, size_t itemSize, int numberOfItems) {

	double * ptr_nextDouble = ptr_doubleArray;

	printf("\n[");
	for (size_t i = 0; i < numberOfItems; i++) {
		char strDouble[50];
		sprintf(strDouble, "%f", ptr_nextDouble[i]);
		//Print string padded to itemSize
		printf("%.*s", itemSize, strDouble);
		if (i >= numberOfItems - 1) break;
	}
	printf("]");
}

void edf_intsToAscii(int * ptr_intArray, size_t itemSize, int numberOfItems) {

	int * ptr_nextInt = ptr_intArray;

	printf("\n[");
	for (size_t i = 0; i < numberOfItems; i++) {
		char strInt[50];
		sprintf(strInt, "%d", ptr_nextInt[i]);
		//Print string padded to itemSize
		printf("%-*s", itemSize, strInt);
		if (i >= numberOfItems - 1) break;
	}
	printf("]");
}

void edf_readAscii(FILE * pFile, size_t numBytes, char * ptr_charBuffOut) {
	int numBytesRead = 0;
	char readBuff[201] = { 0 };
	if (numBytes > 200) numBytes = 200;
	numBytesRead = fread(readBuff, 1, numBytes, pFile);
	strncpy(ptr_charBuffOut, readBuff, numBytes + 1);
	//printf("\n# bytes: %d \t", numBytesRead);
	//printf("[%.*s]", numBytes, ptr_charBuffOut);
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

void edf_readDouble(FILE * pFile, size_t numBytes, double * ptr_doubleOut) {
	char strNumber[10] = { 0 };
	edf_readAscii(pFile, numBytes, strNumber);
	*ptr_doubleOut = strtod(strNumber, NULL);
	return;
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