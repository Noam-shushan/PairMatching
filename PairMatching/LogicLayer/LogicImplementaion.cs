using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace LogicLayer
{
    /// <summary>
    /// Logic implemention of the logic layer main inteface
    /// </summary>
    internal class LogicImplementaion : IBL
    {
        // tha singelton of the data layer 
        private readonly IDataLayer dal = DalFactory.GetDal();

        /// <summary>
        /// maching object to cheack all the condition to make a match
        /// </summary>
        private Matching matcher = new Matching();

        private readonly SendEmail sendEmail = new SendEmail();

        /// <summary>
        /// the students list that keep all the data of the students
        /// </summary>
        public ObservableCollection<BO.Student> StudentList { get; set; } = 
            new ObservableCollection<BO.Student>();

        /// <summary>
        /// the students list that keep all the data of the students
        /// </summary>
        public ObservableCollection<BO.Pair> PairList { get; set; } =
            new ObservableCollection<BO.Pair>();


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

                // update the last date of updating of the sheets
                dal.UpdateLastDateOfSheets(lastDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //await sendEmail.Error(ex);
            }
        }

        /// <summary>
        /// Update the data from the data base
        /// </summary>
        public async Task UpdateAsync()
        {
            await Task.Run(() =>
            {
                matcher.MatchingTimes.Clear();
                StudentList = new ObservableCollection<BO.Student>(CreateAllStudents());
                BuildAllStudents();
                PairList = new ObservableCollection<BO.Pair>(GetAllPairs());
            });
        }
        #endregion

        #region Students
        public IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate)
        {
            return from s in StudentList
                   where predicate(s)
                   select s;
        }

        public BO.Student GetStudent(int id)
        {
            return StudentList.FirstOrDefault(s => s.Id == id);
        }

        public BO.Student GetStudent(Predicate<BO.Student> predicate)
        {
            return StudentList.FirstOrDefault(s => predicate(s));
        }

        public IEnumerable<BO.Student> SearchStudents(string preifxName)
        {
            return from s in StudentList
                   where s.Name.StartsWith(preifxName, StringComparison.InvariantCultureIgnoreCase)
                   select s;
        }

        public async Task RemoveStudentAsync(int id)
        {
            try
            {
                if(StudentList.Any(s => s.MatchTo.Contains(id)))
                {
                    var matchPairOfThisStud = GetStudent(id);
                    if (matchPairOfThisStud != null)
                    {
                        DO.Pair pairToRem;
                        if (matchPairOfThisStud.IsFromIsrael)
                        {
                            pairToRem = dal.GetPair(matchPairOfThisStud.Id, id);
                        }
                        else
                        {
                            pairToRem = dal.GetPair(id, matchPairOfThisStud.Id);
                        }
                        dal.RemovePair(pairToRem);
                        await UpdateAsync();
                    }
                }

                StudentList.Remove(GetStudent(id));
                dal.RemoveStudent(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddStudent(BO.Student student)
        {
            try
            {
                StudentList.Add(student);
                var studDO = student.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                dal.AddStudent(studDO);
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
                dal.UpdatePair(pairDo);
            }
            catch (DO.BadPairException)
            {
                throw new BO.BadPairException("חברותא לא נמצאה", pair.StudentFromIsrael.Name, pair.StudentFromWorld.Name);
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
            try
            {
                first.MatchTo.Add(seconde.Id);
                seconde.MatchTo.Add(first.Id);

                var firstDo = first.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                var secondeDo = seconde.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                
                dal.UpdateStudent(firstDo);
                dal.UpdateStudent(secondeDo);

                DO.Pair pairToAdd = new DO.Pair 
                {
                    DateOfCreate = DateTime.Now,
                    PrefferdTracks = GetPrefferdTrackOfPair(firstDo, secondeDo),
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
                dal.UpdateStudent(studFromIsraelDO);
                dal.UpdateStudent(studFromWorldDO);

                var pairDO = pair.CopyPropertiesToNew(typeof(DO.Pair)) as DO.Pair;
                
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

        public IEnumerable<BO.Pair> FilterPairsByTrack(string track)
        {
            return track == "הכל" ? PairList :
                from p in PairList
                where p.PrefferdTracksShow == track
                select p;
        }
        #endregion

        #region Sending emails
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
                   let sBO = CreateStudent(s, studentList)
                   select sBO;
        }

        private BO.Student CreateStudent(DO.Student studDO, IEnumerable<DO.Student> studentsList)
        {
            try
            {
                // copy the propertis to BO student
                var studBO = studDO.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;
                
                studBO.MatchToShow = string.Join(", ", from s in studentsList
                                                       where studBO.MatchTo.Contains(s.Id)
                                                       select s.Name);

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
            // find matcing student for each one
            foreach (var s in StudentList)
            {
                if (s.IsOpenToMatch)
                {
                    temp.Add(BuildStudent(s));
                }
                else
                {
                    temp.Add(s);
                }
            }
            StudentList = new ObservableCollection<BO.Student>(temp);
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

        private IEnumerable<BO.SuggestStudent> GetMatchingStudents(BO.Student student, Func<BO.Student, BO.Student, bool> func)
        {
            var result = new List<BO.SuggestStudent>();
            if (student.IsFromIsrael)
            {
                var studList = from s in StudentList
                               where !s.IsFromIsrael && s.IsOpenToMatch 
                               select s;
                foreach (var studFromWorld in studList)
                {
                    if(student.IsInTheSuggestStudents(studFromWorld))
                    {
                        continue;
                    }
                    if (func(student, studFromWorld))
                    {
                        var matchingLearningTime = from mt in matcher.MatchingTimes
                                                   where mt.StudentFromIsraelId == student.Id
                                                   && mt.StudentFromWorldId == studFromWorld.Id
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
                            SuggestStudentId = studFromWorld.Id,
                            SuggestStudenCountry = studFromWorld.Country,
                            SuggestStudentName = studFromWorld.Name,
                            MatchingLearningTime = matchingLearningTime
                        };
                        result.Add(mat);
                    }
                }
            }
            else
            {
                var studList = from s in StudentList
                               where s.IsFromIsrael && s.IsOpenToMatch
                               select s;
                foreach (var studFromIsreal in studList)
                {
                    if (student.IsInTheSuggestStudents(studFromIsreal))
                    {
                        continue;
                    }
                    if (func(studFromIsreal, student))
                    {
                        var matchingLearningTime = from mt in matcher.MatchingTimes
                                                   where mt.StudentFromWorldId == student.Id
                                                   && mt.StudentFromIsraelId == studFromIsreal.Id
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
                            SuggestStudentId = studFromIsreal.Id,
                            SuggestStudenCountry = studFromIsreal.Country,
                            SuggestStudentName = studFromIsreal.Name,
                            MatchingLearningTime = matchingLearningTime
                        };
                        result.Add(mat);
                    }
                }
            }

            return result;
        }
        #endregion

        #region Pairs helper functions
        private IEnumerable<DO.PrefferdTracks> GetPrefferdTrackOfPair(DO.Student firstStud, DO.Student secondeStud)
        {
            return firstStud.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER) ?
                secondeStud.PrefferdTracks : firstStud.PrefferdTracks;
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
