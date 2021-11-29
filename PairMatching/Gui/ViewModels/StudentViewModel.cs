using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BO;
using Gui.Commands;
using Gui.Views;
using LogicLayer;
using UtilEntities;

namespace Gui.ViewModels
{
    public class StudentViewModel : ViewModelBase
    {
        private readonly ILogicLayer logicLayer = LogicFactory.GetLogicFactory();

        private Student _student;

        public UpdateStudentCommand UpdateStudent { get; set; }

        public CompareTwoStudentsCommand CompareTo { get; set; }

        public NotesViewModel Notes { get; set; }

        public MatchingHistoriesViewModel MatchingHistories { get; set; }

        public StudentViewModel(Student student)
        {
            _student = student;
            Notes = new NotesViewModel(_student.Notes)
            {
                IsStudent = true
            };

            MatchingHistories = new MatchingHistoriesViewModel(_student.MatchingHistories)
            {
                DateOfRegistered = _student.DateOfRegistered
            };

            UpdateStudent = new UpdateStudentCommand();
            UpdateStudent.Update += UpdateStudent_Update;

            CompareTo = new CompareTwoStudentsCommand();
            CompareTo.Compare += CompareTo_Compare;
        }

        private void CompareTo_Compare(CompareTwoStudentsViewModel compareTwoStudents)
        {
            new CompareTwoStudentsView(compareTwoStudents).Show();
        }

        private void UpdateStudent_Update(StudentViewModel studentViewModel)
        {
            if (IsChanged)
            {
                logicLayer.UpdateStudent(_student);
            }
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
                OnPropertyChanged(nameof(Country));
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
                OnPropertyChanged(nameof(Email));
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
                OnPropertyChanged(nameof(PhoneNumber));
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

        private string _desiredLearningTime;
        public string DesiredLearningTime
        {
            get
            {
                if (IsSimpleStudent)
                {
                    return "";
                }
                if (!string.IsNullOrEmpty(_desiredLearningTime))
                {
                    return _desiredLearningTime;
                }
                var diffVal = _student.DiffFromIsrael;
                var diff = !IsFromIsrael ? $"\nהפרש זמן מישראל: {Math.Abs(diffVal)} שעות " + (diffVal < 0 ? "אחורה" : "קדימה") : "";
                _desiredLearningTime = string.Join("\n", from l in _student.DesiredLearningTime
                                         let day = Dictionaries.DaysDict[l.Day] + " : "
                                         let time = string.Join(", ", from t in l.TimeInDay
                                                                      select Dictionaries.TimesInDayDict[t])
                                         select day + time) + diff;
                return _desiredLearningTime;
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

        private string _prefferdTracks;
        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public string PrefferdTracks 
        {
            get 
            {
                if (!string.IsNullOrEmpty(_prefferdTracks))
                {
                    return _prefferdTracks;
                }
                _prefferdTracks = string.Join(",\n", from p in _student.PrefferdTracks
                                                     select Dictionaries.PrefferdTracksDict[p]);
                return _prefferdTracks;
            }
            
            set
            {
                _student.PrefferdTracks = new List<PrefferdTracks>
                {
                    Dictionaries.PrefferdTracksDictInverse[value]
                };
                OnPropertyChanged(nameof(PrefferdTracks));
            }
        }

        public bool IsOpenToMatch
        {
            get => _student.IsOpenToMatch;
        }

        public string PrefferdGender { get => Dictionaries.GendersDict[_student.PrefferdGender]; }

        public string LearningStyle
        {
            get => Dictionaries.LearningStylesDict[_student.LearningStyle];
        }

        public string SkillLevel
        {
            get => !_student.IsFromIsrael ? Dictionaries.SkillLevelsDict[_student.SkillLevel] : "";
        }

        public string EnglishLevel
        {
            get => _student.IsFromIsrael ? Dictionaries.EnglishLevelsDict[_student.EnglishLevel] : "";
        }

        public string DesiredEnglishLevel
        {
            get => !_student.IsFromIsrael ? Dictionaries.DesiredEnglishLevelsDict[_student.DesiredEnglishLevel] : "";
        }

        public string DesiredSkillLevel
        {
            get => _student.IsFromIsrael ? Dictionaries.SkillLevelsDict[_student.DesiredSkillLevel] : "";
        }

        public bool IsFromIsrael { get => _student.IsFromIsrael; }

        public string MatchTo { get => _student.MatchToShow; }

        bool _isCompereWin;
        public bool IsCompereWin 
        { 
            get => _isCompereWin;
            set
            {
                _isCompereWin = value;
                OnPropertyChanged(nameof(IsCompereWin));
            } 
        }

        public string Languages
        {
            get => _student.Languages.Count() == 0 ? "" : string.Join(", ", _student.Languages);
        }

        public bool IsKnowMoreLanguages { get => _student.IsKnowMoreLanguages; }

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
    }
}
