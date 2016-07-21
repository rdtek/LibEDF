#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <math.h>

#include "logging.h"

void ReadAscii(FILE * pFile, size_t length, char * charBuffOut);
void ReadInt(FILE * pFile, size_t length, int * intOut);

int main(void){

	printf("Reading EDF file...");
	
	char version[9]		= { 0 };
	char patientID[81]	= { 0 };
	char recordID[81]	= { 0 };
	char startDate[9]	= { 0 };
	char startTime[9]	= { 0 };
	int numberOfBytesInHeader;
	char reserved[45]	= { 0 };
	int numberOfDataRecords;
	int durationOfDataRecord;
	int numberOfSignals;
	char labels[10][16];  //Max 10 labels, each 16 ascii chars in length

	FILE *pFile = fopen("C:\\temp\\ecg\\snap64.edf", "r");
    if (pFile == NULL) {
		printf("\nError opening file.");
		return 2;
	} 
	
	//------ Fixed length header part --------
	ReadAscii(pFile, 8,   version);		//Version
	ReadAscii(pFile, 80,  patientID);	//PatientID
	ReadAscii(pFile, 80,  recordID); 	//RecordID
	ReadAscii(pFile, 8,   startDate); 	//StartDate
	ReadAscii(pFile, 8,   startTime); 	//StartTime
	ReadInt(pFile, 8, &numberOfBytesInHeader); 	//NumberOfBytesInHeader
	ReadAscii(pFile, 44, reserved); 			//Reserved
	ReadInt(pFile, 8, &numberOfDataRecords); 	//NumberOfDataRecords
	ReadInt(pFile, 8, &durationOfDataRecord); 	//DurationOfDataRecord
	ReadInt(pFile, 4, &numberOfSignals); 		//NumberOfSignals

	//------ Variable length header part -------
	for (size_t i = 0; i < numberOfSignals && i < 10; i++)
	{
		char signalLabel[16] = { 0 };
		ReadAscii(pFile, 16, signalLabel);
		strcpy(labels[i], signalLabel);
	}
}

void ReadAscii(FILE * pFile, size_t length, char * charBuffOut){
	int readResult = 0;
	readResult = fread (charBuffOut, 1, length, pFile);
	printf("\n# bytes: %d \t", readResult);
	printf("%.*s", length, charBuffOut);
}

void ReadInt(FILE * pFile, size_t length, int * intOut) {
	char strNumber[10] = {0};
	ReadAscii(pFile, length, strNumber);
	*intOut = (int)strtol(strNumber, NULL, 10);
}

