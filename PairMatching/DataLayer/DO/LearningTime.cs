using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class LearningTime : IEquatable<LearningTime>
    {
        public int Id { get; set; }
        public IEnumerable<TimesInDay> TimeInDay { get; set; }
        public Days Day { get; set; }

        public bool Equals(LearningTime other)
        {
            //Check whether the compared object is null.
            if (ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (ReferenceEquals(this, other)) return true;
            
            return Id == other.Id
                && Day == other.Day
                && !TimeInDay.Except(other.TimeInDay).Any() 
                && !other.TimeInDay.Except(TimeInDay).Any();
        }

        public override bool Equals(object obj) => Equals(obj as LearningTime);

        public override int GetHashCode() => base.GetHashCode();
    }
}
