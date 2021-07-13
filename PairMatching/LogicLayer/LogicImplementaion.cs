using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace LogicLayer
{
    class LogicImplementaion : IBL
    {
        readonly IDataLayer dal = DalFactory.GetDal("json");
        readonly Matching match = new Matching();

        public static IBL Instance { get; } = new LogicImplementaion();

        public ObservableCollection<BO.Student> StudentList { get; set; } = new ObservableCollection<BO.Student>();

        LogicImplementaion()
        {
            
        }

        public async Task UpdateData()
        {
            try
            {
                var lastDate = dal.GetLastDateOfSheets();
                if(lastDate == null)
                {
                    lastDate = new DO.LastDateOfSheets();
                }
                GoogleSheetParser parser = new GoogleSheetParser();
                lastDate.EnglishSheets = parser.UpdateDataInEnglish(lastDate);
                lastDate.HebrewSheets = parser.UpdateDataInHebrew(lastDate);
                dal.UpdateLastDateOfSheets(lastDate);
                await Update();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Update()
        {
            await Task.Run(() =>
            {
                StudentList = new ObservableCollection<BO.Student>(GetAllStudents());
                BuildStudents();
            });
        }

        public async Task Match(BO.Student fromIsreal, BO.Student fromWorld)
        {
            try
            {
                fromIsreal.MatchTo = fromWorld.Id;
                fromWorld.MatchTo = fromIsreal.Id;
                
                var fromIsrealDo = fromIsreal.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                fromIsrealDo.PrefferdTracks = fromIsreal.PrefferdTracks;
                var fromWorldDo = fromWorld.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                fromWorldDo.PrefferdTracks = fromWorld.PrefferdTracks;
                dal.UpdateStudent(fromIsrealDo);
                dal.UpdateStudent(fromWorldDo);
                
                dal.AddPair(new DO.Pair()
                {
                    FirstStudent = fromIsreal.Id,
                    SecondStudent = fromWorld.Id,
                    IsDeleted = false
                });
                // update the students from the data base
                await Update();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int AddStudent(BO.Student student)
        {
            int id;
            try
            {
                var studDO = student.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                id = dal.AddStudent(studDO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return id;
        }

        public IEnumerable<BO.Student> GetAllStudents()
        {
            return from id in Enumerable.Range(1, dal.GetCounters().StudentCounter)
                   let sBO = GetStudent(id)
                   select sBO;
        }

        public IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate)
        {
            return from id in Enumerable.Range(1, dal.GetCounters().StudentCounter)
                   let sBO = GetStudent(id)
                   where predicate(sBO)
                   select sBO;
        }

        public BO.Student GetStudent(int id)
        {
            try
            {
                var studDO = dal.GetStudent(id);
                var studBO = studDO.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;
               
                studBO.PrefferdTracks = studDO.PrefferdTracks.Distinct();
                
                studBO.DesiredLearningTime = dal.GetAllLearningTimesBy(l => l.Id == id);

                var openQuestions = dal.GetAllOpenQuestionsBy(o => o.Id == id);
                studBO.OpenQuestions = new Dictionary<string, string>();
                foreach(var o in openQuestions)
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

        void BuildStudents()
        {
            var temp = new List<BO.Student>();
            foreach(var s in StudentList)
            {
                temp.Add(BuildStudent(s));
            }
            StudentList = new ObservableCollection<BO.Student>(temp);
        }

        BO.Student BuildStudent(BO.Student student)
        {
            student.FirstMatchingStudents = GetMatchingStudents(student, match.IsFirstMatching);
            student.SecondeMatchingStudents = GetMatchingStudents(student, match.IsMatchingStudentsCritical);
            student.SecondeMatchingStudents = student.SecondeMatchingStudents.Except(student.FirstMatchingStudents);
            student.SecondeMatchingStudents.Count();
            return student;
        }

        private IEnumerable<BO.Student> GetMatchingStudents(BO.Student student, Func<BO.Student, BO.Student, bool> func)
        {
            var result = new List<BO.Student>();
            if (student.Country == "Israel")
            {
                var studList = from s in StudentList
                               where s.Country != "Israel" && s.MatchTo == 0
                               select s;
                foreach (var studFromWorld in studList)
                {
                    if (func(student, studFromWorld))
                    {
                        result.Add(studFromWorld);
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
                    if (func(studFromIsreal, student))
                    {
                        result.Add(studFromIsreal);
                    }
                }
            }

            return result;
        }

        public IEnumerable<Tuple<BO.Student, BO.Student>> GetAllPairs()
        {
            return from p in dal.GetAllPairs()
                   let s1 = GetStudent(p.FirstStudent)
                   let s2 = GetStudent(p.SecondStudent)
                   select new Tuple<BO.Student, BO.Student>(s1, s2);
        }
    }
}
