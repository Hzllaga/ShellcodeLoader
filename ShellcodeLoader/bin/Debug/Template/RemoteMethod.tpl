		[StructLayout(LayoutKind.Sequential)]
        public class SecurityAttributes
        {
            public Int32 Length = 0;

            public SecurityAttributes()
            {
                this.Length = Marshal.SizeOf(this);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessInformation
        {
            public IntPtr hProcess;
            public IntPtr hThread;
        }

        [Flags]
        public enum CreateProcessFlags : uint
        {
            CREATE_SUSPENDED = 0x00000004,
        }

        [StructLayout(LayoutKind.Sequential)]
        public class StartupInfo
        {
            public Int32 cb = 0;
            public Int32 dwFlags = 0;

            public StartupInfo()
            {
                this.cb = Marshal.SizeOf(this);
            }
        }

        [Flags()]
        public enum AllocationType : uint
        {
            GO = 0x3000,
        }


        [Flags()]
        public enum MemoryProtection : uint
        {
            EXECUTE_READWRITE = 0x40,
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateProcessA(
                String lpApplicationName,
                String lpCommandLine,
                SecurityAttributes lpProcessAttributes,
                SecurityAttributes lpThreadAttributes,
                Boolean bInheritHandles,
                CreateProcessFlags dwCreationFlags,
                IntPtr lpEnvironment,
                String lpCurrentDirectory,
                [In] StartupInfo lpStartupInfo,
                out ProcessInformation lpProcessInformation

            );

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(
               IntPtr lpHandle,
               IntPtr lpAddress,
               IntPtr dwSize,
               AllocationType flAllocationType,
               MemoryProtection flProtect
            );

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] buffer,
            IntPtr dwSize,
            int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool TerminateProcess(
            IntPtr hProcess,
            uint uExitCode);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId);