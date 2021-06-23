using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace LogicLayer
{
    class LogicImplementaion : IBL
    {
        IDataLayer dal = DalFactory.GetDal("json");
        Matching match = new Matching();

        public static IBL Instance { get; } = new LogicImplementaion();

        public IEnumerable<BO.Student> StudentList { get; set; }  

        LogicImplementaion() {}

        public void UpdateData()
        {
            try
            {
                var lastDate = dal.GetLastDateOfSheets();
                if(lastDate == null)
                {
                    lastDate = new DO.LastDateOfSheets();
                }
                //lastDate.EnglishSheets = GoogleSheetParser.UpdateDataInEnglish(lastDate);
                lastDate.HebrewSheets = GoogleSheetParser.UpdateDataInHebrew(lastDate);
                dal.UpdateLastDateOfSheets(lastDate);
                StudentList = GetAllStudents();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Match(BO.Student fromIsreal, BO.Student fromWord)
        {
            try
            {
                fromIsreal.MatchTo = fromWord.Id;
                fromWord.MatchTo = fromIsreal.Id;
                
                var isSt = fromIsreal.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                var wrSt = fromWord.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                
                dal.UpdateStudent(isSt);
                dal.UpdateStudent(wrSt);
                
                dal.AddPair(new DO.Pair()
                {
                    FirstStudent = fromIsreal.Id,
                    SecondStudent = fromWord.Id,
                    IsDeleted = false
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int AddStudent(BO.Student student)
        {
            int id = 0;
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

        private IEnumerable<BO.Student> GetMatchingStudents(BO.Student student, Func<BO.Student, BO.Student, bool> func)
        {
            var result = new List<BO.Student>();
            if (student.Country == "Israel")
            {
                var studList = dal.GetAllStudentsBy(s => s.Country != "Israel");
                foreach (var studFromWord in studList)
                {
                    var studFromWordBo = studFromWord.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;
                    if (func(student, studFromWordBo))
                    {
                        result.Add(studFromWordBo);
                    }
                }
            }
            else
            {
                var studList = dal.GetAllStudentsBy(s => s.Country == "Israel");
                foreach (var studFromIsreal in studList)
                {
                    var studFromIsrealBo = studFromIsreal.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;
                    if (func(studFromIsrealBo, student))
                    {
                        result.Add(studFromIsrealBo);
                    }
                }
            }

            return result;
        }

        public IEnumerable<BO.Student> GetAllStudents()
        {
            return from sDO in dal.GetAllStudents()
                   let sBO = GetStudent(sDO.Id)
                   select sBO;
        }

        public IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate)
        {
            return from sDO in dal.GetAllStudents()
                   let sBO = GetStudent(sDO.Id)
                   where predicate(sBO)
                   select sBO;
        }

        public BO.Student GetStudent(int id)
        {
            try
            {
                var studDO = dal.GetStudent(id);
                var studBO = studDO.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;
                
                return BuildStudent(studBO);
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

        BO.Student BuildStudent(BO.Student student)
        {
            student.DesiredLearningTime = dal.GetAllLearningTimesBy(l => l.Id == student.Id);
            student.FirstMatchingStudents = GetMatchingStudents(student, match.IsFirstMatching);
            student.SecondeMatchingStudents = GetMatchingStudents(student, match.IsMatchingStudentsCritical);
            return student;
        }

    }
}
