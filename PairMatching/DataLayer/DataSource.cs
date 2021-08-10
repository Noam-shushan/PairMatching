using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public static class DataSource
    {
        public static List<DO.Student> StudentsList { get; } = new List<DO.Student>();

        public static List<DO.LearningTime> LearningTimesList { get; } = new List<DO.LearningTime>();

        public static List<DO.OpenQuestion> OpenQuestionsList { get; } = new List<DO.OpenQuestion>();

        public static void ClearLists()
        {
            StudentsList.Clear();
            LearningTimesList.Clear();
            OpenQuestionsList.Clear();
        }
    }
}
