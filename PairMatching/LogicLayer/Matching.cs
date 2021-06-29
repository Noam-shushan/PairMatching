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

        private bool IsMatchingStudentsNotCritical(BO.Student israelStudent, BO.Student other)
        {
            return israelStudent.DesiredSkillLevel == other.SkillLevel
                && israelStudent.LearningStyle == other.LearningStyle;
        }

        /// <summary>
        /// Cheack if the students have the critical condition to be a match
        /// </summary>
        /// <param name="thisStudent"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsMatchingStudentsCritical(BO.Student israelStudent, BO.Student other)
        {
            bool matchTime = IsMatchingHours(israelStudent, other);
            return israelStudent.PrefferdTracks == other.PrefferdTracks
                && israelStudent.EnglishLevel == other.DesiredEnglishLevel
                && (israelStudent.PrefferdGender == other.PrefferdGender
                || (israelStudent.PrefferdGender == other.Gender
                && israelStudent.Gender == other.PrefferdGender))
                && matchTime;
        }

        private bool IsMatchingDays(BO.Student israelStudent, BO.Student other)
        {
            foreach(var dayOfThis in israelStudent.DesiredLearningTime)
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


        private bool IsMatchingHours(BO.Student israelStudent, BO.Student other)
        {
            foreach(var dt in other.DesiredLearningTime)
            {
                var diff = GetDifferenceUtc(other.UtcOffset);
                foreach(var t in dt.TimeInDay)
                {
                    var equivalentIntervalInWord = GetStudentTimes(diff, t);
                    var equivalentTimeInIsrael = GetTimesInDayByInterval(equivalentIntervalInWord);
                    if(equivalentTimeInIsrael == DO.TimesInDay.DONT_MATTER)
                    {
                        continue;
                    }    
                    if(israelStudent.DesiredLearningTime.Any(l => l.Day == dt.Day 
                    && l.TimeInDay.Any(tid => tid == equivalentTimeInIsrael
                     || BoundryOfTimeInDay[tid].IsIn(equivalentIntervalInWord.Start))))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// get the time interval equivalent to the israel local time to the TimeInDay enum 
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
            return DO.TimesInDay.DONT_MATTER;
        }

        /// <summary>
        /// return the diffrens bewteen the local time in israel 
        /// to the utc offset of somewhere in the word
        /// </summary>
        /// <param name="offset">utc offset somewhere in the word</param>
        /// <returns></returns>
        TimeSpan GetDifferenceUtc(TimeSpan offset)
        {
            return offset - TimeZoneInfo.Local.BaseUtcOffset;
        }
    }

    internal class TimeInterval
    {
        static TimeSpan DELTA = TimeSpan.Parse("01:00");
        static TimeSpan MIN_TIME_TO_LEARN = TimeSpan.Parse("02:00");
        
        internal TimeSpan Start { get; set; }
        internal TimeSpan End { get; set; }

        internal bool IsIn(TimeSpan time)
        {
            return ((Start + time) - End) >= MIN_TIME_TO_LEARN;
        }

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
