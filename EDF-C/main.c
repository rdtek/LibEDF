#include <stdio.h>
#include <stdlib.h>
#include <tchar.h>
#include <math.h>

#include "logging.h"

void ReadAscii(FILE * pFile, size_t length, char * ptr_charBuffOut);
void ReadMultipleAscii(FILE * pFile, size_t numBytes, char * ptr_stringArrayOut, int numberOfItems);
void ReadInt(FILE * pFile, size_t length, int * ptr_intOut);
void ReadMultipleInt(FILE * pFile, size_t numBytes, int * ptr_intArrayOut, int numberOfItems);
void ReadDouble(FILE * pFile, size_t numBytes, double * prt_doubleOut);
void ReadMultipleDouble(FILE * pFile, size_t numBytes, double * ptr_doubleArrayOut, int numberOfItems);

int main(void){

    printf("Reading EDF file header...");
    
    char version[9]            = { 0 };
    char patientID[81]         = { 0 };
    char recordID[81]          = { 0 };
    char startDate[9]          = { 0 };
    char startTime[9]          = { 0 };
    int numberOfBytesInHeader;
    char reserved[45]          = { 0 };
    int numberOfDataRecords;
    int durationOfDataRecord;
    int numberOfSignals;

    //Max 10 signals
    char labels[10][16];

    char transducers                    [10][80];  
    char physicalDimensions             [10][8];
    double physicalMinimums             [10][8];
    double physicalMaximums             [10][8];
    int digitalMinimums                 [10][8];
    int digitalMaximums                 [10][8];
    char prefiltering                   [10][80];
    int numberOfSamplesInDataRecord     [10];
    char signalsReserved                [10][32];

    FILE *pFile = fopen("C:\\temp\\ecg\\snap64.edf", "r");
    if (pFile == NULL) {
        printf("\nError opening file.");
        return 2;
    } 
    
    //------ Fixed length header part --------
    ReadAscii  (pFile, 8,  version);        
    ReadAscii  (pFile, 80, patientID);    
    ReadAscii  (pFile, 80, recordID);     
    ReadAscii  (pFile, 8,  startDate);     
    ReadAscii  (pFile, 8,  startTime);     
    ReadInt    (pFile, 8,  &numberOfBytesInHeader);     
    ReadAscii  (pFile, 44, reserved);             
    ReadInt    (pFile, 8,  &numberOfDataRecords);     
    ReadInt    (pFile, 8,  &durationOfDataRecord);     
    ReadInt    (pFile, 4,  &numberOfSignals);         

    //------ Variable length header part -------

    int ns = numberOfSignals;
    if (ns <= 0) return 2; //No signal data to read

    ReadMultipleAscii   (pFile, 16,  labels,                       ns);
    ReadMultipleAscii   (pFile, 80,  transducers,                  ns);
    ReadMultipleAscii   (pFile, 8,   physicalDimensions,           ns);
    ReadMultipleDouble  (pFile, 8,   physicalMinimums,             ns);
    ReadMultipleDouble  (pFile, 8,   physicalMaximums,             ns);
    ReadMultipleInt     (pFile, 8,   digitalMinimums,              ns);
    ReadMultipleInt     (pFile, 8,   digitalMaximums,              ns);
    ReadMultipleAscii   (pFile, 80,  prefiltering,                 ns);
    ReadMultipleInt     (pFile, 8,   numberOfSamplesInDataRecord,  ns);
    ReadMultipleAscii   (pFile, 32,  signalsReserved,              ns);

    //TODO: read signal sample values into arrays if necessary.

    return 0;
}

void ReadAscii(FILE * pFile, size_t numBytes, char * ptr_charBuffOut){
    int numBytesRead = 0;
    char readBuff[201] = { 0 };
    if (numBytes > 200) numBytes = 200;
    numBytesRead = fread (readBuff, 1, numBytes, pFile);
    strncpy(ptr_charBuffOut, readBuff, numBytes);
    printf("\n# bytes: %d \t", numBytesRead);
    printf("[%.*s]", numBytes, ptr_charBuffOut);
}

void ReadMultipleAscii(FILE * pFile, size_t numBytes, char * ptr_stringArrayOut, int numberOfItems)
{
    char * ptr_nextString = ptr_stringArrayOut;

    for (int i = 0; i < numberOfItems; i++) {
        ReadAscii(pFile, numBytes, ptr_nextString);
        if (i >= numberOfItems - 1) break;
        ptr_nextString += numBytes;
    }
}

void ReadDouble(FILE * pFile, size_t numBytes, double * prt_doubleOut) {
    char strNumber[10] = { 0 };
    ReadAscii(pFile, numBytes, strNumber);
    *prt_doubleOut = strtod(strNumber, NULL);
}

void ReadMultipleDouble(FILE * pFile, size_t numBytes, double * ptr_doubleArrayOut, int numberOfItems)
{
    double * ptr_nextDouble = ptr_doubleArrayOut;

    for (int i = 0; i < numberOfItems; i++) {
        ReadDouble(pFile, numBytes, ptr_nextDouble);
        if (i >= numberOfItems - 1) break;
        ptr_nextDouble++;
    }
}

void ReadInt(FILE * pFile, size_t numBytes, int * ptr_intOut) {
    char strNumber[10] = {0};
    ReadAscii(pFile, numBytes, strNumber);
    *ptr_intOut = (int)strtol(strNumber, NULL, 10);
}

void ReadMultipleInt(FILE * pFile, size_t numBytes, int * ptr_intArrayOut, int numberOfItems)
{
    int * ptr_nextInt = ptr_intArrayOut;

    for (int i = 0; i < numberOfItems; i++) {
        ReadInt(pFile, numBytes, ptr_nextInt);
        if (i >= numberOfItems - 1) break;
        ptr_nextInt++;
    }
}
