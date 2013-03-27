// Helper.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "udis86.h"
#include <mbstring.h>


#define XISALIGNED(x)  ((((SIZE_T)(x)) & (sizeof(SIZE_T)-1)) == 0)

void* nktMemCopy(__out void *lpDest, __in const void *lpSrc, __in SIZE_T nCount)
{
  LPBYTE s, d;

  s = (LPBYTE)lpSrc;
  d = (LPBYTE)lpDest;
  if (XISALIGNED(s) && XISALIGNED(d))
  {
    while (nCount >= sizeof(SIZE_T))
    {
      *((SIZE_T*)d) = *((SIZE_T*)s);
      s += sizeof(SIZE_T);
      d += sizeof(SIZE_T);
      nCount -= sizeof(SIZE_T);
    }
  }
  while ((nCount--) > 0)
    *d++ = *s++;
  return lpDest;
}

static __inline ULONG HookEng_ReadUnalignedULong(__in LPVOID lpAddr)
{
  return ((ULONG)(*( (LPBYTE)lpAddr)   )      ) |
         ((ULONG)(*(((LPBYTE)lpAddr)+1)) <<  8) |
         ((ULONG)(*(((LPBYTE)lpAddr)+2)) << 16) |
         ((ULONG)(*(((LPBYTE)lpAddr)+3)) << 24);
}

HANDLE_PTR SkipHook(__in HANDLE_PTR address, __in DWORD dwProcessId)
{
	HANDLE hProcess = ::OpenProcess(PROCESS_ALL_ACCESS, FALSE, dwProcessId);
	BYTE Buffer[20];
	ReadProcessMemory(hProcess, (LPCVOID)address, Buffer, 20, NULL);
	CloseHandle(hProcess);
	HANDLE_PTR retAddress = address;

  LONG nOfs;
#if defined _M_IX86
  LPBYTE d;
#endif

  try
  {
    for (;;)
    {

      if (Buffer[0] == 0xE9)
      {
        nktMemCopy(&nOfs, Buffer+1, sizeof(LONG));
		retAddress += nOfs+5;
		break;
      }
      else if (Buffer[0] == 0xEB)
      {
        nOfs = (LONG)*((signed char*)(Buffer+1));
        retAddress += nOfs+2;
		break;
      }
/*
      else if ((Buffer[0]==0xFF && Buffer[1]==0x25) ||
               (Buffer[0]==0x48 && Buffer[1]==0xFF && Buffer[2]==0x25))
      {
        if (Buffer[0] == 0x48)
          Buffer++;
#if defined _M_IX86
        d = (LPBYTE)HookEng_ReadUnalignedULong(Buffer+2);
        Buffer = (LPBYTE)HookEng_ReadUnalignedULong(d);
#elif defined _M_X64
        nOfs = (LONG)HookEng_ReadUnalignedULong(Buffer+2);
        Buffer = (LPBYTE)HookEng_ReadUnalignedULongLong(Buffer+6+(LONGLONG)nOfs);
#else
  #error Unsupported platform
#endif
      }
*/
      else
		  break;
	  
    }
  }
  catch (...)
  { }

  return retAddress;
}

BOOL isValidPreOpCode(BYTE *buffer, UINT nsize)
{
	ud_t ud_obj;

	ud_init(&ud_obj);
	ud_set_input_buffer(&ud_obj, buffer, nsize);
	ud_set_mode(&ud_obj, 64);
	ud_set_syntax(&ud_obj, UD_SYN_INTEL);

	ud_t temp_ud_obj;

	while (ud_disassemble(&ud_obj)) 
	{
		temp_ud_obj = ud_obj;
	}

	char *str = ud_insn_asm(&temp_ud_obj);

	if(!_stricmp(str, "ret "))
		return true;
	if(!_stricmp(str, "nop "))
		return true;
	if(!_stricmp(str, "int3 "))
		return true;

	return false;
}

BOOL isValidPostOpCode(BYTE *buffer, UINT nsize)
{
	ud_t ud_obj;

	ud_init(&ud_obj);
	ud_set_input_buffer(&ud_obj, buffer, nsize);
	ud_set_mode(&ud_obj, 64);
	ud_set_syntax(&ud_obj, UD_SYN_INTEL);

	ud_disassemble(&ud_obj);
	char *str = ud_insn_asm(&ud_obj);
	char str1[10];
	_mbsnbcpy((unsigned char*)str1, (unsigned char*)strtok(str," "), 10);

	ud_disassemble(&ud_obj);
	str = ud_insn_asm(&ud_obj);
	char str2[10];
	_mbsnbcpy((unsigned char*)str2, (unsigned char*)strtok(str," "), 10);

	ud_disassemble(&ud_obj);
	str = ud_insn_asm(&ud_obj);
	char str3[10];
	_mbsnbcpy((unsigned char*)str3, (unsigned char*)strtok(str," "), 10);

	ud_disassemble(&ud_obj);
	str = ud_insn_asm(&ud_obj);
	char str4[10];
	_mbsnbcpy((unsigned char*)str4, (unsigned char*)strtok(str," "), 10);

	ud_disassemble(&ud_obj);
	str = ud_insn_asm(&ud_obj);
	char str5[10];
	_mbsnbcpy((unsigned char*)str5, (unsigned char*)strtok(str," "), 10);

	if(!_stricmp(str1, "mov") && !_stricmp(str2, "push") && !_stricmp(str3, "call") && !_stricmp(str4, "pop") && !_stricmp(str5, "ret"))
		return false;

	if(!_stricmp(str1, "mov") && !_stricmp(str2, "ret"))
		return false;

	return true;
}

UINT GetInstrSize(BYTE *buffer, UINT nsize)
{
	ud_t ud_obj;

	ud_init(&ud_obj);
	ud_set_input_buffer(&ud_obj, buffer, nsize);
	ud_set_mode(&ud_obj, 64);
	ud_set_syntax(&ud_obj, UD_SYN_INTEL);


	ud_t temp_ud_obj;

	while (ud_disassemble(&ud_obj)) 
	{
		temp_ud_obj = ud_obj;
	}

	return temp_ud_obj.pc - temp_ud_obj.insn_offset;
}