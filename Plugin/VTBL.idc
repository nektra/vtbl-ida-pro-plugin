#include <idc.idc>

static main() 
{
	auto strExec = sprintf("%s", "FULL_PATH_OF_THE_COMPILED_DeviareTest.exe");
	Message("\nAnalize: %s", strExec);
	Exec(strExec);

	auto hCrossRef = fopen("FULL_PATH_OF_THE_BIN_DIRECTORY_CrossRef.dat", "rb");
	if(hCrossRef==0) Message("\nCrossRef.dat fopen failed!, modify the path");

	auto n;
	auto flength = filelength(hCrossRef);
		
	for(n=0; n<flength/4; n=n+2)
	{
		auto from = readlong(hCrossRef, 0);
		auto to = readlong(hCrossRef, 0);

		from = from + MinEA();
		to = to + MinEA();
		
		AddCodeXref(from, to, XREF_USER|fl_F);
		AddCodeXref(to, from, XREF_USER|fl_F);
		
		Message("\nFROM: %x    -    TO: %x", from, to);
	}
	
	fclose(hCrossRef);
	Message("\nDone!");
	
}
