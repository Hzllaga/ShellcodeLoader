        public static long GetHardDiskSpace()
        {
            string diskName = execute_process("cmd.exe", "/c echo %WINDIR%").Split('W')[0];
            long totalSize = new long();
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.Name == diskName)
                {
                    totalSize = drive.TotalSize;
                }
            }
            return totalSize / (1024 * 1024 * 1024);
        }