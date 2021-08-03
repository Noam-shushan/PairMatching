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
        private readonly IDataLayer dal = DalFactory.GetDal("json");

        /// <summary>
        /// maching object to cheack all the condition to make a match
        /// </summary>
        private Matching Matcher { get; } = new Matching();

        SendEmail sendEmail = new SendEmail();

        /// <summary>
        /// the students list that keep all the data of the students
        /// </summary>
        public ObservableCollection<BO.Student> StudentList { get; set; } = 
            new ObservableCollection<BO.Student>();


        #region Singelton referens of the logic layer
        public static IBL Instance { get; } = new LogicImplementaion();

        private LogicImplementaion() { }
        #endregion

        #region Updating data from the Google Sheets and from the data base
        /// <summary>
        /// Update all the data from the google sheets
        /// </summary>
        public async Task UpdateDataAsync()
        {
            try
            {
                // get the last date of the update of the sheets to read from there
                var lastDate = dal.GetLastDateOfSheets();
                if (lastDate == null)
                {
                    lastDate = new DO.LastDateOfSheets();
                }
                var oldDate = lastDate.CopyPropertiesToNew(typeof(DO.LastDateOfSheets)) as DO.LastDateOfSheets;

                // create parser for the spradsheets
                GoogleSheetParser parser = new GoogleSheetParser();
                // read the english sheet
                lastDate.EnglishSheets = parser.UpdateDataInEnglish(lastDate);
                // read the hebrew sheet
                lastDate.HebrewSheets = parser.UpdateDataInHebrew(lastDate);

                // if there is no new data dont update the data base
                if (lastDate.Equals(oldDate))
                {
                    return;
                }

                // update the last date of updating of the sheets
                dal.UpdateLastDateOfSheets(lastDate);
                // update my StudentList with the new students and the new matching
                await UpdateAsync();
            }
            catch (Exception ex)
            {
                await sendEmail.Error(ex);
            }
        }

        /// <summary>
        /// Update the data from the data base
        /// </summary>
        public async Task UpdateAsync()
        {
            await Task.Run(() =>
            {
                Matcher.MatchingHoursList.Clear();
                StudentList = new ObservableCollection<BO.Student>(CreateAllStudents());
                BuildAllStudents();
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

        public IEnumerable<BO.Student> SearchStudents(string preifxName)
        {
            return from s in StudentList
                   where s.Name.StartsWith(preifxName, StringComparison.InvariantCultureIgnoreCase)
                   select s;
        }

        public void RemoveStudent(int id)
        {
            try
            {
                dal.RemoveStudent(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        } 
        #endregion

        #region Pair matching
        /// <summary>
        /// making the match between the tow students
        /// </summary>
        /// <param name="fromIsrael">student from israel</param>
        /// <param name="fromWorld">student from the world</param>
        public async Task MatchAsync(BO.Student fromIsrael, BO.Student fromWorld)
        {
            try
            {
                fromIsrael.MatchTo = fromWorld.Id;
                fromWorld.MatchTo = fromIsrael.Id;

                var fromIsrealDo = fromIsrael.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                fromIsrealDo.PrefferdTracks = fromIsrael.PrefferdTracks;
                var fromWorldDo = fromWorld.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                fromWorldDo.PrefferdTracks = fromWorld.PrefferdTracks;
                dal.UpdateStudent(fromIsrealDo);
                dal.UpdateStudent(fromWorldDo);

                dal.AddPair(new DO.Pair()
                {
                    FirstStudent = fromIsrael.Id,
                    SecondStudent = fromWorld.Id,
                    IsDeleted = false
                });
                // update the students from the data base
                // after making one match we need to calculate evereting agien
                await UpdateAsync();
            }
            catch (DO.BadPairException)
            {
                throw new BO.BadPairException(" החברותא כבר קיימת ", fromIsrael.Name, fromWorld.Name);
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
                   let pt = GetPrefferdTrackOfPair(p)
                   select new BO.Pair
                   {
                       FirstStudent = GetSimpleStudent(p.FirstStudent),
                       SecondStudent = GetSimpleStudent(p.SecondStudent),
                       PrefferdTracks = pt
                   };
        }

        public async Task RemovePairAsync(BO.Pair pair)
        {
            try
            {
                var firstStudDO = dal.GetStudent(pair.FirstStudent.Id);
                var secondeStudDO = dal.GetStudent(pair.SecondStudent.Id);
                firstStudDO.MatchTo = 0;
                secondeStudDO.MatchTo = 0;
                dal.UpdateStudent(firstStudDO);
                dal.UpdateStudent(secondeStudDO);
                dal.RemovePair(pair.FirstStudent.Id, pair.SecondStudent.Id);
                await UpdateAsync();
            }
            catch (DO.BadPairException)
            {
                throw new BO.BadPairException("חברותא לא קיימת", pair.FirstStudent.Name, pair.SecondStudent.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Build all the students and find a suggests students for each one
        private IEnumerable<BO.Student> CreateAllStudents()
        {
            return from id in Enumerable.Range(1, dal.GetCounters().StudentCounter)
                   let sBO = CreateStudent(id)
                   select sBO;
        }

        private BO.Student CreateStudent(int id)
        {
            try
            {
                // get the student from the data base
                var studDO = dal.GetStudent(id);

                // copy the propertis to BO student
                var studBO = studDO.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;

                // copy the enumrable
                studBO.PrefferdTracks = studDO.PrefferdTracks.Distinct();

                // get the learning time of the student from the data base
                studBO.DesiredLearningTime = dal.GetAllLearningTimesBy(l => l.Id == id);

                // get the open question and answers of the student from the data base
                var openQuestions = dal.GetAllOpenQuestionsBy(o => o.Id == id);

                // create a dictionary of the open Q&A for the student 
                studBO.OpenQuestions = new Dictionary<string, string>();
                foreach (var o in openQuestions)
                {
                    studBO.OpenQuestions.Add(o.Question, o.Answer);
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
                temp.Add(BuildStudent(s));
            }
            StudentList = new ObservableCollection<BO.Student>(temp);
        }

        private BO.Student BuildStudent(BO.Student student)
        {
            // find matching students from first rank to this one
            // and order them by the number of matcing houers
            student.FirstSuggestStudents = from s in GetMatchingStudents(student, Matcher.IsFirstMatching)
                                           orderby (from l in s.MatchingLearningTime
                                                    select l.TimeInDay.Count()).Count()
                                                    descending
                                           select s;

            // find matching students from seconde rank to this one
            // and order them by the number of matcing houers
            student.SecondeSuggestStudents = from s in GetMatchingStudents(student, Matcher.IsMatchingStudentsCritical)
                                             orderby (from l in s.MatchingLearningTime
                                                      select l.TimeInDay.Count()).Count()
                                                    descending
                                             select s;

            return student;
        }

        private IEnumerable<BO.SuggestStudent> GetMatchingStudents(BO.Student student, Func<BO.Student, BO.Student, bool> func)
        {
            var result = new List<BO.SuggestStudent>();
            if (student.Country == "Israel")
            {
                var studList = from s in StudentList
                               where s.Country != "Israel" && s.MatchTo == 0
                               select s;
                foreach (var studFromWorld in studList)
                {
                    if(IsInTheSuggestStudents(student, studFromWorld))
                    {
                        continue;
                    }
                    if (func(student, studFromWorld))
                    {
                        // get the matchin hours between the tow students
                        var matchingLearningTime = from m in Matcher.MatchingHoursList
                                                   where m.Item1.Id == studFromWorld.Id
                                                   && m.Item2.Id == student.Id
                                                   group m.Item1.TimeInDay.First() by m.Item1.Day into times
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
                               where s.Country == "Israel" && s.MatchTo == 0
                               select s;
                foreach (var studFromIsreal in studList)
                {
                    if (IsInTheSuggestStudents(student, studFromIsreal))
                    {
                        continue;
                    }
                    if (func(studFromIsreal, student))
                    {
                        // get the matchin hours between the tow students
                        var matchingLearningTime = from m in Matcher.MatchingHoursList
                                                   where m.Item1.Id == student.Id
                                                   && m.Item2.Id == studFromIsreal.Id
                                                   group m.Item2.TimeInDay.First() by m.Item2.Day into times
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

        private bool IsInTheSuggestStudents(BO.Student student, BO.Student suggestStudent)
        {
            if(student.FirstSuggestStudents != null 
                && student.FirstSuggestStudents.Any(s => s.SuggestStudentId == suggestStudent.Id))
            {
                return true;
            }

            if (student.SecondeSuggestStudents != null
                && student.SecondeSuggestStudents.Any(s => s.SuggestStudentId == suggestStudent.Id))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Pairs helper functions
        private IEnumerable<DO.PrefferdTracks> GetPrefferdTrackOfPair(DO.Pair p)
        {
            var firstStudDO = dal.GetStudent(p.FirstStudent);
            var secondeStudDO = dal.GetStudent(p.SecondStudent);
            return firstStudDO.PrefferdTracks.Contains(DO.PrefferdTracks.DONT_MATTER) ?
                secondeStudDO.PrefferdTracks : firstStudDO.PrefferdTracks;
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
