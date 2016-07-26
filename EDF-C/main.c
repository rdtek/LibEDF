#include <stdlib.h>
#include "edf.h"

int main(void){

	edf_readEDF("C:\\temp\\ecg\\snap64.edf");
	
	printf("\nPress Any Key to Continue\n");
	getchar();

    return 0;
}