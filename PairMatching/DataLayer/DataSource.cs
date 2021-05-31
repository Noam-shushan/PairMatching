using System;
using System.Collections.Generic;
using System.Text;

namespace DS
{
    public static class DataSource
    {
        public static List<DO.Student> StudentsList;
        public static List<DO.Pair> PairsList;
        
        static DataSource()
        {
            StudentsList = new List<DO.Student>();
            PairsList = new List<DO.Pair>();
        }
    }
}
