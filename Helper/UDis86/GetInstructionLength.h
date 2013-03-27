/*
 * Copyright (c) 2010-2012 Nektra S.A., Buenos Aires, Argentina.
 * All rights reserved.
 *
 **/

#ifndef _ASM_GETINSTRUCTION_LENGTH_H
#define _ASM_GETINSTRUCTION_LENGTH_H

#include <windows.h>

//-----------------------------------------------------------

SIZE_T GetInstructionLength_x86(LPVOID lpAddr, SIZE_T nSize);
SIZE_T GetInstructionLength_x64(LPVOID lpAddr, SIZE_T nSize);

//-----------------------------------------------------------

#endif //_ASM_GETINSTRUCTION_LENGTH_H
