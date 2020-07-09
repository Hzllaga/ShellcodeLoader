		private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_READWRITE = 0x04;
        private static UInt32 PAGE_EXECUTE = 0x10;
        [DllImport("kernel32")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr,
            UInt32 size, UInt32 flAllocationType, UInt32 flProtect);

        [DllImport("kernel32")]
        static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32")]
        private static extern IntPtr CreateThread(
            UInt32 lpThreadAttributes,
            UInt32 dwStackSize,
            UInt32 lpStartAddress,
            IntPtr param,
            UInt32 dwCreationFlags,
            ref UInt32 lpThreadId
        );
        [DllImport("kernel32")]
        private static extern UInt32 WaitForSingleObject(
            IntPtr hHandle,
            UInt32 dwMilliseconds
        );