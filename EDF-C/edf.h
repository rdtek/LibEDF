#ifndef HEADER_EDF
#define HEADER_EDF

#include <stdio.h>

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

#endif