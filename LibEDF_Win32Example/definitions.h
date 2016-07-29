//Need to include this header file to call functions from the imported DLL
#pragma once
#define BUILDING_DLL
#ifdef BUILDING_DLL
#define DLL_FUNCTION __declspec(dllexport)
#else
#define DLL_FUNCTION __declspec(dllimport)
#endif