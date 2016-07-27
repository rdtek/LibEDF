#include <stdlib.h>
#include "edf.h"

int main(void){

	int readResult = edf_readEDF("C:\\temp\\ecg\\snap64.edf");
	
	if(readResult == 1) edf_printHeader();

	printf("\n\nPress Any Key to Continue\n");
	getchar();

    return 0;
}