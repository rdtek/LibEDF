#include <stdlib.h>
#include "libedf.h"

int main(void){

	edf_printLibVersion();

	int readResult = edf_readEDF("C:\\temp\\test1.edf", true);
	
	if(readResult == 1) edf_printHeader();

	printf("\n\nPress Any Key to Continue\n");
	getchar();

    return 0;
}