using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilEntities;

namespace LogicLayer.Domain
{
    public class StudentsDomain
    {
        private readonly IDataLayer dal;

        private List<BO.Student> _studentList;

        private List<int> studentIdsToRemove;

        public List<BO.Student> Students
        {
            get { return _studentList; }
            set { _studentList = value; }
        }


        public StudentsDomain()
        {
            dal = DalFactory.GetDal();
            studentIdsToRemove = new List<int>();
        }

        /// <summary>
        /// Get all the students by some predicate from the database
        /// </summary>
        /// <param name="predicate">condition on Student</param>
        /// <returns>all students that is the in the condition</returns>
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
            var result = _studentList.FirstOrDefault(s => s.Id == id);
            if(result != null)
            {
                return result;
            }
            var studDO = dal.GetStudent(id);
            if(studDO != null)
            {
                studDO.CopyPropertiesTo(result);
                return result;
            }
            throw new Exception("משתתף זה לא נמצא");
        }

        /// <summary>
        /// Get Student from the database
        /// </summary>
        /// <param name="predicate">predicate on the student properties</param>
        /// <returns>the student that match to the condition</returns>
        public BO.Student GetStudent(Predicate<BO.Student> predicate)
        {
            return _studentList.FirstOrDefault(s => predicate(s));
        }

        /// <summary>
        /// Remove student from the database
        /// remove all the pairs of this student
        /// </summary>
        /// <param name="id">the student id</param>
        public void RemoveStudentAsync(int id)
        {
            try
            {
                var studToRem = GetStudent(id);
                //RemovePairFromStudent(studToRem);

                _studentList.Remove(studToRem);
                studentIdsToRemove.Add(id);
            }
            catch (DO.BadStudentException ex1)
            {
                throw new BO.BadStudentException(ex1.Message, id);
            }
            catch (Exception ex2)
            {
                throw new Exception(ex2.Message);
            }
        }

        //private async Task RemovePairFromStudent(BO.Student student)
        //{
        //    if (_studentList.Any(s => s.MatchTo.Contains(student.Id)))
        //    {
        //        var matchPairOfThisStud = _pairList
        //            .Where(p => p.StudentFromIsraelId == student.Id
        //            || p.StudentFromWorldId == student.Id)
        //            .ToList();
        //        if (matchPairOfThisStud != null && matchPairOfThisStud.Count() > 0)
        //        {
        //            List<Task> tasks = new List<Task>();
        //            foreach (var pairToRem in matchPairOfThisStud)
        //            {
        //                tasks.Add(RemovePairAsync(pairToRem.Id, false));
        //            }
        //            await Task.WhenAll(tasks);
        //        }
        //    }
        //}

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
    }
}
