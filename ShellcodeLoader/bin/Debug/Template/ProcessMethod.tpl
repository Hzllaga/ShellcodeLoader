        static bool check_process_running()
        {
            string[] blacklist = { "vmsrvc", "tcpview", "wireshark", "visual basic", "fiddler", "vmware", "vbox", "process explorer", "autoit", "vboxtray", "vmtools", "vmrawdsk", "vmusbmouse", "vmvss", "vmscsi", "vmxnet", "vmx_svga", "vmmemctl", "df5serv", "vboxservice", "vmhgfs", "vmtoolsd" };
            string process = execute_process("tasklist.exe", "/svc");
            foreach (var name in blacklist)
            {
                if (process.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }