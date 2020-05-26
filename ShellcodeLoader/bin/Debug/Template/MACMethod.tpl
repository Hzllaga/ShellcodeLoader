        static bool checks_mac_address()
        {
            string[] blacklist = { "080027", "000569", "000C29", "001C14", "005056", "001C42", "00163E", "0A0027" };
            string mac = execute_process("getmac.exe").Replace("-", "").Replace(":", "");
            foreach (var name in blacklist)
            {
                if (mac.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }