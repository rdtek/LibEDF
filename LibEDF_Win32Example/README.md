# LibEDF Win32 Example

Small C program demonstrating import of LibEDF Dll and calling functions from the Dll.  
    
The example code reads the EDF file header and prints the header fields to the console.

```c
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
```
