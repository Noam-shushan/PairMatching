using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using UtilEntities;

namespace Gui.ViewModels
{
    public class StudentViewModel : INotifyPropertyChanged
    {
        private Student _student;

        public StudentViewModel(Student student)
        {
            _student = student;
        }

        /// <summary>
        /// the id number of the student
        /// </summary>
        public int Id { get => _student.Id; }

        public bool IsSimpleStudent { get => _student.IsSimpleStudent; }

        /// <summary>
        /// the name of the student
        /// </summary>
        public string Name 
        { 
            get => _student.Name;
        }

        /// <summary>
        /// the country of the student
        /// </summary>
        public string Country 
        { 
            get => _student.Country;
            set
            {
                _student.Country = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Country"));
            } 
        }

        /// <summary>
        /// the email of the student
        /// </summary>
        public string Email
        {
            get => _student.Email;
            set
            {
                _student.Email = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Email"));
            }
        }

        /// <summary>
        /// the phone number of the student
        /// </summary>
        public string PhoneNumber
        {
            get => _student.PhoneNumber;
            set
            {
                _student.PhoneNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhoneNumber"));
            }
        }

        /// <summary>
        /// the gender of the student
        /// </summary>
        public string Gender
        {
            get => Dictionaries.GendersDict[_student.Gender];
        }

        public bool IsMatch { get => _student.IsMatch; }

        public string DesiredLearningTime
        {
            get
            {
                if (IsSimpleStudent)
                {
                    return "";
                }
                var diffVal = _student.DiffFromIsrael;
                var diff = !IsFromIsrael ? $"\nהפרש זמן מישראל: {Math.Abs(diffVal)} שעות " + (diffVal < 0 ? "אחורה" : "קדימה") : "";
                return string.Join("\n", from l in _student.DesiredLearningTime
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

        public string PrefferdTracks 
        {
            get => string.Join(",\n", from p in _student.PrefferdTracks
                                      select Dictionaries.PrefferdTracksDict[p]);
            set
            {
                _student.PrefferdTracks = new List<PrefferdTracks>
                {
                    Dictionaries.PrefferdTracksDictInverse[value]
                };
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PrefferdTracks"));
            }
        }
        public DateTime DateOfRegistered { get; set; }

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

        public string MatchToShow { get; set; }

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

        public IEnumerable<SuggestStudent> FirstSuggestStudents { get => _student.FirstSuggestStudents; }

        public IEnumerable<SuggestStudent> SecondeSuggestStudents { get => _student.SecondeSuggestStudents; }

        Dictionary<string, string> _openQuestionsDict;
        public Dictionary<string, string> OpenQuestionsDict
        {
            get
            {
                if(_openQuestionsDict == null)
                {
                    _openQuestionsDict = new Dictionary<string, string>();
                    foreach (var o in _student.OpenQuestions)
                    {
                        _openQuestionsDict.Add(o.Question, o.Answer.SpliceText(10));
                    }
                }
                return _openQuestionsDict;
            }
        }


        public List<Note> Notes { get => _student.Notes; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
