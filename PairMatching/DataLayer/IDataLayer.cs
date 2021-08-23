using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public interface IDataLayer
    { 
        #region Student
        /// <summary>
        /// Get all the students from the data source
        /// </summary>
        /// <returns>list of all students</returns>
        IEnumerable<DO.Student> GetAllStudents();

        /// <summary>
        /// Get all the students by some predicate from the data source
        /// </summary>
        /// <param name="predicate">condition on Student</param>
        /// <returns>all students thet is the condition</returns>
        IEnumerable<DO.Student> GetAllStudentsBy(Predicate<DO.Student> predicate);

        Task SaveAllDataFromSpredsheetAsync();

        int GetNewStudentId();

        /// <summary>
        /// Add new student to the data source
        /// </summary>
        /// <param name="student">the new student</param>
        int AddStudent(DO.Student student);

        /// <summary>
        /// Get Student from the data source
        /// </summary>
        /// <param name="id">the student id</param>
        /// <returns>the student match to the id</returns>
        DO.Student GetStudent(int id);

        /// <summary>
        /// Remove student from the data source
        /// </summary>
        /// <param name="id">the student id</param>
        void RemoveStudent(int id);

        /// <summary>
        /// update student in the data source
        /// </summary>
        void UpdateStudent(DO.Student student);
        #endregion

        #region Pair
        /// <summary>
        /// Get all the pairs from the data source
        /// </summary>
        /// <returns>all pairs</returns>
        IEnumerable<DO.Pair> GetAllPairs();
        
        /// <summary>
        /// Get all the pairs by some predicate from the data source
        /// </summary>
        /// <param name="predicate">condition on the Pair</param>
        /// <returns>all pairs thet is the condition</returns>
        IEnumerable<DO.Pair> GetAllPairsBy(Predicate<DO.Pair> predicate);

        /// <summary>
        /// Add new pair to the data source 
        /// </summary>
        /// <param name="pair">the new pair</param>
        int AddPair(DO.Pair pair);

        /// <summary>
        /// Remove pair from the data source
        /// </summary>
        /// <param name="id">the id of the pair</param>
        void RemovePair(DO.Pair pair);

        void UpdatePair(DO.Pair pair);

        DO.Pair GetPair(int id);

        DO.Pair GetPair(int studFromIsraelId, int studFromWorldId);
        #endregion

        #region LastDateOfSheets
        void UpdateLastDateOfSheets(DO.LastDataOfSpredsheet lastDateOfSheets);
        DO.LastDataOfSpredsheet GetLastDateOfSheets(); 
        #endregion
    }
}
