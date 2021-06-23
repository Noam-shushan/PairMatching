using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Matching
    {

        static Dictionary<DO.TimesInDay, TimeInterval> BoundryOfTimeInDay =
            new Dictionary<DO.TimesInDay, TimeInterval>();

        static Matching()
        {
            SetBoundrysOfTimeInDay();
        }
        
        static void SetBoundrysOfTimeInDay()
        {
            BoundryOfTimeInDay.Add(
                DO.TimesInDay.MORNING, 
                new TimeInterval
                {
                    Start = TimeSpan.Parse("5:00"),
                    End = TimeSpan.Parse("12:00")
                });
            BoundryOfTimeInDay.Add(
                DO.TimesInDay.NOON,
                new TimeInterval
                {
                    Start = TimeSpan.Parse("12:00"),
                    End = TimeSpan.Parse("18:00")
                });
            BoundryOfTimeInDay.Add(
                DO.TimesInDay.EVENING,
                new TimeInterval
                {
                    Start = TimeSpan.Parse("18:00"),
                    End = TimeSpan.Parse("21:00")
                });
            BoundryOfTimeInDay.Add(
                DO.TimesInDay.NIGHT,
                new TimeInterval
                {
                    Start = TimeSpan.Parse("21:00"),
                    End = TimeSpan.Parse("2:00")
                });
        }

        /// <summary>
        /// Cheack if tow student is a match from first degree 
        /// </summary>
        /// <param name="thisStudent"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsFirstMatching(BO.Student thisStudent, BO.Student other)
        {
            return IsMatchingStudentsNotCritical(thisStudent, other)
                && IsMatchingStudentsCritical(thisStudent, other);
        }

        public bool IsMatchingStudentsNotCritical(BO.Student isrealStudent, BO.Student other)
        {
            return isrealStudent.DesiredSkillLevel == other.SkillLevel
                && isrealStudent.EnglishLevel == other.DesiredEnglishLevel
                && isrealStudent.LearningStyle == other.LearningStyle;
        }

        /// <summary>
        /// Cheack if the students have the critical condition to be a match
        /// </summary>
        /// <param name="thisStudent"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsMatchingStudentsCritical(BO.Student isrealStudent, BO.Student other)
        {          
            return isrealStudent.PrefferdTracks == other.PrefferdTracks
                && isrealStudent.PrefferdGender == other.Gender
                && IsMatchingHours(isrealStudent, other)
                && IsMatchingDays(isrealStudent, other);
        }

        private bool IsMatchingDays(BO.Student isrealStudent, BO.Student other)
        {
            foreach(var dayOfThis in isrealStudent.DesiredLearningTime)
            {
                foreach(var dayOfOther in other.DesiredLearningTime)
                {
                    if(dayOfThis.Day == dayOfOther.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private bool IsMatchingHours(BO.Student isrealStudent, BO.Student other)
        {
            foreach(var dt in other.DesiredLearningTime)
            {
                var diff = GetDifferenceUtc(other.UtcOffset);
                foreach(var t in dt.TimeInDay)
                {
                    var equivalentOffset = GetStudentTimes(diff, t);
                    var equivalentTimeInIsrael = GetTimesInDayByInterval(equivalentOffset);
                        
                    if(isrealStudent.DesiredLearningTime.Any(l => l.Day == dt.Day 
                    && l.TimeInDay.Any(tid => tid == equivalentTimeInIsrael)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// get the time interval equivalent to the isreal local time to the TimeInDay enum 
        /// </summary>
        /// <param name="studentDiff"></param>
        /// <param name="timesInDay"></param>
        /// <returns></returns>
        TimeInterval GetStudentTimes(TimeSpan studentDiff, DO.TimesInDay timesInDay)
        {
            var interval = BoundryOfTimeInDay[timesInDay];
            return new TimeInterval
            {
                Start = interval.Start + studentDiff,
                End = interval.End + studentDiff
            };
        }

        /// <summary>
        /// return the TimeInDay equivalent to the time interval 
        /// </summary>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        DO.TimesInDay GetTimesInDayByInterval(TimeInterval timeInterval)
        {
            if(BoundryOfTimeInDay[DO.TimesInDay.MORNING] == timeInterval)
            {
                return DO.TimesInDay.MORNING;
            }
            if (BoundryOfTimeInDay[DO.TimesInDay.NOON] == timeInterval)
            {
                return DO.TimesInDay.NOON;
            }
            if (BoundryOfTimeInDay[DO.TimesInDay.EVENING] == timeInterval)
            {
                return DO.TimesInDay.EVENING;
            }
            if (BoundryOfTimeInDay[DO.TimesInDay.NIGHT] == timeInterval)
            {
                return DO.TimesInDay.NIGHT;
            }
            return default;
        }

        /// <summary>
        /// return the diffrens bewteen the local time in israel 
        /// to the utc offset of somewhere in the word
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        TimeSpan GetDifferenceUtc(TimeSpan offset)
        {
            return offset - TimeZoneInfo.Local.BaseUtcOffset;
        }
    }

    internal class TimeInterval
    {
        static TimeSpan DELTA = TimeSpan.Parse("01:00");
        
        internal TimeSpan Start { get; set; }
        internal TimeSpan End { get; set; }

        public static bool operator ==(TimeInterval left, TimeInterval right)
        {
            return (left.Start == right.Start && left.End == right.End) ||
                (left.Start + DELTA == right.Start && left.End + DELTA == right.End) ||
                (left.Start - DELTA == right.Start && left.End - DELTA == right.End) ||
                (left.Start - DELTA == right.Start && left.End + DELTA == right.End) ||
                (left.Start + DELTA == right.Start && left.End - DELTA == right.End) ||

                (left.Start == right.Start + DELTA && left.End == right.End + DELTA) ||
                (left.Start == right.Start - DELTA && left.End == right.End - DELTA) ||
                (left.Start == right.Start + DELTA && left.End == right.End - DELTA) ||
                (left.Start == right.Start - DELTA && left.End == right.End + DELTA) ||

                (left.Start + DELTA == right.Start && left.End == right.End + DELTA) ||
                (left.Start + DELTA == right.Start && left.End == right.End - DELTA) ||
                (left.Start - DELTA == right.Start && left.End == right.End + DELTA) ||
                (left.Start - DELTA == right.Start && left.End == right.End - DELTA) ||

                (left.Start == right.Start + DELTA && left.End + DELTA == right.End) ||
                (left.Start == right.Start + DELTA && left.End - DELTA == right.End) ||
                (left.Start == right.Start - DELTA && left.End - DELTA == right.End) ||
                (left.Start == right.Start - DELTA && left.End + DELTA == right.End);
        }

        public static bool operator !=(TimeInterval left, TimeInterval right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return this == obj as TimeInterval;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
