#include "edf.h"

void edf_readEDF(const char * edfFileName) {
	printf("Reading EDF file header...");

	EDFHeader edfHead;

	FILE *pFile = fopen(edfFileName, "r");
	if (pFile == NULL) {
		printf("\nError opening file.");
		return 2;
	}

	//------ Fixed length header part --------
	edf_readAscii(pFile, 8, edfHead.version);
	edf_readAscii(pFile, 80, edfHead.patientID);
	edf_readAscii(pFile, 80, edfHead.recordID);
	edf_readAscii(pFile, 8, edfHead.startDate);
	edf_readAscii(pFile, 8, edfHead.startTime);
	edf_readInt(pFile, 8, &(edfHead.numberOfBytesInHeader));
	edf_readAscii(pFile, 44, edfHead.reserved);
	edf_readInt(pFile, 8, &(edfHead.numberOfDataRecords));
	edf_readInt(pFile, 8, &(edfHead.durationOfDataRecord));
	edf_readInt(pFile, 4, &(edfHead.numberOfSignals));

	//------ Variable length header part -------

	int ns = edfHead.numberOfSignals;
	if (ns <= 0) return 2; //No signal data to read

	edf_readMultipleAscii(pFile, 16, edfHead.labels, ns);
	edf_readMultipleAscii(pFile, 80, edfHead.transducers, ns);
	edf_readMultipleAscii(pFile, 8, edfHead.physicalDimensions, ns);
	edf_readMultipleDouble(pFile, 8, edfHead.physicalMinimums, ns);
	edf_readMultipleDouble(pFile, 8, edfHead.physicalMaximums, ns);
	edf_readMultipleInt(pFile, 8, edfHead.digitalMinimums, ns);
	edf_readMultipleInt(pFile, 8, edfHead.digitalMaximums, ns);
	edf_readMultipleAscii(pFile, 80, edfHead.prefiltering, ns);
	edf_readMultipleInt(pFile, 8, edfHead.numberOfSamplesInDataRecord, ns);
	edf_readMultipleAscii(pFile, 32, edfHead.signalsReserved, ns);

	edf_printHeader(edfHead);

	//TODO: read signal sample values into arrays if necessary.
}

void edf_printHeader(EDFHeader h) {
	
	printf("\n[%s]", h.version);
	printf("\n[%s]", h.patientID);
	printf("\n[%s]", h.recordID);
	printf("\n[%s]", h.startDate);
	printf("\n[%s]", h.startTime);
	printf("\n[%d]", h.numberOfBytesInHeader);
	printf("\n[%s]", h.reserved);
	printf("\n[%d]", h.numberOfDataRecords);
	printf("\n[%d]", h.durationOfDataRecord);
	printf("\n[%d]", h.numberOfSignals);

	int ns = h.numberOfSignals;
	if (ns <= 0) return;
	
	edf_stringArrayToAscii(h.labels, 16, ns);
	edf_stringArrayToAscii(h.transducers, 80, ns);
	edf_stringArrayToAscii(h.physicalDimensions, 8, ns);
	edf_doubleArrayToAscii(h.physicalMinimums, 8, ns);
	edf_doubleArrayToAscii(h.physicalMaximums, 8, ns);

	return;
}

void edf_stringArrayToAscii(char * ptr_strArray, size_t itemSize, int numberOfItems) {
	
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

void edf_doubleArrayToAscii(double * ptr_doubleArray, size_t itemSize, int numberOfItems) {

	double * ptr_nextDouble = ptr_doubleArray;

	printf("\n[");
	for (size_t i = 0; i < numberOfItems; i++) {
		char strDouble[50];
		sprintf(strDouble, "%f", ptr_nextDouble[i]);
		printf("%.*s", itemSize, strDouble); //TODO: set printf padding to itemSize
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

void edf_readDouble(FILE * pFile, size_t numBytes, double * prt_doubleOut) {
	char strNumber[10] = { 0 };
	edf_readAscii(pFile, numBytes, strNumber);
	*prt_doubleOut = strtod(strNumber, NULL);
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