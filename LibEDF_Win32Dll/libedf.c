#include "libedf.h"

static EDFHeader edfHead;

DLL_FUNCTION void edf_printLibVersion() {
	printf("LibEDF Version 1");
}

DLL_FUNCTION int edf_readEDF(const char * edfFileName, bool readSignalSamples) {

	int readResult = 0;
	printf("\n%s", edfFileName);

	FILE *ptr_file = fopen(edfFileName, "r");
	if (ptr_file == NULL) {
		printf("\nError opening EDF file.");
		return 2;
	}
	printf("\n\nReading EDF file header...\n");

	//------ Fixed length header part --------
	edf_readAscii(ptr_file, 8, edfHead.version);
	edf_readAscii(ptr_file, 80, edfHead.patientID);
	edf_readAscii(ptr_file, 80, edfHead.recordID);
	edf_readAscii(ptr_file, 8, edfHead.startDate);
	edf_readAscii(ptr_file, 8, edfHead.startTime);
	edf_readInt(ptr_file, 8, &(edfHead.numberOfBytesInHeader));
	edf_readAscii(ptr_file, 44, edfHead.reserved);
	edf_readInt(ptr_file, 8, &(edfHead.numberOfDataRecords));
	edf_readInt(ptr_file, 8, &(edfHead.durationOfDataRecord));
	edf_readInt(ptr_file, 4, &(edfHead.numberOfSignals));

	//------ Variable length header part -------

	int ns = edfHead.numberOfSignals;
	if (ns <= 0) return 2; //No signal data to read

	edf_readMultipleAscii(ptr_file, 16, edfHead.labels, ns);
	edf_readMultipleAscii(ptr_file, 80, edfHead.transducers, ns);
	edf_readMultipleAscii(ptr_file, 8, edfHead.physicalDimensions, ns);
	edf_readMultipleDouble(ptr_file, 8, edfHead.physicalMinimums, ns);
	edf_readMultipleDouble(ptr_file, 8, edfHead.physicalMaximums, ns);
	edf_readMultipleInt(ptr_file, 8, edfHead.digitalMinimums, ns);
	edf_readMultipleInt(ptr_file, 8, edfHead.digitalMaximums, ns);
	edf_readMultipleAscii(ptr_file, 80, edfHead.prefiltering, ns);
	edf_readMultipleInt(ptr_file, 8, edfHead.numberOfSamplesInDataRecord, ns);
	edf_readMultipleAscii(ptr_file, 32, edfHead.signalsReserved, ns);

	//Read signal sample values into arrays if necessary.
	if (readSignalSamples) {
		EDFSignal edfSignals[10]; //Max 10 signals
		edf_readSignals(ptr_file, edfSignals, ns, edfHead.numberOfSamplesInDataRecord[0]);
	}

	//Everything OK
	readResult = 1;

	return readResult;
}

DLL_FUNCTION void edf_printHeader() {

	EDFHeader h = edfHead;

	printf("\n==== Begin header ====");

	edf_stringsToAscii(h.version, 8, 1);
	edf_stringsToAscii(h.patientID, 80, 1);
	edf_stringsToAscii(h.recordID, 80, 1);
	edf_stringsToAscii(h.startDate, 8, 1);
	edf_stringsToAscii(h.startTime, 8, 1);
	edf_intsToAscii(&(h.numberOfBytesInHeader), 8, 1);
	edf_stringsToAscii(h.reserved, 44, 1);
	edf_intsToAscii(&(h.numberOfDataRecords), 8, 1);
	edf_intsToAscii(&(h.durationOfDataRecord), 8, 1);
	edf_intsToAscii(&(h.numberOfSignals), 4, 1);

	int ns = h.numberOfSignals;
	if (ns <= 0) return;

	edf_stringsToAscii(h.labels, 16, ns);
	edf_stringsToAscii(h.transducers, 80, ns);
	edf_stringsToAscii(h.physicalDimensions, 8, ns);
	edf_doublesToAscii(h.physicalMinimums, 8, ns);
	edf_doublesToAscii(h.physicalMaximums, 8, ns);
	edf_intsToAscii(h.digitalMinimums, 8, ns);
	edf_intsToAscii(h.digitalMaximums, 8, ns);
	edf_stringsToAscii(h.prefiltering, 80, ns);
	edf_intsToAscii(h.numberOfSamplesInDataRecord, 8, ns);
	edf_stringsToAscii(h.signalsReserved, 32, ns);

	printf("\n==== End header ====");

	return;
}

void edf_readSignals(FILE* ptr_file, EDFSignal* ptr_edfSignalsOut, int numberOfSignals, int numberOfSamples) {

	//In EDF all signal 1 samples, followed by all signal 2 samples, and so on.
	for (int iSig = 0; iSig < numberOfSignals; iSig++) {
		//printf("\nSignal : %d \t", (iSig + 1));
		for (int iSamp = 0; iSamp < numberOfSamples; iSamp++) {
			//Each sample is a 16 bit (short) integer so read 2 bytes
			short shortVal;
			int numBytesRead = 0;
			numBytesRead = fread(&shortVal, sizeof(shortVal), 1, ptr_file);
			//TODO: copy shortVal to EDFSignal.digitalSamples array.
			//printf("\n# shortVal: %d \t", shortVal);
		}
	}
}

void edf_readAscii(FILE* pFile, size_t numBytes, char* ptr_charBuffOut) {
	int numBytesRead = 0;
	char readBuff[201] = { 0 };
	if (numBytes > 200) numBytes = 200;
	numBytesRead = fread(readBuff, 1, numBytes, pFile);
	strncpy(ptr_charBuffOut, readBuff, numBytes + 1);
	//printf("\n# bytes: %d \t", numBytesRead);
	//printf("[%.*s]", numBytes, ptr_charBuffOut);
}

void edf_readMultipleAscii(FILE* pFile, size_t numBytes, char* ptr_stringArrayOut, int numberOfItems)
{
	char * ptr_nextString = ptr_stringArrayOut;

	for (int i = 0; i < numberOfItems; i++) {
		edf_readAscii(pFile, numBytes, ptr_nextString);
		if (i >= numberOfItems - 1) break;
		ptr_nextString += numBytes;
	}
}

void edf_readDouble(FILE* pFile, size_t numBytes, double* ptr_doubleOut) {
	char strNumber[10] = { 0 };
	edf_readAscii(pFile, numBytes, strNumber);
	*ptr_doubleOut = strtod(strNumber, NULL);
	return;
}

void edf_readMultipleDouble(FILE* pFile, size_t numBytes, double* ptr_doubleArrayOut, int numberOfItems)
{
	double * ptr_nextDouble = ptr_doubleArrayOut;

	for (int i = 0; i < numberOfItems; i++) {
		edf_readDouble(pFile, numBytes, ptr_nextDouble);
		if (i >= numberOfItems - 1) break;
		ptr_nextDouble++;
	}
}

void edf_readInt(FILE* pFile, size_t numBytes, int* ptr_intOut) {
	char strNumber[10] = { 0 };
	edf_readAscii(pFile, numBytes, strNumber);
	*ptr_intOut = (int)strtol(strNumber, NULL, 10);
}

void edf_readMultipleInt(FILE* pFile, size_t numBytes, int* ptr_intArrayOut, int numberOfItems)
{
	int * ptr_nextInt = ptr_intArrayOut;

	for (int i = 0; i < numberOfItems; i++) {
		edf_readInt(pFile, numBytes, ptr_nextInt);
		if (i >= numberOfItems - 1) break;
		ptr_nextInt++;
	}
}

void edf_stringsToAscii(char* ptr_strArray, size_t itemSize, int numberOfItems) {

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

void edf_doublesToAscii(double* ptr_doubleArray, size_t itemSize, int numberOfItems) {

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

void edf_intsToAscii(int* ptr_intArray, size_t itemSize, int numberOfItems) {

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