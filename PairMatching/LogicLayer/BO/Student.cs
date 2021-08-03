using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DO;
using LogicLayer;

namespace BO
{
    public class Student : IEquatable<Student>
    {
        /// <summary>
        /// the id number of the student
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// is this student as deleted from the data source
        /// </summary>
        public bool IsDeleted { get; set; }

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

        public bool IsMatch { get => MatchTo != 0; }

        /// <summary>
        /// Desired learning time and day
        /// </summary>
        public IEnumerable<LearningTime> DesiredLearningTime { get; set; }

        public string DesiredLearningTimeShow
        {
            get
            {
                var diff = Country != "Israel" ? $"\nהפרש זמן מישראל: {ReverseString(Matching.GetDifferenceUtc(UtcOffset).Hours.ToString())}" : "";
                return string.Join("\n", from l in DesiredLearningTime
                              let day = Dictionaries.DaysDict[l.Day] + " : "
                              let time = string.Join(", ", from t in l.TimeInDay
                                                          select Dictionaries.TimesInDayDict[t])
                              select day + time) + diff;
            }
        }

        string ReverseString(string s)
        {
            char[] array = s.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public IEnumerable<PrefferdTracks> PrefferdTracks { get; set; }

        public string PrefferdTracksShow
        {
            get
            {
                return string.Join(",\n", from p in PrefferdTracks 
                                         select Dictionaries.PrefferdTracksDict[p]);
            }
        }

        public bool IsSelected { get; set; }

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
            get => Country != "Israel" ? Dictionaries.SkillLevelsDict[SkillLevel] : "";
        }

        public string EnglishLevelShow
        {
            get => Country == "Israel" ? Dictionaries.EnglishLevelsDict[EnglishLevel] : "";
        }

        public string DesiredEnglishLevelShow
        {
            get => Country != "Israel" ? Dictionaries.EnglishLevelsDict[DesiredEnglishLevel] : "";
        }

        public string DesiredSkillLevelShow
        {
            get => Country == "Israel" ? Dictionaries.SkillLevelsDict[DesiredSkillLevel] : "";
        }


        /// <summary>
        /// the utc offset of the student
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        /// <summary>
        /// the id of the student that match to this.
        /// </summary>
        public int MatchTo { get; set; }

        public IEnumerable<SuggestStudent> FirstSuggestStudents { get; set; }

        public IEnumerable<SuggestStudent> SecondeSuggestStudents { get; set; }

        public Dictionary<string, string> OpenQuestions { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"Country: {Country}\n" +
                $"Gender: {Gender}";
        }

        public override bool Equals(object obj) => Equals(obj as Student);

        public override int GetHashCode() => (Id, Name).GetHashCode();

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

