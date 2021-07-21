using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        Task UpdateDataAsync();

        /// <summary>
        /// the students list which wiil use in the Gui layer
        /// </summary>
        ObservableCollection<BO.Student> StudentList { get; set; }

        /// <summary>
        /// making the match between the tow students
        /// </summary>
        /// <param name="fromIsrael">student from israel</param>
        /// <param name="fromWorld">student from the world</param>
        Task MatchAsync(BO.Student fromIsrael, BO.Student fromWorld);

        /// <summary>
        /// Get all pairs from the data base
        /// </summary>
        /// <returns>list of all pairs in a tuple of tow students</returns>
        IEnumerable<Tuple<BO.Student, BO.Student>> GetAllPairs();

        /// <summary>
        /// Update the data from the data base
        /// </summary>
        Task UpdateAsync();

        IEnumerable<BO.Pair> GetAllPair();

        Task RemovePairAsync(BO.Pair pair);
    }
}
