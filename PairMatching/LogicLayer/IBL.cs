using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IBL
    {
        /// <summary>
        /// Get all the students from the data source
        /// </summary>
        /// <returns>list of all students</returns>
        IEnumerable<BO.Student> GetAllStudents();

        /// <summary>
        /// Get all the students by some predicate from the data source
        /// </summary>
        /// <param name="predicate">condition on Student</param>
        /// <returns>all students thet is the condition</returns>
        IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate);

        /// <summary>
        /// Add new student to the data source
        /// </summary>
        /// <param name="student">the new student</param>
        int AddStudent(BO.Student student);

        /// <summary>
        /// Get Student from the data source
        /// </summary>
        /// <param name="id">the student id</param>
        /// <returns>the student match to the id</returns>
        BO.Student GetStudent(int id);

        /// <summary>
        /// Remove student from the data source
        /// </summary>
        /// <param name="id">the student id</param>
        void RemoveStudent(int id);

        /// <summary>
        /// Update all the data from the google sheets
        /// </summary>
        void UpdateData();

        /// <summary>
        /// the students list
        /// </summary>
        IEnumerable<BO.Student> StudentList { get; set; }

        /// <summary>
        /// making the match between the tow students
        /// </summary>
        /// <param name="fromIsreal"></param>
        /// <param name="fromWord"></param>
        void Match(BO.Student fromIsreal, BO.Student fromWord);

        IEnumerable<Tuple<BO.Student, BO.Student>> GetAllPairs();
    }
}
