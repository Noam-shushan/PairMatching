using System;
using System.Collections.Generic;

namespace DataLayer
{
    public static class DataSource
    {
        public static List<DO.Student> StudentsList { get; } = new List<DO.Student>();

        public static void ClearLists()
        {
            StudentsList.Clear();
        }
    }
}
