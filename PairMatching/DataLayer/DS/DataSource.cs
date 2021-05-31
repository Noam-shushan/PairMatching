using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS
{
    public static class DataSource
    {
        public static List<DO.Student> StudentsList;
        public static List<DO.Pair> PairsList;

        static DataSource()
        {
            if(StudentsList == null)
            {
                StudentsList = new List<DO.Student>();
            }
            if(PairsList == null)
            {
                PairsList = new List<DO.Pair>();
            }               
        }
    }
}
