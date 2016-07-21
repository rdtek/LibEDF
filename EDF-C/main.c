#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <math.h>

#include "logging.h"

void ReadAscii(FILE * pFile, size_t length, char * charBuff);

int main(void){

	printf("Reading EDF file...");
	
	char version[9] = {0};
	char patientID[81] = {0};
	char recordID[81] = {0};
	char startDate[9] = {0};
	char startTime[9] = {0};
	
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
	ReadAscii(pFile, 8,   startDate); 	//StartTime
	//ReadInt16(8); 	//NumberOfBytesInHeader
	//ReadAscii(44); 	//Reserved
	//ReadInt16(8); 	//NumberOfDataRecords
	//ReadInt16(8); 	//DurationOfDataRecord
	//ReadInt16(4); 	//NumberOfSignals
}

void ReadAscii(FILE * pFile, size_t length, char * charBuff){
	int readResult = 0;
	//unsigned char buf[100] = {0};
	readResult = fread (&charBuff, 1, length, pFile);
	printf("\n# bytes: %d \t", readResult);
	printf("%.*s", length, charBuff);
}