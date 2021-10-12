using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicLayer.FindMatching
{
    public class Matching
    {
        public static Matching Instance { get; } = new Matching();

        private Matching() { }

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

        public void AddScores()
        {
            CurrentForIsraeliSuggest.AddScore();
            CurrentForWorldSuggest.AddScore();
        }

        /// <summary>
        /// Cheack if tow student is a match from first degree 
        /// </summary>
        /// <param name="thisStudent"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsFirstMatching(BO.Student thisStudent, BO.Student other, bool flagNotFound = false)
        {
            return IsMatchingStudentsNotCritical(thisStudent, other)
                && IsMatchingStudentsCritical(thisStudent, other, flagNotFound);
        }

        private bool IsMatchingStudentsNotCritical(BO.Student israelStudent, BO.Student other)
        {
            return ((israelStudent.DesiredSkillLevel <= other.SkillLevel && other.SkillLevel != DO.SkillLevels.DONT_MATTER) 
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
        public bool IsMatchingStudentsCritical(BO.Student israelStudent, BO.Student other, bool flagNotFound = false)
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

            if (israelStudent.PrefferdGender == other.Gender
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

            if (israelStudent.PrefferdTracks
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

            if (flagNotFound)
            {
                IsMatch = matchTrack
                && matchEnglishLevel
                && matchGender
                && matchLenguage;
            }
            else
            {
                IsMatch = matchTrack
                    && matchEnglishLevel
                    && matchGender
                    && matchLenguage
                    && MatchinHouers.IsMatchingHours(israelStudent, other);
            }

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
    }
}
