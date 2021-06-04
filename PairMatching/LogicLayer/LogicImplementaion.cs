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

        LogicImplementaion()
        {
            StudentList = GetAllStudents();
        }

        public void UpdateData()
        {
            StudentList = GetAllStudents();
        }

        public void AddStudent(BO.Student student)
        {
            student.FirstMatchingStudents = getFirstMatchingStudents(student);
            student.SecondeMatchingStudents = getSecondeMatchingStudents(student);
            try
            {
                foreach (var mPair in student.FirstMatchingStudents)
                {
                    var pair = new DO.Pair
                    {
                        FirstStudent = student.Id,
                        SecondStudent = mPair.Id,
                        IsDeleted = false,
                        MatchingDegree = DO.MatchingDegrees.FIRST
                    };
                    dal.AddPair(pair);
                }

                foreach (var mPair in student.SecondeMatchingStudents)
                {
                    var pair = new DO.Pair
                    {
                        FirstStudent = student.Id,
                        SecondStudent = mPair.Id,
                        IsDeleted = false,
                        MatchingDegree = DO.MatchingDegrees.SECONDE
                    };
                    dal.AddPair(pair);
                }
                var studDO = student.CopyPropertiesToNew(typeof(DO.Student)) as DO.Student;
                dal.AddStudent(studDO);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private IEnumerable<BO.Student> getFirstMatchingStudents(BO.Student student)
        {
            var studList = GetAllStudents();
            var result = new List<BO.Student>();
            foreach (var stud in studList)
            {
                if(match.IsFirstMatching(student, stud))
                {
                    result.Add(stud);
                } 
            }
            return result;
        }

        private IEnumerable<BO.Student> getSecondeMatchingStudents(BO.Student student)
        {
            var studList = GetAllStudents();
            var result = new List<BO.Student>();
            foreach (var stud in studList)
            {
                if (match.IsMatchingStudentsCritical(student, stud))
                {
                    result.Add(stud);
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
                var studDO = dal.GetAllStudentsBy(s => s.Id == id).FirstOrDefault();
                var studBO = studDO.CopyPropertiesToNew(typeof(BO.Student)) as BO.Student;
                
                studBO.FirstMatchingStudents = getFirstMatchingStudents(studBO);
                studBO.SecondeMatchingStudents = getSecondeMatchingStudents(studBO);
                
                return studBO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveStudent(int id)
        {
            throw new NotImplementedException();
        }

    }
}
