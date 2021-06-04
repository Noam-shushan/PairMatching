using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class Matching
    {
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

        public bool IsMatchingStudentsNotCritical(BO.Student thisStudent, BO.Student other)
        {
            return thisStudent.DesiredSkillLevel == other.SkillLevel
                && thisStudent.DesiredEnglishLevel == other.EnglishLevel
                && thisStudent.LearningStyle == other.LearningStyle;
        }

        /// <summary>
        /// Cheack if the students have the critical condition to be a match
        /// </summary>
        /// <param name="thisStudent"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsMatchingStudentsCritical(BO.Student thisStudent, BO.Student other)
        {
            return thisStudent.PrefferdTracks == other.PrefferdTracks
                && thisStudent.PrefferdGender == other.Gender
                && isMatchingHours(thisStudent, other)
                && thisStudent.DesiredLearningTime.Day == other.DesiredLearningTime.Day;
        }

        private bool isMatchingHours(BO.Student thisStudent, BO.Student other)
        {
            if (thisStudent.Country == "Israel")
            {   //TODO parse in the correct format of the country string
                var timeSomeWhere = TimeSpan.Parse(other.Country) + DateTime.UtcNow.TimeOfDay;
                return thisStudent.DesiredLearningTime.TimeInDay == getTimesInDay(timeSomeWhere);
            }
            else
            {
                var timeSomeWhere = TimeSpan.Parse(thisStudent.Country) + DateTime.UtcNow.TimeOfDay;
                return other.DesiredLearningTime.TimeInDay == getTimesInDay(timeSomeWhere);
            }

        }

        private DO.TimesInDay getTimesInDay(TimeSpan studentOffset)
        {
            if (studentOffset >= TimeSpan.Parse("7:00") && studentOffset <= TimeSpan.Parse("12:30"))
                return DO.TimesInDay.MORNING;

            if (studentOffset >= TimeSpan.Parse("12:30") && studentOffset <= TimeSpan.Parse("18:30"))
                return DO.TimesInDay.NOON;

            if (studentOffset >= TimeSpan.Parse("18:30") && studentOffset <= TimeSpan.Parse("22:00"))
                return DO.TimesInDay.EVENING;

            if (studentOffset >= TimeSpan.Parse("22:00") && studentOffset <= TimeSpan.Parse("1:00"))
                return DO.TimesInDay.NIGHT;

            return default;
        }
    }
}
