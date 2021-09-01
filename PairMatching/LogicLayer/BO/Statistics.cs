using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Statistics
    {
        public int NumberOfStudents { get; set; }
        public int NumberOfStudentsWithoutPair { get; set; }
        public int NumberOfPair { get; set; }
        public int NumberOfStudentFromIsrael { get; set; }
        public int NumberOfStudentFromWorld { get; set; }
        public int NumberOfStudentFromWorldWithoutPair { get; set; }
        public int NumberOfStudentFromIsraelWithoutPair { get; set; }
    }
}
