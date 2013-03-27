using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Nektra.Deviare2;




namespace DeviareTest
{
    public partial class Form1 : Form
    {
        [DllImport(@"FULLPATH_OF_helper.dll")]
        public static extern IntPtr SkipHook(IntPtr address, int processid);

        [DllImport(@"FULLPATH_OF_helper.dll")]
        public static extern bool isValidPreOpCode(byte[] buffer, UInt32 size);

        [DllImport(@"FULLPATH_OF_helper.dll")]
        public static extern bool isValidPostOpCode(byte[] buffer, UInt32 size);

        [DllImport(@"FULLPATH_OF_helper.dll")]
        public static extern uint GetInstrSize(byte[] buffer, UInt32 size);

        [DllImport(@"FULLPATH_OF_DeviareCOM.dll")]
        public static extern UInt32 GetLastErrorCode();

        private NktSpyMgr _spyMgr;
        private NktProcess _process;
        private UInt64 ModStartAddress;
        private UInt64 ModEndAddress;
        private UInt64 SecStartAddress;
        private UInt64 SecEndAddress;
        private object ContinueEvent;

        struct VTBL
        {
            public UInt64 Address;
            public List<UInt64> ValuesList;
        }

        struct CROSSREF
        {
            public UInt64 From;
            public UInt64 To;
        }

        private List<VTBL> VTableList = new List<VTBL>();
        private HashSet<CROSSREF> CrossRefSet = new HashSet<CROSSREF>();

        public void WorkThreadFunction()
        {
            _spyMgr = new NktSpyMgr();
            _spyMgr.LicenseKey = Properties.Resources.License;
            _spyMgr.Initialize();
            _spyMgr.OnFunctionCalled += new DNktSpyMgrEvents_OnFunctionCalledEventHandler(OnFunctionCalled);
        }


        public Form1()
        {
            InitializeComponent();

            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            thread.Join();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnHook.Enabled = false;
            btnClear.Enabled = false;
        }

        private IntPtr GetModuleBase(string moduleName)
        {
            NktModule tempModule = _process.ModuleByName(moduleName);
            return tempModule.BaseAddress;
        }

        private IntPtr GetModuleSize(string moduleName)
        {
            NktModule tempModule = _process.ModuleByName(moduleName);
            return tempModule.Size;
        }

        private bool GetProcess(string proccessName)
        {
            NktProcessesEnum enumProcess = _spyMgr.Processes();
            NktProcess tempProcess = enumProcess.First();
            while (tempProcess != null)
            {
                if (tempProcess.Name.Equals(proccessName, StringComparison.InvariantCultureIgnoreCase))
                {
                    _process = tempProcess;
                    return true;
                }
                tempProcess = enumProcess.Next();
            }

            _process = null;
            return false;
        }

        private void OnFunctionCalled(NktHook hook, NktProcess process, NktHookCallInfo hookCallInfo)
        {
            NktStackTrace stack = hookCallInfo.StackTrace();
            NktProcessMemory memory = _spyMgr.ProcessMemoryFromPID(_process.Id);

            UInt32 StackOpcodeSize = 50;
            byte[] StackOpcode = new byte[StackOpcodeSize];

            for (UInt32 n = 0; n < StackOpcodeSize; n++)
            {
                StackOpcode[n] = (byte)memory.Read((IntPtr)((UInt64)stack.Address(0) - StackOpcodeSize + n), eNktDboFundamentalType.ftUnsignedByte);
            }

            UInt64 actualAddr = (UInt64)hookCallInfo.get_Register(eNktRegister.asmRegEip);
            UInt64 nInstrSize = (UInt64)GetInstrSize(StackOpcode, StackOpcodeSize);
            UInt64 callingAddr = (UInt64)stack.Address(0) - nInstrSize;

            string str = "From: 0x" + callingAddr.ToString("x") + "    To: 0x" + actualAddr.ToString("x") + "\n";
            Output(str, false);

            actualAddr -= SecStartAddress;
            callingAddr -= SecStartAddress;
            
            CROSSREF crossref = new CROSSREF();
            crossref.From = callingAddr;
            crossref.To = actualAddr;
            CrossRefSet.Add(crossref);
        }

        public delegate void OutputDelegate(string strOutput, bool Clear);

        private void Output(string strOutput, bool Clear)
        {
            if (InvokeRequired)
                BeginInvoke(new OutputDelegate(Output), strOutput, Clear);
            else
            {
                if (Clear)
                    textOutput.Text = "";
                else
                    textOutput.AppendText(strOutput);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string[] Args = Environment.GetCommandLineArgs();

            FileStream stream;
            BinaryWriter writer;

            string strfile = Path.GetDirectoryName(Args[0]) + "\\CrossRef.dat";
            stream = File.Open(strfile, FileMode.Create);
            writer = new BinaryWriter(stream);

            for (int n = 0; n < CrossRefSet.Count; n++)
            {
                CROSSREF crossref = CrossRefSet.ElementAt(n);
                writer.Write((Int32)crossref.From);
                writer.Write((Int32)crossref.To);
            }

            writer.Flush();
            stream.Close();
        }

        private void btnHook_Click(object sender, EventArgs e)
        {
            VTBL vtbl = VTableList.ElementAt(listBoxVTBL.SelectedIndex);

            for(int a=0; a<vtbl.ValuesList.Count; a++)
            {
                NktHook hook = _spyMgr.CreateHookForAddress(_process, (IntPtr)vtbl.ValuesList.ElementAt(a), "",
                                                           (int)
                                                           (eNktHookFlags.flgOnlyPreCall |
                                                            eNktHookFlags.flgDontCheckAddress));
                hook.Hook(true);            
            }


            if (checkSuspended.Checked)
            {
                _spyMgr.ResumeProcess(_process, ContinueEvent);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Output("", true);
        }

        private void comboBoxModules_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxModules.Enabled = false;

            int selected = comboBoxModules.SelectedIndex;
            List<NktModule> ModuleList = (List<NktModule>)comboBoxModules.Tag;
            NktModule module = ModuleList.ElementAt(selected);

            NktStructPESections sections = module.Sections();
            int nSectionCode = 0;
            for (int n = 0; n < sections.Count; n++)
            {
                if (sections.get_Name(n) == ".text")
                {
                    nSectionCode = n;
                    break;
                }
            }

            SecStartAddress = (UInt64)sections.get_StartAddress(nSectionCode);
            SecEndAddress = (UInt64)sections.get_EndAddress(nSectionCode);

            ModStartAddress = (UInt64)GetModuleBase(_process.Name);
            ModEndAddress = ModStartAddress + (UInt64)GetModuleSize(_process.Name);

            NktProcessMemory memory = _spyMgr.ProcessMemoryFromPID(_process.Id);

            uint nvtable = 0;
            ulong tmpAddress = 0;
            VTBL vtbl;
            vtbl.Address = 0;
            vtbl.ValuesList = null;

            for (UInt64 CurAddress = ModStartAddress; CurAddress < ModEndAddress; CurAddress++)
            {

                progressBar.Value = (int)( CurAddress * 100 / ModEndAddress);

                UInt32 CurValue = (UInt32)memory.Read((IntPtr)CurAddress, eNktDboFundamentalType.ftUnsignedDoubleWord);

                if (CurValue >= SecStartAddress && CurValue <= SecEndAddress)
                {
                    UInt32 PreOpcodeSize = 50;
                    byte[] PreOpcode = new byte[PreOpcodeSize];
                    for (UInt32 n = 0; n < PreOpcodeSize; n++)
                    {
                        PreOpcode[n] =
                            (byte)memory.Read((IntPtr)(CurValue - PreOpcodeSize + n), eNktDboFundamentalType.ftUnsignedByte);
                    }

                    UInt32 PostOpcodeSize = 50;
                    byte[] PostOpcode = new byte[PostOpcodeSize];
                    for (UInt32 n = 0; n < PostOpcodeSize; n++)
                    {
                        PostOpcode[n] =
                            (byte)memory.Read((IntPtr)(CurValue + n), eNktDboFundamentalType.ftUnsignedByte);
                    }

                    if (isValidPreOpCode(PreOpcode, PreOpcodeSize) && isValidPostOpCode(PostOpcode, PostOpcodeSize))
                    {
                        if ((CurAddress - tmpAddress) > 500 || tmpAddress == 0) //este valor lo podemos ir adaptando, lo correcto seria (CurAddress - tmpAddress != 4)
                        {
                            vtbl = new VTBL();
                            vtbl.Address = CurAddress;
                            vtbl.ValuesList = new List<UInt64>();
                            VTableList.Add(vtbl);
                            nvtable++;
                        }

                        tmpAddress = CurAddress;

                        vtbl.ValuesList.Add((UInt64)SkipHook((IntPtr)CurValue, _process.Id));
                    }
 
                
               }    
            }

            progressBar.Value = 100;

            for (int n = 0; n < VTableList.Count; n++)
            {
                string vtblname = "VTBL_" + n.ToString("X") + "_" + VTableList.ElementAt(n).Address.ToString("X") + "_" + VTableList.ElementAt(n).ValuesList.Count;

                listBoxVTBL.Items.Add(vtblname);
            }


            btnHook.Enabled = true;
            btnClear.Enabled = true;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
           
            _process = _spyMgr.CreateProcess(openFileDialog1.FileName, checkSuspended.Checked, out ContinueEvent);
        }

        private void comboBoxModules_DropDown(object sender, EventArgs e)
        {
            if (comboBoxModules.Items.Count == 0)
            {
                List<NktModule> ModuleList = new List<NktModule>();
                comboBoxModules.Tag = ModuleList;

                NktModulesEnum modulesEnum = _process.Modules();
                NktModule tempModule = modulesEnum.First();
                while (tempModule != null)
                {
                    comboBoxModules.Items.Add(tempModule.Name);
                    ModuleList.Add(tempModule);
                    tempModule = modulesEnum.Next();
                }
            }
        }
 
    }
}
