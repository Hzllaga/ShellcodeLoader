		static int get_current_time()
        {
            DateTime dt = DateTime.Now;
            return dt.Minute;
        }

        static bool check_sleep_acceleration()
        {
            int first = get_current_time();
            Thread.Sleep(120000);
            int second = get_current_time();
            if (second - first != 2)
            {
                return true;
            }
            return false;
        }