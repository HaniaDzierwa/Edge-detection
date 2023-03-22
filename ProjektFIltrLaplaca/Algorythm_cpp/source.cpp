/*
* Hanna Dzierwa 
* AEI Inf gr1
* Filtr Laplace'a in c++
*/

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <iostream>
#include<chrono>

extern "C" {

	_declspec(dllexport) void doAlgorythmInCpp(BYTE* inputList, BYTE* outputList, int size, int heightMul3)
	{
		// one pass of the loop return new pixel commponent. For each pixel component below algorythm must be run
		//  
		// 0 -1 0
		// -1 4 -1
		// 0 -1 0 
		// '0' value cann be miss, so after multiplaying each coresponding pixel component and adding, new value will be created 
	
		for ( int i = 0; i < size; i++)
		{
			int newValueFromAll = inputList[i - heightMul3] * (-1)// top
				+ inputList[i - 3] * (-1) // left
				+ inputList[i] * 4 // mid
				+ inputList[i + 3] * (-1) // right
				+ inputList[i + heightMul3] * (-1); // bot

			// checking if new value pixel componnet is in range (0:255)
			if (newValueFromAll < 0)
			{
				newValueFromAll = 0;
			}

			if (newValueFromAll > 255)
			{
				newValueFromAll = 255;
			}

			// adding new value to list 
			outputList[i] = newValueFromAll;
		}	
	}
};