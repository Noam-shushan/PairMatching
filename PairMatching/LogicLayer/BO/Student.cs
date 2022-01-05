using System;
using System.Collections.Generic;
using static LogicLayer.FindMatching.MatchinHouers;
using UtilEntities;
using System.Linq;

namespace BO
{
    public class Student : IEquatable<Student>
    {
        /// <summary>
        /// the id number of the student
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// is this student as deleted from the database
        /// </summary>
        public bool IsDeleted { get; set; }

        public bool IsSimpleStudent { get; set; } = false;

        /// <summary>
        /// the name of the student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the country of the student
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// the email of the student
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the phone number of the student
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the gender of the student
        /// </summary>
        public Genders Gender { get; set; }

        public string GenderShow { get => Dictionaries.GendersDict[Gender]; }

        public bool IsMatch { get => MatchTo.Count() > 0; }

        /// <summary>
        /// Desired learning time and day
        /// </summary>
        public IEnumerable<LearningTime> DesiredLearningTime { get; set; } = new List<LearningTime>();

        public string DesiredLearningTimeShow
        {
            get
            {
                if (IsSimpleStudent)
                {
                    return "";
                }
                var diffVal = GetDifferenceUtc(UtcOffset).Hours;
                var diff = !IsFromIsrael ? $"\nהפרש זמן מישראל: {Math.Abs(diffVal)} שעות " + (diffVal < 0 ? "אחורה" : "קדימה")  : "";
                return string.Join("\n", from l in DesiredLearningTime
                              let day = Dictionaries.DaysDict[l.Day] + " : "
                              let time = string.Join(", ", from t in l.TimeInDay
                                                          select Dictionaries.TimesInDayDict[t])
                              select day + time) + diff;
            }
        }

        public Dictionary<string, string> OpenQuestionsHeaders
        {
            get
            {
                return IsFromIsrael ? Dictionaries.OpenQuestionsHeaderInHebrow :
                    Dictionaries.OpenQuestionsHeaderInEnglish;
            }
        }

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public List<PrefferdTracks> PrefferdTracks { get; set; } = new List<PrefferdTracks>();

        public string PrefferdTracksShow => string.Join(",\n", from p in PrefferdTracks
                                                               select Dictionaries.PrefferdTracksDict[p]);
        public DateTime DateOfRegistered { get; set; }

        public List<StudentMatchingHistory> MatchingHistories { get; set; } =
                new List<StudentMatchingHistory>();

        public List<StudentMatchingHistoryShow> MatchingHistoriesShow { get; set; } =
        new List<StudentMatchingHistoryShow>();

        public bool IsSelected { get; set; }

        public bool IsOpenToMatch
        {
            get => !IsSimpleStudent && (MatchTo.Count() < PrefferdNumberOfMatchs);
        }

        /// <summary>
        /// the prefferd gender to lern with
        /// </summary>
        public Genders PrefferdGender { get; set; }

        public string PrefferdGenderShow { get => Dictionaries.GendersDict[PrefferdGender]; }

        /// <summary>
        ///  Desired level of english from the other pair
        /// </summary>
        public EnglishLevels DesiredEnglishLevel { get; set; }

        /// <summary>
        ///  level of english
        /// </summary>
        public EnglishLevels EnglishLevel { get; set; }

        /// <summary>
        ///  Desired level of skiil from the other pair
        /// </summary>
        public SkillLevels DesiredSkillLevel { get; set; }

        /// <summary>
        /// level of skiil 
        /// </summary>
        public SkillLevels SkillLevel { get; set; }

        /// <summary>
        /// learning style 
        /// </summary>
        public LearningStyles LearningStyle { get; set; }

        public string LearningStyleShow
        {
            get => Dictionaries.LearningStylesDict[LearningStyle];
        }

        public string SkillLevelShow
        {
            get => !IsFromIsrael ? Dictionaries.SkillLevelsDict[SkillLevel] : "";
        }

        public string EnglishLevelShow
        {
            get => IsFromIsrael ? Dictionaries.EnglishLevelsDict[EnglishLevel] : "";
        }

        public string DesiredEnglishLevelShow
        {
            get => !IsFromIsrael ? Dictionaries.DesiredEnglishLevelsDict[DesiredEnglishLevel] : "";
        }

        public string DesiredSkillLevelShow
        {
            get => IsFromIsrael ? Dictionaries.SkillLevelsDict[DesiredSkillLevel] : "";
        }

        public bool IsFromIsrael { get => Country == "Israel"; }


        /// <summary>
        /// the utc offset of the student
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        /// <summary>
        /// the id of the student that match to this.
        /// </summary>
        public List<int> MatchTo { get; set; } = new List<int>();

        public IEnumerable<Student> MatchToShow { get; set; }

        public int PrefferdNumberOfMatchs { get; set; }

        public string InfoAbout { get; set; } = "";

        public bool IsCompereWin { get; set; }

        public List<string> Languages { get; set; } = new List<string>();

        public string LanguagesShow 
        {
            get => Languages.Count() == 0 ? "" : string.Join(", ", Languages); 
        }

        public MoreLanguages MoreLanguages { get; set; }

        public bool IsKnowMoreLanguages { get => Languages.Count() > 0; }

        public IEnumerable<SuggestStudent> FirstSuggestStudents { get; set; }

        public IEnumerable<SuggestStudent> SecondeSuggestStudents { get; set; }

        public Dictionary<string, string> OpenQuestionsDict { get; set; }

        public IEnumerable<OpenQuestion> OpenQuestions { get; set; }

        public List<Note> Notes { get; set; } = new List<Note>();

        public int DiffFromIsrael { get => GetDifferenceUtc(UtcOffset).Hours; }

        public bool IsInArchive { get; set; }
        
        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"Country: {Country}\n" +
                $"Gender: {Gender}";
        }

        public override bool Equals(object obj) => Equals(obj as Student);

        public override int GetHashCode() => (Id, Name).GetHashCode();

        public bool IsInTheSuggestStudents(Student student)
        {
            if (FirstSuggestStudents != null
                    && FirstSuggestStudents.Any(s => s.SuggestStudentId == student.Id))
            {
                return true;
            }

            if (SecondeSuggestStudents != null
                && SecondeSuggestStudents.Any(s => s.SuggestStudentId == student.Id))
            {
                return true;
            }
            return false;
        }

        public bool Equals(Student other)
        {
            if (other is null)
            {
                return false;
            }

            return Id == other.Id;
        }
    }
}

