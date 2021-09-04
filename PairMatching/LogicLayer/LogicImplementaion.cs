using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace LogicLayer
{
    /// <summary>
    /// Logic implemention of the logic layer main inteface
    /// </summary>
    internal class LogicImplementaion : IBL, INotifyPropertyChanged
    {
        // tha singelton of the data layer 
        private readonly IDataLayer dal = DalFactory.GetDal();

        /// <summary>
        /// maching object to cheack all the condition to make a match
        /// </summary>
        private Matching matcher = new Matching();

        private readonly SendEmail sendEmail = SendEmail.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Predicate<BO.Student> BaseStudentsFilter = s => !s.IsDeleted;
        Predicate<BO.Student> _studentListFilter = BaseStudentsFilter;
        public Predicate<BO.Student> StudentListFilter 
        {
            get => _studentListFilter;
            set
            {
                _studentListFilter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StudentListFilter"));
            } 
        }

        public BO.Statistics Statistics 
        {
            get 
            {
                return new BO.Statistics
                {
                    NumberOfPair = _pairList.Count,
                    NumberOfStudentFromIsrael = _studentList.Where(s => s.IsFromIsrael).Count(),
                    NumberOfStudents = _studentList.Count,
                    NumberOfStudentsWithoutPair = _studentList.Where(s => !s.IsMatch).Count(),
                    NumberOfStudentFromWorldWithoutPair = _studentList.Where(s => !s.IsMatch && !s.IsFromIsrael).Count(),
                    NumberOfStudentFromIsraelWithoutPair = _studentList.Where(s => !s.IsMatch && s.IsFromIsrael).Count(),
                    NumberOfStudentFromWorld = _studentList.Where(s => !s.IsFromIsrael).Count()
                };
            }
        }

        ObservableCollection<BO.Student> _studentList = new ObservableCollection<BO.Student>();
        /// <summary>
        /// the students list that keep all the data of the students
        /// </summary>
        public ObservableCollection<BO.Student> StudentList 
        {
            get 
            {
                if (StudentListFilter != BaseStudentsFilter)
                {
                    return new ObservableCollection<BO.Student>
                        (_studentList.Where(s => StudentListFilter(s)));
                }
                else
                {
                    return _studentList;
                }
            } 
            set
            {
                _studentList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StudentList"));
            } 
        }

        public static Predicate<BO.Pair> BasePairsFilter = p => !p.IsDeleted;
        Predicate<BO.Pair> _pairListFilter = BasePairsFilter;
        public Predicate<BO.Pair> PairListFilter 
        { 
            get => _pairListFilter;
            set
            {
                _pairListFilter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PairListFilter"));
            }
        }

        ObservableCollection<BO.Pair> _pairList = new ObservableCollection<BO.Pair>();
        /// <summary>
        /// the students list that keep all the data of the students
        /// </summary>
        public ObservableCollection<BO.Pair> PairList 
        {
            get
            {
                if(PairListFilter != BasePairsFilter)
                {
                    return new ObservableCollection<BO.Pair>(_pairList.Where(p => PairListFilter(p)));
                }
                else
                {
                    return _pairList;
                }
               
            }
            set
            {
                _pairList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PairList"));
            } 
        }
            

        public List<BO.SimpleStudent> StudentWithUnvalidEmail { get; set; } =
             new List<BO.SimpleStudent>();

        #region Singelton referens of the logic layer
        public static IBL Instance { get; } = new LogicImplementaion();

        private LogicImplementaion() { }
        #endregion

        #region Updating data from the Google Sheets and from the data base
        /// <summary>
        /// Update all the data from the google sheets
        /// </summary>
        public async Task ReadDataFromSpredsheetAsync()
        {
            try
            {
                // get the last date of the update of the sheets to read from there
                var lastDate = dal.GetLastDateOfSheets();
                var oldDate = new DO.LastDataOfSpredsheet 
                { 
                    EnglishSheets = lastDate.EnglishSheets, 
                    HebrewSheets = lastDate.HebrewSheets 
                };

                // create parser for the spradsheets
                GoogleSheetParser parser = new GoogleSheetParser();
                // read the english sheet
                var englishSheets = parser.ReadAsync(new EnglishDiscriptor(lastDate));
                // read the hebrew sheet
                var hebrewSheets = parser.ReadAsync(new HebrewDescriptor(lastDate));

                await Task.WhenAll(englishSheets, hebrewSheets);

                lastDate.EnglishSheets = englishSheets.Result;
                lastDate.HebrewSheets = hebrewSheets.Result;

                // if there is no new data dont update the data base
                if (lastDate.Equals(oldDate))
                {
                    return;
                }

                // save all new data from the spredsheet to the DB
                await dal.SaveAllDataFromSpredsheetAsync();

                await SendRegesterEmailForNewStudent();

                // update the last date of updating of the sheets
                dal.UpdateLastDateOfSheets(lastDate);
            }
            catch (Exception ex)
            {
                await sendEmail.Error(ex);
                throw new Exception("באג לא ידוע, פרטים על הבאג נשלחו למפתח");
            }
        }

        /// <summary>
        /// Update the data from the data base
        /// </summary>
        public async Task UpdateAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    matcher.MatchingTimes.Clear();
                    _studentList = new ObservableCollection<BO.Student>(CreateAllStudents());
                    BuildAllStudents();
                    _pairList = new ObservableCollection<BO.Pair>(GetAllPairs());
                });
            }
            catch (Exception ex)
            {
                await sendEmail.Error(ex);
                throw new Exception("באג לא ידוע, פרטים על הבאג נשלחו למפתח");
            }
        }
        #endregion

        #region Students
        public IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate)
        {
            return from s in _studentList
                   where predicate(s)
                   select s;
        }

        public BO.Student GetStudent(int id)
        {
            return _studentList.FirstOrDefault(s => s.Id == id);
        }

        public BO.Student GetStudent(Predicate<BO.Student> predicate)
        {
            return _studentList.FirstOrDefault(s => predicate(s));
        }

        public void SearchStudents(string preifxName)
        {
            StudentListFilter = s => s.Name.StartsWith(preifxName, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task RemoveStudentAsync(int id)
        {
            try
            {
                var studToRem = GetStudent(id);
                if (_studentList.Any(s => s.MatchTo.Contains(id)))
                {
                    var matchPairOfThisStud = studToRem.IsFromIsrael ?
                        _pairList
                        .ToList()
                        .FindAll(p => p.StudentFromIsraelId == id) 
                    :
                        _pairList
                        .ToList()
                        .FindAll(p => p.StudentFromWorldId == id);
                    if (matchPairOfThisStud != null && matchPairOfThisStud.Count > 0)
                    {
                        List<Task> tasks = new List<Task>();
                        foreach(var pairToRem in matchPairOfThisStud)
                        {
                            tasks.Add(RemovePairAsync(pairToRem));
                        }
                        await Task.WhenAll(tasks);
                    }
                }

                _studentList.Remove(studToRem);
                dal.RemoveStudent(id);
            }
            catch(DO.BadStudentException ex1)
            {
                new BO.BadStudentException(ex1.Message, id);
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message);
            }
        }

        public void AddStudent(BO.Student student)
        {
            int id = 0;
            try
            {
                var studDO = student.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                id = dal.AddStudent(studDO);
                student.Id = id;
                _studentList.Add(student);
            }
            catch (DO.BadStudentException ex1)
            {
                new BO.BadStudentException(ex1.Message, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStudent(BO.Student student)
        {
            try
            {
                var studDO = student.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                dal.UpdateStudent(studDO);
            }
            catch (DO.BadStudentException ex1)
            {
                new BO.BadStudentException(ex1.Message, student.Id);
            }
            catch (Exception ex)
            {
                new Exception(ex.Message);
            }
        }
        #endregion

        #region Pair matching

        public void UpdatePair(BO.Pair pair)
        {
            var pairDo = pair.CopyPropertiesToNew(typeof(DO.Pair)) as DO.Pair;

            try
            {
                var studFromIsrael = GetStudent(pair.StudentFromIsrael.Id);
                var studFromWorld = GetStudent(pair.StudentFromWorld.Id);

                var updateTrack = new Tuple<DateTime, DO.PrefferdTracks>(DateTime.Now, pair.PrefferdTracks);

                var matchHistFromIsrael = studFromIsrael
                    .MatchingHistories
                    .Find(mh => mh.MatchStudentId == studFromWorld.Id 
                    && !mh.IsUnMatch);
                if (matchHistFromIsrael != null && matchHistFromIsrael
                    .TracksHistory
                    .Find(t => t.Item2 == pair.PrefferdTracks) == null)
                {
                    matchHistFromIsrael
                        .TracksHistory.
                        Add(updateTrack);
                }

                var matchHistFromWorld = studFromWorld
                    .MatchingHistories
                    .Find(mh => mh.MatchStudentId == studFromIsrael.Id
                            && !mh.IsUnMatch);
                if (matchHistFromWorld != null && matchHistFromWorld
                    .TracksHistory
                    .Find(t => t.Item2 == pair.PrefferdTracks) == null)
                {
                    matchHistFromWorld
                        .TracksHistory.
                        Add(updateTrack);
                }

                UpdateStudent(studFromIsrael);
                UpdateStudent(studFromWorld);


                dal.UpdatePair(pairDo);
            }
            catch (DO.BadPairException)
            {
                throw new BO.BadPairException("חברותא לא נמצאה", pair.StudentFromIsrael.Name, pair.StudentFromWorld.Name);
            }
        }

        public async Task ActivatePairAsync(BO.Pair pair)
        {
            pair.IsActive = true;
            try
            {
                var studFromIsrael = GetStudent(pair.StudentFromIsrael.Id);
                var studFromWorld = GetStudent(pair.StudentFromWorld.Id);

                var matchHistFromIsrael = studFromIsrael
                    .MatchingHistories
                    .Find(mh => mh.MatchStudentId == studFromWorld.Id
                    && !mh.IsUnMatch);
                matchHistFromIsrael.IsActive = true;

                var matchHistFromWorld = studFromWorld
                    .MatchingHistories
                    .Find(mh => mh.MatchStudentId == studFromIsrael.Id
                    && !mh.IsUnMatch);
                matchHistFromWorld.IsActive = true;

                UpdateStudent(studFromIsrael);
                UpdateStudent(studFromWorld);
                UpdatePair(pair);
                await UpdateAsync();
                await SendEmailToPairAsync(pair, EmailTypes.YouGotPair);
                await SendEmailToPairAsync(pair, EmailTypes.ToSecretaryNewPair);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// making the match between the tow students
        /// </summary>
        /// <param name="first">student from israel</param>
        /// <param name="seconde">student from the world</param>
        public async Task<int> MatchAsync(BO.Student first, BO.Student seconde)
        {
            if(first.IsFromIsrael && seconde.IsFromIsrael)
            {
                throw new Exception("!אי אפשר לחבר בין שני משתתפים מישראל");
            }
            if(_pairList.Any(p => 
            (p.StudentFromIsraelId == first.Id) && (p.StudentFromWorldId == seconde.Id)
            || (p.StudentFromIsraelId == seconde.Id && p.StudentFromWorldId == first.Id)))
            {
                throw new BO.BadPairException($"החברותא {first.Name}, {seconde.Name} קיימת כבר", 
                    first.Name, seconde.Name);
            }
            try
            {
                first.MatchTo.Add(seconde.Id);
                seconde.MatchTo.Add(first.Id);

                var metchTrack = GetPrefferdTrackOfPair(first, seconde);

                first.MatchingHistories.Add(GetNewMatchingHistory(seconde, metchTrack));

                seconde.MatchingHistories.Add(GetNewMatchingHistory(first, metchTrack));

                var firstDo = first.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                var secondeDo = seconde.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                
                dal.UpdateStudent(firstDo);
                dal.UpdateStudent(secondeDo);

                DO.Pair pairToAdd = new DO.Pair 
                {
                    DateOfCreate = DateTime.Now,
                    PrefferdTracks = metchTrack,
                    IsDeleted = false
                };

                if (first.IsFromIsrael)
                {
                    pairToAdd.StudentFromIsraelId = first.Id;
                    pairToAdd.StudentFromWorldId = seconde.Id;
                }
                else
                {
                    pairToAdd.StudentFromIsraelId = seconde.Id;
                    pairToAdd.StudentFromWorldId = first.Id;
                }

                int id = dal.AddPair(pairToAdd);
                // update the students from the data base
                // after making one match we need to calculate evereting agien
                await UpdateAsync();
                return id;
            }
            catch (DO.BadPairException)
            {
                throw new BO.BadPairException(" החברותא כבר קיימת ", first.Name, seconde.Name);
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message);
            }
        }

        private DO.StudentMatchingHistory GetNewMatchingHistory(BO.Student secondeStudent, DO.PrefferdTracks track)
        {
            var matchHistoryFirst = new DO.StudentMatchingHistory
            {
                DateOfMatch = DateTime.Now,
                IsUnMatch = false,
                MatchStudentName = secondeStudent.Name,
                MatchStudentId = secondeStudent.Id
            };
            matchHistoryFirst
                .TracksHistory
                .Add(new Tuple<DateTime, DO.PrefferdTracks>(DateTime.Now, track));
            return matchHistoryFirst;
        }

        /// <summary>
        /// Get all pairs from the data base
        /// </summary>
        /// <returns>list of all pairs in a tuple of tow students</returns>
        public IEnumerable<BO.Pair> GetAllPairs()
        {
            return from p in dal.GetAllPairs()
                   select new BO.Pair()
                    .CreateFromDO(p, GetSimpleStudent);
        }

        public async Task RemovePairAsync(BO.Pair pair)
        {
            try
            {
                var studFromIsraelDO = dal.GetStudent(pair.StudentFromIsrael.Id);
                var studFromWorldDO = dal.GetStudent(pair.StudentFromWorld.Id);
                
                studFromIsraelDO.MatchTo.Remove(studFromWorldDO.Id);
                studFromWorldDO.MatchTo.Remove(studFromIsraelDO.Id);

                var matchHisIsrael = studFromIsraelDO.MatchingHistories
                    .Find(mh => mh.MatchStudentId == studFromWorldDO.Id
                    && !mh.IsUnMatch);
                matchHisIsrael.IsUnMatch = true;
                matchHisIsrael.DateOfUnMatch = DateTime.Now;

                var matchHisWorld = studFromWorldDO.MatchingHistories
                    .Find(mh => mh.MatchStudentId == studFromIsraelDO.Id
                    && !mh.IsUnMatch);
                matchHisWorld.IsUnMatch = true;
                matchHisWorld.DateOfUnMatch = DateTime.Now;

                

                dal.UpdateStudent(studFromIsraelDO);
                dal.UpdateStudent(studFromWorldDO);

                var pairDO = pair.CopyPropertiesToNew(typeof(DO.Pair)) as DO.Pair;
                if (pair.IsActive)
                {
                    await SendEmailToPairAsync(pair, EmailTypes.PairBroke);
                    await SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                }

                pairDO.IsActive = false;
                dal.RemovePair(pairDO);
                await UpdateAsync();
            }
            catch (DO.BadPairException)
            {
                throw new BO.BadPairException("חברותא לא קיימת", pair.StudentFromIsrael.Name, pair.StudentFromWorld.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FilterPairsByTrack(string track)
        {
            if (track == "הכל")
            {
                PairListFilter = p => !p.IsDeleted;
            }
            else
            {
                PairListFilter = p => p.PrefferdTracksShow == track;
            }
        }
        #endregion

        #region Sending emails
        async Task SendRegesterEmailForNewStudent()
        {
            foreach (var s in DataSource.StudentsList)
            {
                try
                {
                    if (s.Country == "Israel")
                    {
                        await sendEmail
                            .To(s.Email)
                            .SendAsync(s, Templates.SuccessfullyRegisteredHebrew);
                    }
                    else
                    {
                        await sendEmail
                            .To(s.Email)
                            .SendAsync(s, Templates.SuccessfullyRegisteredEnglish);
                    }
                }
                catch (FormatException)
                {
                    StudentWithUnvalidEmail.Add(new BO.SimpleStudent
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Email = s.Email
                    });
                    continue;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task SendEmailToPairAsync(BO.Pair pair, EmailTypes emailTypes)
        {
            try
            {
                switch (emailTypes)
                {
                    case EmailTypes.YouGotPair:
                        await sendEmail
                        .To(pair.StudentFromIsrael.Email)
                            .SendAsync(pair, Templates.YouGotPairHebrew);
                        
                        await sendEmail
                            .To(pair.StudentFromWorld.Email)
                            .SendAsync(pair, Templates.YouGotPairEnglish);
                        break;
                    
                    case EmailTypes.PairBroke:
                        await sendEmail
                            .To(pair.StudentFromIsrael.Email)
                            .SendAsync(pair, Templates.PairBrokeHebrew);

                        await sendEmail
                            .To(pair.StudentFromWorld.Email)
                            .SendAsync(pair, Templates.PairBrokeEnglish);
                        break;
                    case EmailTypes.ToSecretaryNewPair:
                        await sendEmail
                            .To(sendEmail.FromMail.Address)
                            .SendAsync(pair, Templates.ToSecretaryNewPair);
                        break;
                    case EmailTypes.ToSecretaryPairBroke:
                        await sendEmail
                            .To(sendEmail.FromMail.Address)
                            .SendAsync(pair, Templates.ToSecretaryPairBroke);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendEmailToStudentAsync(BO.Student student, EmailTypes emailTypes)
        {
            try
            {
                switch (emailTypes)
                {
                    case EmailTypes.SuccessfullyRegistered:
                        if (student.IsFromIsrael)
                        {
                            await sendEmail
                                .To(student.Email)
                                .SendAsync(student, Templates.SuccessfullyRegisteredHebrew);
                        }
                        else
                        {
                            await sendEmail
                                .To(student.Email)
                                .SendAsync(student, Templates.SuccessfullyRegisteredEnglish);
                        }
                        break;
                    case EmailTypes.StatusQuiz:
                        if (student.IsFromIsrael)
                        {
                            await sendEmail
                                .To(student.Email)
                                .SendAsync(student, Templates.StatusQuizHebrew);
                        }
                        else
                        {
                            await sendEmail
                                .To(student.Email)
                                .SendAsync(student, Templates.StatusQuizEnglish);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendOpenEmailAsync(string to, string subject, string body, string fileAttachment = "")
        {
            await sendEmail
                .To(to)
                .Subject(subject)
                .Template(new StringBuilder().Append(body))
                .SendOpenMailAsync(fileAttachment);
        }

        #endregion

        #region Build all the students and find a suggests students for each one
        private IEnumerable<BO.Student> CreateAllStudents()
        {
            var studentList = dal.GetAllStudents();
            return from s in studentList
                   let sBO = CreateStudent(s)
                   select sBO;
        }

        private BO.Student CreateStudent(DO.Student studDO)
        {
            try
            {
                // copy the propertis to BO student
                var studBO = studDO.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;

                if (studBO.IsSimpleStudent)
                {
                    return studBO;
                }
                
                // create a dictionary of the open Q&A for the student 
                studBO.OpenQuestionsDict = new Dictionary<string, string>();
                foreach (var o in studDO.OpenQuestions)
                {
                    studBO.OpenQuestionsDict.Add(o.Question, o.Answer.SpliceText(10));
                }

                foreach(var mh in studBO.MatchingHistories)
                {
                    studBO.MatchingHistoriesShow
                        .Add(mh.CopyPropertiesToNew(typeof(BO.StudentMatchingHistoryShow))
                        as BO.StudentMatchingHistoryShow);
                }

                return studBO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BuildAllStudents()
        {
            var temp = new List<BO.Student>();
            // find matcing student for each one
            foreach (var stud in _studentList)
            {
                stud.MatchToShow = string.Join(", ", from s in _studentList
                                                     where stud.MatchTo.Contains(s.Id)
                                                    select s.Name);
                if (stud.IsOpenToMatch)
                {
                    temp.Add(BuildStudent(stud));
                }
                else
                {
                    temp.Add(stud);
                }
            }
            _studentList = new ObservableCollection<BO.Student>(temp);
        }

        private BO.Student BuildStudent(BO.Student student)
        {
            // find matching students from first rank to this one
            // and order them by the number of matcing houers
            student.FirstSuggestStudents = from s in GetMatchingStudents(student, matcher.IsFirstMatching)
                                           orderby (from l in s.MatchingLearningTime
                                                    select l.TimeInDay.Count()).Count()
                                                    descending
                                           select s;

            // find matching students from seconde rank to this one
            // and order them by the number of matcing houers
            student.SecondeSuggestStudents = from s in GetMatchingStudents(student, matcher.IsMatchingStudentsCritical)
                                             orderby (from l in s.MatchingLearningTime
                                                      select l.TimeInDay.Count()).Count()
                                                    descending
                                             select s;

            return student;
        }

        private IEnumerable<BO.Student> GetOptionalStudents(BO.Student student)
        {
            if (student.IsFromIsrael)
            {
                return from s in _studentList
                       where !s.IsFromIsrael && s.IsOpenToMatch
                       && !student.IsInTheSuggestStudents(s)
                       select s;
            }
            else
            {
                return from s in _studentList
                       where s.IsFromIsrael && s.IsOpenToMatch
                       && !student.IsInTheSuggestStudents(s)
                       select s;
            }
        }


        private IEnumerable<BO.SuggestStudent> GetMatchingStudents(BO.Student student, Func<BO.Student, BO.Student, bool> func)
        {
            var result = new List<BO.SuggestStudent>();
            var studList = GetOptionalStudents(student);
            foreach (var other in studList)
            {
                if (student.IsFromIsrael)
                {
                    matcher.BuildMatch(student, other);
                    if (func(student, other))
                    {
                        var matchingLearningTime = from mt in matcher.MatchingTimes
                                                    where mt.StudentFromIsraelId == student.Id
                                                    && mt.StudentFromWorldId == other.Id
                                                    let lt = mt.MatchingLearningTimeInWorld
                                                    group lt.TimeInDay.First() by lt.Day into times
                                                    select new DO.LearningTime
                                                    {
                                                        Day = times.Key,
                                                        TimeInDay = times.Distinct()
                                                    };
                        var mat = new BO.SuggestStudent
                        {
                            ThisStudentId = student.Id,
                            SuggestStudentId = other.Id,
                            SuggestStudenCountry = other.Country,
                            SuggestStudentName = other.Name,
                            MatchingLearningTime = matchingLearningTime.ToList()
                        };
                        result.Add(mat);
                    }
                }
                else
                {
                    matcher.BuildMatch(other, student);
                    if (func(other, student))
                    {
                        var matchingLearningTime = from mt in matcher.MatchingTimes
                                                    where mt.StudentFromWorldId == student.Id
                                                    && mt.StudentFromIsraelId == other.Id
                                                    let lt = mt.MatchingLearningTimeInIsrael
                                                    group lt.TimeInDay.First() by lt.Day into times
                                                    select new DO.LearningTime
                                                    {
                                                        Day = times.Key,
                                                        TimeInDay = times.Distinct()
                                                    };

                        var mat = new BO.SuggestStudent
                        {
                            ThisStudentId = student.Id,
                            SuggestStudentId = other.Id,
                            SuggestStudenCountry = other.Country,
                            SuggestStudentName = other.Name,
                            MatchingLearningTime = matchingLearningTime.ToList()
                        };
                        result.Add(mat);
                    }
                }
            }

            return result;
        }
        #endregion

        #region Pairs helper functions
        private DO.PrefferdTracks GetPrefferdTrackOfPair(BO.Student firstStud, BO.Student secondeStud)
        {
            return firstStud.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER) ?
                secondeStud.PrefferdTracks.First() : firstStud.PrefferdTracks.First();
        }

        private BO.SimpleStudent GetSimpleStudent(int id)
        {
            var studDO = dal.GetStudent(id);
            var simpleStudent = studDO.CopyPropertiesToNew(typeof(BO.SimpleStudent)) as BO.SimpleStudent;
            return simpleStudent;
        }
        #endregion
    }
}
