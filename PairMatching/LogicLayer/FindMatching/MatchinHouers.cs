using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;

namespace LogicLayer
{
    // TODO: find the problem with the day after 
    internal class MatchinHouers
    {
        private static Matching matcher = Matching.Instance;

        private static readonly Dictionary<TimesInDay, TimeInterval> BoundryOfTimeInDay =
                new Dictionary<TimesInDay, TimeInterval>()
        {
            [TimesInDay.MORNING] =
                new TimeInterval(TimeSpan.Parse("05:00"), TimeSpan.Parse("12:00")),

            [TimesInDay.NOON] =
                new TimeInterval(TimeSpan.Parse("12:00"), TimeSpan.Parse("18:00")),

            [TimesInDay.EVENING] =
                new TimeInterval(TimeSpan.Parse("18:00"), TimeSpan.Parse("21:00")),

            [TimesInDay.NIGHT] =
                new TimeInterval(TimeSpan.Parse("21:00"), TimeSpan.Parse("02:00"))
        };

        /// <summary>
        /// ceack for equivalent houers between the tow students
        /// </summary>
        /// <param name="israelStudent">student from israel</param>
        /// <param name="other">student from the world</param>
        /// <returns></returns>
        public static bool IsMatchingHours(BO.Student israelStudent, BO.Student other)
        {
            bool found = false;

            // get the difference utc time between the student in israel and student from the world
            var diff = GetDifferenceUtc(other.UtcOffset);

            foreach (var dt in other.DesiredLearningTime)
            {
                foreach (var t in dt.TimeInDay)
                {
                    if (t == TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    var equivalentIntervalInWorld = GetStudentTimes(diff, t);
                    var equivalentTimeInIsrael = GetTimesInDayByInterval(equivalentIntervalInWorld);

                    if (FindMatcinHouers(israelStudent, dt, t,
                        equivalentIntervalInWorld, equivalentTimeInIsrael, diff))
                    {
                        found = true;
                    }
                }
            }
            return found;
        }

        private static bool FindMatcinHouers(BO.Student israeliStudent, LearningTime learningTimeFromWorld,
            TimesInDay timesInDayFromWorld, TimeInterval equivalentIntevalFromWorld,
            TimesInDay equivalentTimeInIsrael, TimeSpan diff)
        {
            bool found = false;
            var ltToAddFromWorld = new LearningTime
            {
                //Id = learningTimeFromWorld.Id,
                Day = learningTimeFromWorld.Day,
                TimeInDay = new List<TimesInDay> { timesInDayFromWorld }
            };
            foreach (var lt in israeliStudent.DesiredLearningTime)
            {
                // if there is more then one day before or after between the tow students
                // there is no point to continue cheacking for matches 
                if (Math.Abs(lt.Day - learningTimeFromWorld.Day) > 1)
                {
                    continue;
                }

                foreach (var t in lt.TimeInDay)
                {
                    if (t == TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    if (lt.Day == learningTimeFromWorld.Day
                        && BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld))
                    {
                        matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                        matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new LearningTime
                        {
                            Day = lt.Day,
                            TimeInDay = new List<TimesInDay> { t }
                        });
                        matcher.AddScores();
                        found = true;
                        continue;
                    }
                    if (diff < TimeSpan.Zero)
                    {
                        if ((lt.Day - learningTimeFromWorld.Day) == 1 &&
                            (BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld)
                            || (t == equivalentTimeInIsrael && equivalentTimeInIsrael != TimesInDay.DONT_MATTER)))
                        {
                            matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                            matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new LearningTime
                            {
                                Day = lt.Day,
                                TimeInDay = new List<TimesInDay> { t }
                            });
                            matcher.AddScores();
                            found = true;
                            continue;
                        }
                    }
                    if (diff > TimeSpan.Zero)
                    {
                        if ((learningTimeFromWorld.Day - lt.Day) == 1 &&
                            (BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld)
                            || (t == equivalentTimeInIsrael && equivalentTimeInIsrael != TimesInDay.DONT_MATTER)))
                        {
                            matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                            matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new LearningTime
                            {
                                Day = lt.Day,
                                TimeInDay = new List<TimesInDay> { t }
                            });
                            matcher.AddScores();
                            found = true;
                            continue;
                        }
                    }
                    if (equivalentTimeInIsrael == TimesInDay.DONT_MATTER)
                    {
                        continue;
                    }
                    if (lt.Day == learningTimeFromWorld.Day
                        && t == equivalentTimeInIsrael)
                    {
                        matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                        matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new LearningTime
                        {
                            Day = lt.Day,
                            TimeInDay = new List<TimesInDay> { t }
                        });
                        matcher.AddScores();
                        found = true;
                        continue;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// get the time interval equivalent to the israel local time to the TimeInDay enum 
        /// </summary>
        /// <param name="studentDiff"></param>
        /// <param name="timesInDay"></param>
        /// <returns></returns>
        static TimeInterval GetStudentTimes(TimeSpan studentDiff, TimesInDay timesInDay)
        {
            var interval = BoundryOfTimeInDay[timesInDay];
            return new TimeInterval(interval.Start - studentDiff,
                interval.End - studentDiff);
        }

        /// <summary>
        /// return the TimeInDay equivalent to the time interval 
        /// </summary>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        static TimesInDay GetTimesInDayByInterval(TimeInterval timeInterval)
        {
            if (BoundryOfTimeInDay[TimesInDay.MORNING] == timeInterval)
            {
                return TimesInDay.MORNING;
            }
            if (BoundryOfTimeInDay[TimesInDay.NOON] == timeInterval)
            {
                return TimesInDay.NOON;
            }
            if (BoundryOfTimeInDay[TimesInDay.EVENING] == timeInterval)
            {
                return TimesInDay.EVENING;
            }
            if (BoundryOfTimeInDay[TimesInDay.NIGHT] == timeInterval)
            {
                return TimesInDay.NIGHT;
            }
            return TimesInDay.DONT_MATTER;
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
}
