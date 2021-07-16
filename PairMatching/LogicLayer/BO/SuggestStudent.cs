using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class SuggestStudent : IEquatable<SuggestStudent>
    {
        public int ThisStudentId { get; set; }
        
        public int SuggestStudentId { get; set; }
        
        public string SuggestStudentName { get; set; }
        
        public string SuggestStudenCountry { get; set; }
        
        public IEnumerable<DO.LearningTime> MatchingLearningTime { get; set; }

        public bool Equals(SuggestStudent other)
        {
            //Check whether the compared object is null.
            if (ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other)) return true;

            return ThisStudentId == other.ThisStudentId
                && SuggestStudentId == other.SuggestStudentId;
        }

        public override string ToString()
        {
            return SuggestStudentName + "\n" +
                SuggestStudenCountry + "\n" +
                "שעות מקבילות:\n" +
                string.Join("\n", from l in MatchingLearningTime
                                  let day = Dictionaries.DaysDict[l.Day] + " : "
                                  let time = string.Join(", ", from t in l.TimeInDay
                                                               select Dictionaries.TimesInDayDict[t])
                                  select day + time);
        }

        public override bool Equals(object obj) => Equals(obj as SuggestStudent);

        public override int GetHashCode() => (ThisStudentId, SuggestStudentId).GetHashCode();
    }
}
