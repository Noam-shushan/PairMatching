using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Bar
    {
        public string BarName { set; get; }

        public int Value { set; get; }
    }

    public class Statistics
    {
        public Bar NumberOfStudents { get; set; } = new Bar { BarName = "מספר המשתתפים הכולל" };
        public Bar NumberOfStudentsWithoutPair { get; set; } = new Bar { BarName = "מספר המשתתפים מישראל ללא חברותא" };
        public Bar NumberOfPair { get; set; } = new Bar { BarName = "מספר החברותות" };
        public Bar NumberOfStudentFromIsrael { get; set; } = new Bar { BarName = "מספר המשתתפים מישראל" };
        public Bar NumberOfStudentFromWorld { get; set; } = new Bar { BarName = "מספר המשתתפים מהתפוצות" };
        public Bar NumberOfStudentFromWorldWithoutPair { get; set; } = new Bar { BarName = "מספר המשתתפים מהתפוצות ללא חברותא" };
        public Bar NumberOfStudentFromIsraelWithoutPair { get; set; } = new Bar { BarName = "מספר המשתתפים מישראל ללא חברותא" };
        public Bar NumberOfActivePairs { get; set; } = new Bar { BarName = "חברותות פעילות" };
    }
}
