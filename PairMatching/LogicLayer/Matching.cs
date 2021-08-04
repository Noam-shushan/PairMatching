using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Matching
    {
        private static readonly Dictionary<DO.TimesInDay, TimeInterval> BoundryOfTimeInDay =
            new Dictionary<DO.TimesInDay, TimeInterval>()
            {
                {DO.TimesInDay.MORNING,
                    new TimeInterval(TimeSpan.Parse("05:00"), TimeSpan.Parse("12:00"))},
                
                {DO.TimesInDay.NOON,
                    new TimeInterval(TimeSpan.Parse("12:00"), TimeSpan.Parse("18:00"))},
                
                {DO.TimesInDay.EVENING,
                    new TimeInterval(TimeSpan.Parse("18:00"), TimeSpan.Parse("21:00"))},
                
                {DO.TimesInDay.NIGHT,
                    new TimeInterval(TimeSpan.Parse("21:00"), TimeSpan.Parse("02:00"))}
            };

        public List<Tuple<DO.LearningTime, DO.LearningTime>> MatchingHoursList { get; } =
            new List<Tuple<DO.LearningTime, DO.LearningTime>>();

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
            return (israelStudent.DesiredSkillLevel <= other.SkillLevel
                || israelStudent.DesiredSkillLevel == DO.SkillLevels.DONT_MATTER)
                && (israelStudent.LearningStyle == other.LearningStyle
                || israelStudent.LearningStyle == DO.LearningStyles.DONT_MATTER 
                     || other.LearningStyle == DO.LearningStyles.DONT_MATTER);
        }

        /// <summary>
        /// Cheack if the students have the critical condition to be a match
        /// </summary>
        /// <param name="israelStudent">student from israel</param>
        /// <param name="other"></param>
        /// <returns>true is there is a match in the critical desires of the tow students</returns>
        public bool IsMatchingStudentsCritical(BO.Student israelStudent, BO.Student other)
        {
            bool matchEnglishLevel = other.DesiredEnglishLevel == DO.EnglishLevels.DONT_MATTER 
                                    || other.DesiredEnglishLevel <= israelStudent.EnglishLevel;

            bool matchGender = israelStudent.PrefferdGender == other.PrefferdGender
                               || (other.PrefferdGender == DO.Genders.DONT_MATTER
                                    && israelStudent.PrefferdGender == other.Gender)
                               || (israelStudent.PrefferdGender == DO.Genders.DONT_MATTER
                                    && israelStudent.Gender == other.PrefferdGender)
                               || (israelStudent.PrefferdGender == other.Gender
                                    && other.PrefferdGender == israelStudent.Gender);
            
            bool matchTrack = israelStudent.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER)
                              || other.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER) 
                              || israelStudent.PrefferdTracks
                                  .Select(p => p)
                                  .Intersect(other.PrefferdTracks)
                                  .Any();
 
            return matchTrack
                && matchEnglishLevel
                && matchGender
                && IsMatchingHours(israelStudent, other);
        }

        /// <summary>
        /// ceack for equivalent houers between the tow students
        /// </summary>
        /// <param name="israelStudent">student from israel</param>
        /// <param name="other">student from the world</param>
        /// <returns></returns>
        private bool IsMatchingHours(BO.Student israelStudent, BO.Student other)
        {
            bool found = false;
            
            // get the difference utc time between the student in israel and student from the world
            var diff = GetDifferenceUtc(other.UtcOffset);
            
            foreach (var dt in other.DesiredLearningTime)
            {
                foreach(var t in dt.TimeInDay)
                {
                    if(t == DO.TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    var equivalentIntervalInWorld = GetStudentTimes(diff, t);
                    var equivalentTimeInIsrael = GetTimesInDayByInterval(equivalentIntervalInWorld);
                    
                    if(FindMatcinHouers(israelStudent, dt,t, 
                        equivalentIntervalInWorld, equivalentTimeInIsrael, diff))
                    {
                        found = true;
                    }
                }
            }
            return found;
        }

        private bool FindMatcinHouers(BO.Student israeliStudent, DO.LearningTime learningTimeFromWorld,
            DO.TimesInDay timesInDayFromWorld, TimeInterval equivalentIntevalFromWorld, 
            DO.TimesInDay equivalentTimeInIsrael, TimeSpan diff)
        {
            bool found = false;
            var ltToAddFromWorld = new DO.LearningTime
            {
                Id = learningTimeFromWorld.Id,
                Day = learningTimeFromWorld.Day,
                TimeInDay = new List<DO.TimesInDay> { timesInDayFromWorld }
            };
            foreach (var lt in israeliStudent.DesiredLearningTime)
            {
                // if there is more then one day before or after between the tow students
                // there is no point to continue cheacking for matches 
                if (Math.Abs(lt.Day - learningTimeFromWorld.Day) > 1)
                {
                    continue;
                }
                foreach(var t in lt.TimeInDay)
                {
                    if (t == DO.TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    if (lt.Day == learningTimeFromWorld.Day 
                        && BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld.Start))
                    {
                        AddToMatchingHouersList(ltToAddFromWorld, new DO.LearningTime 
                        {
                            Id = lt.Id,
                            Day = lt.Day,
                            TimeInDay = new List<DO.TimesInDay> { t } 
                        });
                        found = true;
                        continue;
                    }
                    if (diff < TimeSpan.Zero)
                    {
                        if ((lt.Day - learningTimeFromWorld.Day) == 1 &&
                            (BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld.Start)
                            || (t == equivalentTimeInIsrael && equivalentTimeInIsrael != DO.TimesInDay.DONT_MATTER)))
                        {
                            AddToMatchingHouersList(ltToAddFromWorld, new DO.LearningTime
                            {
                                Id = lt.Id,
                                Day = lt.Day,
                                TimeInDay = new List<DO.TimesInDay> { t }
                            });
                            found = true;
                            continue;
                        }
                    }
                    if (diff > TimeSpan.Zero)
                    {
                        if ((learningTimeFromWorld.Day - lt.Day) == 1 &&
                            (BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld.Start)
                            || (t == equivalentTimeInIsrael && equivalentTimeInIsrael != DO.TimesInDay.DONT_MATTER)))
                        {
                            AddToMatchingHouersList(ltToAddFromWorld, new DO.LearningTime
                            {
                                Id = lt.Id,
                                Day = lt.Day,
                                TimeInDay = new List<DO.TimesInDay> { t }
                            });
                            found = true;
                            continue;
                        }
                    }
                    if (equivalentTimeInIsrael == DO.TimesInDay.DONT_MATTER)
                    {
                        continue;
                    }
                    if (lt.Day == learningTimeFromWorld.Day 
                        && t == equivalentTimeInIsrael)
                    {
                        AddToMatchingHouersList(ltToAddFromWorld, new DO.LearningTime
                        {
                            Id = lt.Id,
                            Day = lt.Day,
                            TimeInDay = new List<DO.TimesInDay> { t }
                        });
                        found = true;
                        continue;
                    }
                }
            }

            return found;
        }

        private void AddToMatchingHouersList(DO.LearningTime first, DO.LearningTime second)
        {
/*            if(!(from m in MatchingHoursList
                where m.Item1.Equals(first)
               select m.Item1).Any() && 
               !(from m in MatchingHoursList
                 where m.Item2.Equals(second)
                 select m.Item2).Any())*/
            {
                MatchingHoursList.Add(new Tuple<DO.LearningTime, DO.LearningTime>(first, second));
            }
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
            return new TimeInterval(interval.Start + studentDiff, 
                interval.End + studentDiff);
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
        public static TimeSpan GetDifferenceUtc(TimeSpan offset)
        {
            return offset - TimeZoneInfo.Local.BaseUtcOffset;
        }
    }

    internal class TimeInterval
    {
        private static TimeSpan DELTA = TimeSpan.Parse("01:00");
        private static TimeSpan MIN_TIME_TO_LEARN = TimeSpan.Parse("02:00");

        internal TimeInterval(TimeSpan start, TimeSpan end)
        {
            Start = Raound(start);
            End = end < start ? Raound(end + TimeSpan.FromHours(24)) : Raound(end);
        }

        internal TimeSpan Start { get; set; }
        internal TimeSpan End { get; set; }

        private TimeSpan Raound(TimeSpan t)
        {
            if (t < TimeSpan.Zero)
                return TimeSpan.FromHours(24 + t.Hours);
            return t;
        }

        internal bool IsIn(TimeSpan time)
        {
            return time + MIN_TIME_TO_LEARN <= End && time >= Start;
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
            return (Start, End).GetHashCode();
        }
    }
}
