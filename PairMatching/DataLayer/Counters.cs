using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Counters
    {
        static int _studentCounter = 0;

        public static Counters Instance { get; } = new Counters();

        Counters() 
        {
            
        }

        public int StudentCounter
        {
            get => _studentCounter;
            set => _studentCounter = value;
        }

        public void IncStudentCounter()
        {
            ++_studentCounter;
        }
    }
}
