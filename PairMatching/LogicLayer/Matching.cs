using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer
{
    public class Matching
    {
        public BO.SuggestStudent CurrentForIsraeliSuggest { get; set; }
        public BO.SuggestStudent CurrentForWorldSuggest = new BO.SuggestStudent();
        public bool IsMatch { get; set; }

        public void BuildMatch(BO.Student israelStudent, BO.Student other)
        {
            CurrentForWorldSuggest = new BO.SuggestStudent
            {
                ThisStudentId = other.Id,
                SuggestStudentId = israelStudent.Id,
                MatcingScore = 0,
                SuggestStudenCountry = israelStudent.Country,
                SuggestStudentName = israelStudent.Name,
            };
            CurrentForIsraeliSuggest = new BO.SuggestStudent
            {
                ThisStudentId = israelStudent.Id,
                SuggestStudentId = other.Id,
                MatcingScore = 0,
                SuggestStudenCountry = other.Country,
                SuggestStudentName = other.Name,
            };
        }

        private void AddScores()
        {
            CurrentForIsraeliSuggest.AddScore();
            CurrentForWorldSuggest.AddScore();
        }

        public List<MatchingTime> MatchingTimes { get; } =
                new List<MatchingTime>();

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
            bool matchLenguage =
                ((israelStudent.MoreLanguages == DO.MoreLanguages.NO || 
                    israelStudent.MoreLanguages == DO.MoreLanguages.YES) 
                    && (other.MoreLanguages == DO.MoreLanguages.YES
                        || other.MoreLanguages == DO.MoreLanguages.NO))
                ||
                (israelStudent.MoreLanguages == DO.MoreLanguages.NOT_ENGLISH
                    && israelStudent.Languages
                        .Select(l => l)
                        .Intersect(other.Languages)
                        .Any()) 
                || (other.MoreLanguages == DO.MoreLanguages.NOT_ENGLISH
                    && other.Languages
                        .Select(l => l)
                        .Intersect(israelStudent.Languages)
                        .Any());

            if (other.DesiredEnglishLevel <= israelStudent.EnglishLevel)
            {
                AddScores();
            }
            bool matchEnglishLevel = other.DesiredEnglishLevel == DO.EnglishLevels.DONT_MATTER 
                                    || other.DesiredEnglishLevel <= israelStudent.EnglishLevel;

            if(israelStudent.PrefferdGender == other.Gender 
                && israelStudent.Gender == other.PrefferdGender)
            {
                AddScores();
            }
            bool matchGender = israelStudent.PrefferdGender == other.PrefferdGender
                               || (other.PrefferdGender == DO.Genders.DONT_MATTER
                                    && israelStudent.PrefferdGender == other.Gender)
                               || (israelStudent.PrefferdGender == DO.Genders.DONT_MATTER
                                    && israelStudent.Gender == other.PrefferdGender)
                               || (israelStudent.PrefferdGender == other.Gender
                                    && other.PrefferdGender == israelStudent.Gender);
            
            if(israelStudent.PrefferdTracks
                                  .Where(p => p != DO.PrefferdTracks.DONT_MATTER)
                                  .Intersect(other.PrefferdTracks)
                                  .Any())
            {
                AddScores();
            }

            bool matchTrack = israelStudent.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER)
                              || other.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER) 
                              || israelStudent.PrefferdTracks
                                  .Select(p => p)
                                  .Intersect(other.PrefferdTracks)
                                  .Any();

            IsMatch = matchTrack
                && matchEnglishLevel
                && matchGender
                && matchLenguage
                && IsMatchingHours(israelStudent, other);

            if (IsMatch)
            {
                OrderMatchingLearningTime(CurrentForIsraeliSuggest);
                OrderMatchingLearningTime(CurrentForWorldSuggest);

            }

            return IsMatch;
        }

        private void OrderMatchingLearningTime(BO.SuggestStudent suggestStudent)
        {
            suggestStudent.MatchingLearningTime
                = (from m in suggestStudent.MatchingLearningTime
                   group m.TimeInDay.First() by m.Day into times
                   select new DO.LearningTime
                   {
                       Day = times.Key,
                       TimeInDay = times.Distinct()
                   }).ToList();
                }

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
                    
                    if(FindMatcinHouers(israelStudent, dt, t, 
                        equivalentIntervalInWorld, equivalentTimeInIsrael, diff, other.Id))
                    {
                        found = true;
                    }
                }
            }
            return found;
        }

        private bool FindMatcinHouers(BO.Student israeliStudent, DO.LearningTime learningTimeFromWorld,
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
                foreach(var t in lt.TimeInDay)
                {
                    if (t == DO.TimesInDay.INCAPABLE)
                    {
                        continue;
                    }
                    if (lt.Day == learningTimeFromWorld.Day 
                        && BoundryOfTimeInDay[t].IsIn(equivalentIntevalFromWorld.Start))
                    {
                        MatchingTimes.Add(new MatchingTime
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
                        CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                        CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
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
                            MatchingTimes.Add(new MatchingTime
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
                            CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                            CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
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
                            MatchingTimes.Add(new MatchingTime
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
                            CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                            CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
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
                        MatchingTimes.Add(new MatchingTime
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
                        CurrentForIsraeliSuggest.MatchingLearningTime.Add(ltToAddFromWorld);
                        CurrentForWorldSuggest.MatchingLearningTime.Add(new DO.LearningTime
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

    public class MatchingTime
    {
        public DO.LearningTime MatchingLearningTimeInIsrael { get; set; }
        
        public DO.LearningTime MatchingLearningTimeInWorld { get; set; }

        public int StudentFromIsraelId { get; set; }

        public int StudentFromWorldId { get; set; }
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
