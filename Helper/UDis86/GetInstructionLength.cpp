#include "GetInstructionLength.h"
extern "C" {
  #include "types.h"
  #include "extern.h"
  #include "itab.h"
};

//-----------------------------------------------------------

SIZE_T GetInstructionLength_x86(LPVOID lpAddr, SIZE_T nSize)
{
  ud_t ud_obj;

  ud_init(&ud_obj);
  ud_set_mode(&ud_obj, 32);
  ud_set_input_buffer(&ud_obj, (LPBYTE)lpAddr, nSize);
  ud_set_syntax(&ud_obj, NULL);
  return (SIZE_T)ud_disassemble(&ud_obj);
}

SIZE_T GetInstructionLength_x64(LPVOID lpAddr, SIZE_T nSize)
{
  ud_t ud_obj;

  ud_init(&ud_obj);
  ud_set_mode(&ud_obj, 64);
  ud_set_input_buffer(&ud_obj, (LPBYTE)lpAddr, nSize);
  ud_set_syntax(&ud_obj, NULL);
  return (SIZE_T)ud_disassemble(&ud_obj);
}
