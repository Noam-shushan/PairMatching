using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using UtilEntities;
using LogicLayer.Email;
using LogicLayer.FindMatching;
using LogicLayer.GoogleSheet;

namespace LogicLayer
{
    /// <summary>
    /// Logic implemention of the logic layer main inteface
    /// </summary>
    internal class LogicImplementaion : ILogicLayer, INotifyPropertyChanged
    {
        // tha singelton of the data layer 
        private readonly IDataLayer dal;

        // maching object to cheack all the condition to make a match
        private readonly Matching matcher = Matching.Instance;

        // Email sender
        private readonly SendEmail emailSender = SendEmail.Instance;

        #region Properties
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

        private BO.Statistics _statistics = new BO.Statistics();
        public BO.Statistics Statistics
        {
            get
            {
                _statistics.NumberOfPair.Value = _pairList.Count;
                _statistics.NumberOfStudentFromIsrael.Value = _studentList
                                                                .Where(s => s.IsFromIsrael)
                                                                .Count();
                _statistics.NumberOfStudents.Value = _studentList.Count;
                _statistics.NumberOfStudentsWithoutPair.Value = _studentList
                                                                .Where(s => !s.IsMatch)
                                                                .Count();
                _statistics.NumberOfStudentFromWorldWithoutPair.Value = _studentList
                                                                .Where(s => !s.IsMatch && !s.IsFromIsrael)
                                                                .Count();
                _statistics.NumberOfStudentFromIsraelWithoutPair.Value = _studentList
                                                                        .Where(s => !s.IsMatch && s.IsFromIsrael)
                                                                        .Count();
                _statistics.NumberOfStudentFromWorld.Value = _studentList
                    .Where(s => !s.IsFromIsrael)
                    .Count();
                _statistics.NumberOfActivePairs.Value = _pairList
                    .Where(p => p.IsActive)
                    .Count();
                return _statistics;
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
                if (PairListFilter != BasePairsFilter)
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
        #endregion

        #region Singleton referens of the logic layer
        //private static ILogicLayer _instance;

        //private static readonly object _loke = new object();

        public static ILogicLayer Instance { get; } = new LogicImplementaion();
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            lock (_loke)
        //            {
        //                if (_instance == null)
        //                {
        //                    _instance = new LogicImplementaion();
        //                }
        //            }
        //        }
        //        return _instance;
        //    }
        //}

        private LogicImplementaion() 
        {
            dal = DalFactory.GetDal();
        }
        #endregion

        #region Statistics
        public List<BO.Bar> GetStatistics()
        {
            var listOfBars = new List<BO.Bar>();
            var propList = Statistics.GetType().GetProperties();
            foreach (var prop in propList)
            {
                var val = prop.GetValue(_statistics);
                listOfBars.Add(val as BO.Bar);
            }
            return listOfBars;
        } 
        #endregion

        #region Data reading and updating   
        /// <summary>
        /// Read the new data of student from the google sheets
        /// Seaving all the data to the database.
        /// Sending automatic emails to the new studnts
        /// </summary>
        public async Task<bool> ReadDataFromSpredsheetAsync()
        {
            DO.SpredsheetLastRange lastRange, oldRange;
            try
            {
                // get the last date of the update of the sheets to read from there
                lastRange = dal.GetSheetsLastRang();
                oldRange = new DO.SpredsheetLastRange
                {
                    EnglishSheets = lastRange.EnglishSheets,
                    HebrewSheets = lastRange.HebrewSheets
                };
            }
            catch (Exception ex1)
            {
                await emailSender.Error(ex1);
                throw new Exception("בעיה בתוכנה\n. נסה ללחוץ על עדכן נתונים");
            }
            try
            {
                // create parser for the spradsheets
                GoogleSheetParser parser = new GoogleSheetParser();
                // read the english sheet
                var englishSheets = parser.ReadAsync(new EnglishDiscriptor(lastRange));
                // read the hebrew sheet
                var hebrewSheets = parser.ReadAsync(new HebrewDescriptor(lastRange));

                await Task.WhenAll(englishSheets, hebrewSheets);

                lastRange.EnglishSheets = englishSheets.Result;
                lastRange.HebrewSheets = hebrewSheets.Result;

                // if there is no new data dont update the data base
                if (lastRange.Equals(oldRange))
                {
                    return false;
                }

                // save all new data from the spredsheet to the DB
                await dal.SaveAllDataFromSpredsheetAsync();

                // update the last date of updating of the sheets
                dal.UpdateSheetsLastRange(lastRange);

                // send email for all new students
                await SendRegesterEmailForNewStudent();
                
                return true;
            }
            catch (Exception ex)
            {
                await emailSender.Error(ex);
                throw new Exception("באג לא ידוע, פרטים על הבאג נשלחו למפתח");
            }
        }

        /// <summary>
        /// Update all the data from the database and find new matching students
        /// </summary>
        public async Task UpdateAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    _studentList = new ObservableCollection<BO.Student>(CreateAllStudents());
                    BuildAllStudents();
                    EmailAddressChecker.NotifyOnNotValidAddrress(_studentList.ToList());
                    _pairList = new ObservableCollection<BO.Pair>(GetAllPairs());
                });
            }
            catch (Exception ex)
            {
                await emailSender.Error(ex);
                throw new Exception("באג לא ידוע, פרטים על הבאג נשלחו למפתח");
            }
        }
        #endregion

        #region Students
        /// <summary>
        /// Get all the students by some predicate from the database
        /// </summary>
        /// <param name="predicate">condition on Student</param>
        /// <returns>all students thet is the condition</returns>
        public IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate)
        {
            return from s in _studentList
                   where predicate(s)
                   select s;
        }

        /// <summary>
        /// Get Student from the database
        /// </summary>
        /// <param name="id">the student id</param>
        /// <returns>the student match to the id</returns>
        public BO.Student GetStudent(int id)
        {
            return _studentList.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Get Student from the database
        /// </summary>
        /// <param name="predicate">predicate on the student propertiecs</param>
        /// <returns>the student that match to the condition</returns>
        public BO.Student GetStudent(Predicate<BO.Student> predicate)
        {
            return _studentList.FirstOrDefault(s => predicate(s));
        }

        /// <summary>
        /// Search students by prefix of thire name 
        /// </summary>
        /// <param name="preifxName">The prefix that cams from the Gui</param>
        public void SearchStudents(string subtext)
        {
            StudentListFilter = s => SearchText(s.Name, subtext);
        }

        /// <summary>
        /// Remove student from the database
        /// remove all the pairs of this student
        /// </summary>
        /// <param name="id">the student id</param>
        public async Task RemoveStudentAsync(int id)
        {
            try
            {
                var studToRem = GetStudent(id);
                await RemovePairFromStudent(studToRem);

                _studentList.Remove(studToRem);
                dal.RemoveStudent(id);
            }
            catch(DO.BadStudentException ex1)
            {
                throw new BO.BadStudentException(ex1.Message, id);
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message);
            }
        }

        private async Task RemovePairFromStudent(BO.Student student)
        {
            if (_studentList.Any(s => s.MatchTo.Contains(student.Id)))
            {
                var matchPairOfThisStud = _pairList
                    .Where(p => p.StudentFromIsraelId == student.Id
                    || p.StudentFromWorldId == student.Id)
                    .ToList();
                if (matchPairOfThisStud != null && matchPairOfThisStud.Count() > 0)
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var pairToRem in matchPairOfThisStud)
                    {
                        tasks.Add(RemovePairAsync(pairToRem.Id, false));
                    }
                    await Task.WhenAll(tasks);
                }
            }
        }

        /// <summary>
        /// Add student to the database
        /// </summary>
        /// <param name="student">The new student to add</param>
        public void AddStudent(BO.Student student)
        {
            int id = 0;
            try
            { 
                student.IsSimpleStudent = true;
                student.DateOfRegistered = DateTime.Now;
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

        /// <summary>
        /// Update student in the database
        /// </summary>
        /// <param name="student">The student to updata</param>
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

        /// <summary>
        /// Add note to student
        /// </summary>
        /// <param name="student">The student to add note to</param>
        /// <param name="note">The note to add to the student</param>
        public void AddNoteToStudent(BO.Student student, Note note)
        {
            student.Notes.Add(note);
            var s = dal.GetStudent(student.Id);
            s.Notes.Add(note);
            dal.UpdateStudent(s);
        }

        /// <summary>
        /// Remove note from student
        /// </summary>
        /// <param name="student">The student to remove the note from</param>
        /// <param name="note">The note to remove from the student</param>
        public void RemoveNoteFromStudent(BO.Student student, Note note)
        {
            student.Notes.Remove(note);
            var s = dal.GetStudent(student.Id);
            s.Notes.Remove(note);
            dal.UpdateStudent(s);
        }
        #endregion

        #region Pair matching
        /// <summary>
        /// Get pair from the pair list by predicte
        /// </summary>
        /// <param name="predicate">the predicate</param>
        /// <returns></returns>
        public BO.Pair GetPair(Predicate<BO.Pair> predicate)
        {
            return _pairList.FirstOrDefault(p => predicate(p));
        }

        /// <summary>
        /// Get pair from the pair list by id
        /// </summary>
        /// <param name="pairId">The pair id</param>
        /// <returns></returns>
        public BO.Pair GetPair(int pairId)
        {
            return _pairList.FirstOrDefault(p => p.Id == pairId);
        }

        /// <summary>
        /// Add note to pair
        /// </summary>
        /// <param name="pair">The pair to add note to</param>
        /// <param name="note">The note to add to the pair</param>
        public void AddNoteToPair(BO.Pair pair, Note note)
        {
            pair.Notes.Add(note);
            var p = dal.GetPair(pair.Id);
            p.Notes.Add(note);
            dal.UpdatePair(p);
        }

        /// <summary>
        /// Remove note from pair
        /// </summary>
        /// <param name="pair">The pair to remove note from</param>
        /// <param name="note">The note to remove from the pair</param>
        public void RemoveNoteFromPair(BO.Pair pair, Note note)
        {
            pair.Notes.Remove(note);
            var p = dal.GetPair(pair.Id);
            p.Notes.Remove(note);
            dal.UpdatePair(p);
        }

        public void UpdatePair(BO.Pair pair)
        {
            var pairDo = pair.CopyPropertiesToNew(typeof(DO.Pair)) as DO.Pair;

            try
            {
                var studFromIsrael = GetStudent(pair.StudentFromIsrael.Id);
                var studFromWorld = GetStudent(pair.StudentFromWorld.Id);

                var updateTrack = new Tuple<DateTime, PrefferdTracks>(DateTime.Now, pair.PrefferdTracks);

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

        /// <summary>
        /// Active pair from standby status to active pair.
        /// recalculating finding suggestions for matching
        /// </summary>
        /// <param name="pair">The pair to activete</param>
        /// <returns></returns>
        public async Task ActivatePairAsync(BO.Pair pair, bool sendEmail)
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

                if (sendEmail)
                {
                    await SendEmailToPairAsync(pair, EmailTypes.YouGotPair);
                    await SendEmailToPairAsync(pair, EmailTypes.ToSecretaryNewPair);
                }
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

        private StudentMatchingHistory GetNewMatchingHistory(BO.Student secondeStudent, PrefferdTracks track)
        {
            var matchHistoryFirst = new StudentMatchingHistory
            {
                DateOfMatch = DateTime.Now,
                IsUnMatch = false,
                MatchStudentName = secondeStudent.Name,
                MatchStudentId = secondeStudent.Id
            };
            matchHistoryFirst
                .TracksHistory
                .Add(new Tuple<DateTime, PrefferdTracks>(DateTime.Now, track));
            return matchHistoryFirst;
        }

        /// <summary>
        /// Get all pairs from the database
        /// </summary>
        /// <returns>list of all pairs</returns>
        public IEnumerable<BO.Pair> GetAllPairs()
        {
            var pairsList = dal.GetAllPairs();
            return from p in pairsList
                   select CreatePairFromDO(p, GetSimpleStudent);
        }

        public BO.Pair CreatePairFromDO(DO.Pair pairDo, Func<int, BO.SimpleStudent> createSimpleStudentFunc)
        {
            var res = pairDo.CopyPropertiesToNew(typeof(BO.Pair)) as BO.Pair;
            res.StudentFromIsrael = createSimpleStudentFunc(pairDo.StudentFromIsraelId);
            res.StudentFromWorld = createSimpleStudentFunc(pairDo.StudentFromWorldId);
            return res;
        }

        /// <summary>
        /// Remove pair from the database.
        /// after the remove recalculating finding suggestions for matching
        /// </summary>
        /// <param name="pair">The pair to renove</param>
        /// <returns></returns>
        public async Task RemovePairAsync(int pairId ,bool notifyByEmail)
        {
            var pair = GetPair(pairId);
            await RemoveOnePairAsync(pair, notifyByEmail);
        }

        private async Task RemoveOnePairAsync(BO.Pair pair, bool notifyByEmail)
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
                if (pair.IsActive && notifyByEmail)
                {
                    await SendEmailToPairAsync(pair, EmailTypes.PairBroke);
                    await SendEmailToPairAsync(pair, EmailTypes.ToSecretaryPairBroke);
                }

                pairDO.IsActive = false;
                _pairList.Remove(pair);
                dal.RemovePair(pairDO);
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

        /// <summary>
        /// Filter the pair list by track of stading
        /// </summary>
        /// <param name="track">the track stading</param>
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
        public async Task SendAutoEmailOnMatchAgain(string to, int pairId)
        {
            var pair = GetPair(pairId);
            if(pair.StudentFromIsrael.Email == to)
            {
                await SendEmailToPairAsync(pair, EmailTypes.YouGotPair, true, false);
            }
            if (pair.StudentFromWorld.Email == to)
            {
                await SendEmailToPairAsync(pair, EmailTypes.YouGotPair, false, true);
            }
        }

        /// <summary>
        /// Send email to new students that registerd in the google forms
        /// </summary>
        /// <returns></returns>
        private async Task SendRegesterEmailForNewStudent()
        {
            foreach (var s in DataSource.StudentsList)
            {
                try
                {
                    if (s.Country == "Israel")
                    {
                        await emailSender
                            .To(s.Email)
                            .SendAutoEmailAsync(s, Templates.SuccessfullyRegisteredHebrew);
                    }
                    else
                    {
                        await emailSender
                            .To(s.Email)
                            .SendAutoEmailAsync(s, Templates.SuccessfullyRegisteredEnglish);
                    }
                }
                catch (FormatException) // the email addres of the student is not valid
                {
                   continue;
                }
                catch (Exception ex)
                {
                    await emailSender.Error(ex);
                }
            }
        }

        /// <summary>
        /// Send automatic email to pair
        /// </summary>
        /// <param name="pair">The pair to send the email to</param>
        /// <param name="emailTypes">The email type [new pair, pair broke, ect...]</param>
        /// <returns></returns>
        public async Task SendEmailToPairAsync(BO.Pair pair, EmailTypes emailTypes, 
            bool onlyToIsreli = true, bool onlyToWorld = true)
        {
            try
            {
                switch (emailTypes)
                {
                    case EmailTypes.YouGotPair:
                        if (onlyToIsreli)
                        {
                            await emailSender
                                .To(pair.StudentFromIsrael.Email)
                                .SendAutoEmailAsync(pair, Templates.YouGotPairHebrew);
                        }

                        if (onlyToWorld)
                        {
                            await emailSender
                            .To(pair.StudentFromWorld.Email)
                            .SendAutoEmailAsync(pair, Templates.YouGotPairEnglish); 
                        }
                        break;
                    
                    case EmailTypes.PairBroke:
                        await emailSender
                            .To(pair.StudentFromIsrael.Email)
                            .SendAutoEmailAsync(pair, Templates.PairBrokeHebrew);

                        await emailSender
                            .To(pair.StudentFromWorld.Email)
                            .SendAutoEmailAsync(pair, Templates.PairBrokeEnglish);
                        break;
                    case EmailTypes.ToSecretaryNewPair:
                        await emailSender
                            .To(emailSender.SystemMail)
                            .SendAutoEmailAsync(pair, Templates.ToSecretaryNewPair);
                        break;
                    case EmailTypes.ToSecretaryPairBroke:
                        await emailSender
                            .To(emailSender.SystemMail)
                            .SendAutoEmailAsync(pair, Templates.ToSecretaryPairBroke);
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

        /// <summary>
        /// Send automatic email to studnet
        /// </summary>
        /// <param name="student">The student to send the email to</param>
        /// <param name="emailTypes">The email type [new rejester, status email, ect...]</param>
        /// <returns></returns>     
        public async Task SendEmailToStudentAsync(BO.Student student, EmailTypes emailTypes)
        {
            try
            {
                switch (emailTypes)
                {
                    case EmailTypes.SuccessfullyRegistered:
                        if (student.IsFromIsrael)
                        {
                            await emailSender
                                .To(student.Email)
                                .SendAutoEmailAsync(student, Templates.SuccessfullyRegisteredHebrew);
                        }
                        else
                        {
                            await emailSender
                                .To(student.Email)
                                .SendAutoEmailAsync(student, Templates.SuccessfullyRegisteredEnglish);
                        }
                        break;
                    case EmailTypes.StatusQuiz:
                        if (student.IsFromIsrael)
                        {
                            await emailSender
                                .To(student.Email)
                                .SendAutoEmailAsync(student, Templates.StatusQuizHebrew);
                        }
                        else
                        {
                            await emailSender
                                .To(student.Email)
                                .SendAutoEmailAsync(student, Templates.StatusQuizEnglish);
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

        /// <summary>
        ///  Send open email 
        /// </summary>
        /// <param name="to">Email addres to send</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="body">The body of the email</param>
        /// <param name="fileAttachment">File name to attach to the email</param>
        /// <returns></returns>
        public async Task SendOpenEmailAsync(IEnumerable<string> to, string subject, string body, IEnumerable<string> fileAttachments)
        {
            await emailSender
                .To(to.ToArray())
                .Subject(subject)
                .Template(new StringBuilder().Append(body))
                .SendOpenMailAsync(fileAttachments);
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

                foreach (var mh in studBO.MatchingHistories)
                {
                    studBO.MatchingHistoriesShow
                        .Add(mh.CopyPropertiesToNew(typeof(BO.StudentMatchingHistoryShow))
                        as BO.StudentMatchingHistoryShow);
                }

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
            // Find matcing student for each one
            foreach (var stud in _studentList)
            {
                // Get the names of the students that thy paired with
                stud.MatchToShow = from s in _studentList
                                   where stud.MatchTo.Contains(s.Id)
                                   select s;

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
            // Find matching students from first rank to this one
            student.FirstSuggestStudents = GetMatchingStudents(student, matcher.IsFirstMatching); 
            if (student.FirstSuggestStudents.Count() == 0)
            {   // If there isn't any matching students look for matching without compereing houers
                student.FirstSuggestStudents = GetMatchingStudents(student, matcher.IsFirstMatching, true);
            }

            // Find matching students from seconde rank to this one
            student.SecondeSuggestStudents = GetMatchingStudents(student, matcher.IsMatchingStudentsCritical);
            if (student.SecondeSuggestStudents.Count() == 0)
            {   // If there isn't any matching students look for matching without compereing houers
                student.SecondeSuggestStudents = GetMatchingStudents(student, matcher.IsMatchingStudentsCritical, true);
            }

            return student;
        }

        private IEnumerable<BO.SuggestStudent> GetMatchingStudents(BO.Student student, Func<BO.Student, BO.Student, bool, bool> findFunc, bool flagNotFound = false)
        {
            var result = new List<BO.SuggestStudent>();
            // Get optional students that is relevnt for finding matching
            var studList = GetOptionalStudents(student);
            foreach (var other in studList)
            {
                if (student.IsFromIsrael)
                {
                    // Create new suggest students
                    matcher.BuildMatch(student, other);
                    // Find and build the current suggest students
                    if (findFunc(student, other, flagNotFound))
                    {
                        result.Add(matcher.CurrentForIsraeliSuggest);
                    }
                }
                else
                {
                    // Create new suggest students
                    matcher.BuildMatch(other, student);
                    // Find and build the current suggest students
                    if (findFunc(other, student, flagNotFound))
                    {
                        result.Add(matcher.CurrentForWorldSuggest);
                    }
                }
            }

            // Return the suggest students list order by the matching score
            return GetOrderdSuggestStudents(result);
        }

        private IEnumerable<BO.SuggestStudent> GetOrderdSuggestStudents(IEnumerable<BO.SuggestStudent> suggestStudents)
        {
            return from s in suggestStudents
                   orderby s.MatcingScore descending
                   select s;
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
        #endregion

        #region Pairs helper functions
        private PrefferdTracks GetPrefferdTrackOfPair(BO.Student firstStud, BO.Student secondeStud)
        {
            var result = PrefferdTracks.DONT_MATTER;
            if (firstStud.PrefferdTracks.Count() == 0
                && secondeStud.PrefferdTracks.Count() != 0)
            {
                result = secondeStud.PrefferdTracks.FirstOrDefault(p => p != PrefferdTracks.DONT_MATTER);
            }
            else if(firstStud.PrefferdTracks.Count() != 0
                && secondeStud.PrefferdTracks.Count() == 0)
            {
                result = firstStud.PrefferdTracks.FirstOrDefault(p => p != PrefferdTracks.DONT_MATTER);
            }
            else if(firstStud.PrefferdTracks.Count() == 0
                && secondeStud.PrefferdTracks.Count() == 0)
            {
                return PrefferdTracks.DONT_MATTER;
            }
            else
            {
                result = firstStud.PrefferdTracks.Contains(PrefferdTracks.DONT_MATTER) ?
                    secondeStud.PrefferdTracks.FirstOrDefault(p => p != PrefferdTracks.DONT_MATTER)
                    : firstStud.PrefferdTracks.FirstOrDefault(p => p != PrefferdTracks.DONT_MATTER);
            }
            
            return result == default ? PrefferdTracks.DONT_MATTER : result;
        }

        private BO.SimpleStudent GetSimpleStudent(int id)
        {
            var stud = GetStudent(id);
            var simpleStudent = stud.CopyPropertiesToNew(typeof(BO.SimpleStudent)) as BO.SimpleStudent;
            return simpleStudent;
        }

        public void SearchPairs(string text)
        {
            PairListFilter = p =>
            SearchText(p.StudentFromIsrael.Name, text) ||
                SearchText(p.StudentFromWorld.Name, text);
        }

        private bool SearchText(string text, string subtext)
        {
            return text.ToLower().Contains(subtext.ToLower());
        }
        #endregion
    }
}
