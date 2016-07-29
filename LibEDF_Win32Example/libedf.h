//Need to include this header file to call functions from the imported DLL
#pragma once
#include "definitions.h";
#include <Windows.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>

// Specify exported functions for DLL
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
			  // used by C++ source code
#endif
	DLL_FUNCTION void edf_printLibVersion();
	DLL_FUNCTION int edf_readEDF(const char* edfFileName, bool readSignalSamples);
	DLL_FUNCTION void edf_printHeader();
#ifdef __cplusplus
}
#endif