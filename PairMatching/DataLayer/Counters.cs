using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class Counters
    {
        static int _studentCounter = 0;

        public static int StudentCounter
        {
            get
            {
                _studentCounter++;
                return _studentCounter;
            }

            set => _studentCounter = value;
        }
    }
}
