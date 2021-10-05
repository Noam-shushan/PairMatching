using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer.FindMatching
{
    // TODO: find the problem with the day after 
    public class MatchinHouers
    {
        private static Matching matcher = Matching.Instance;

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
                    if (t == DO.TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    var equivalentIntervalInWorld = GetStudentTimes(diff, t);
                    var equivalentTimeInIsrael = GetTimesInDayByInterval(equivalentIntervalInWorld);

                    if (FindMatcinHouers(israelStudent, dt, t,
                        equivalentIntervalInWorld, equivalentTimeInIsrael, diff, other.Id))
                    {
                        found = true;
                    }
                }
            }
            return found;
        }

        private static bool FindMatcinHouers(BO.Student israeliStudent, DO.LearningTime learningTimeFromWorld,
            DO.TimesInDay timesInDayFromWorld, TimeInterval equivalentIntevalFromWorld,
            DO.TimesInDay equivalentTimeInIsrael, TimeSpan diff, int worldStudentId)
        {
            bool found = false;
            var ltToAddFromWorld = new DO.LearningTime
            {
                //Id = learningTimeFromWorld.Id,
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
                foreach (var t in lt.TimeInDay)
                {
                    if (t == DO.TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    if (lt.Day == learningTimeFromWorld.Day
                        && BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld.Start))
                    {
                        matcher.MatchingTimes.Add(new MatchingTime
                        {
                            MatchingLearningTimeInIsrael = new DO.LearningTime
                            {
                                Day = lt.Day,
                                TimeInDay = new List<DO.TimesInDay> { t }
                            },
                            MatchingLearningTimeInWorld = ltToAddFromWorld,
                            StudentFromIsraelId = israeliStudent.Id,
                            StudentFromWorldId = worldStudentId
                        });
                        matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                        matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
                        {
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
                            matcher.MatchingTimes.Add(new MatchingTime
                            {
                                MatchingLearningTimeInIsrael = new DO.LearningTime
                                {
                                    Day = lt.Day,
                                    TimeInDay = new List<DO.TimesInDay> { t }
                                },
                                MatchingLearningTimeInWorld = ltToAddFromWorld,
                                StudentFromIsraelId = israeliStudent.Id,
                                StudentFromWorldId = worldStudentId
                            });
                            matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                            matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
                            {
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
                            matcher.MatchingTimes.Add(new MatchingTime
                            {
                                MatchingLearningTimeInIsrael = new DO.LearningTime
                                {
                                    Day = lt.Day,
                                    TimeInDay = new List<DO.TimesInDay> { t }
                                },
                                MatchingLearningTimeInWorld = ltToAddFromWorld,
                                StudentFromIsraelId = israeliStudent.Id,
                                StudentFromWorldId = worldStudentId
                            });
                            matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                            matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
                            {
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
                        matcher.MatchingTimes.Add(new MatchingTime
                        {
                            MatchingLearningTimeInIsrael = new DO.LearningTime
                            {
                                Day = lt.Day,
                                TimeInDay = new List<DO.TimesInDay> { t }
                            },
                            MatchingLearningTimeInWorld = ltToAddFromWorld,
                            StudentFromIsraelId = israeliStudent.Id,
                            StudentFromWorldId = worldStudentId
                        });
                        matcher.CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                        matcher.CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
                        {
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

        /// <summary>
        /// get the time interval equivalent to the israel local time to the TimeInDay enum 
        /// </summary>
        /// <param name="studentDiff"></param>
        /// <param name="timesInDay"></param>
        /// <returns></returns>
        static TimeInterval GetStudentTimes(TimeSpan studentDiff, DO.TimesInDay timesInDay)
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
        static DO.TimesInDay GetTimesInDayByInterval(TimeInterval timeInterval)
        {
            if (BoundryOfTimeInDay[DO.TimesInDay.MORNING] == timeInterval)
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

    public class MatchingTime
    {
        public DO.LearningTime MatchingLearningTimeInIsrael { get; set; }

        public DO.LearningTime MatchingLearningTimeInWorld { get; set; }

        public int StudentFromIsraelId { get; set; }

        public int StudentFromWorldId { get; set; }
    }
}
