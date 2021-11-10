using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// TODO: add documentation
    /// </summary>
    public interface IDataLayer
    {
        #region Student
        /// <summary>
        /// Get all the students from the database
        /// </summary>
        /// <returns>list of all students</returns>
        IEnumerable<DO.Student> GetAllStudents();

        Task SaveAllDataFromSpredsheetAsync();

        int GetNewStudentId();

        /// <summary>
        /// Add new student to the database
        /// </summary>
        /// <param name="student">the new student</param>
        int AddStudent(DO.Student student);

        /// <summary>
        /// Get Student from the database
        /// </summary>
        /// <param name="id">the student id</param>
        /// <returns>the student match to the id</returns>
        DO.Student GetStudent(int id);

        /// <summary>
        /// Remove student from the database
        /// </summary>
        /// <param name="id">the student id</param>
        void RemoveStudent(int id);

        /// <summary>
        /// update student in the database
        /// </summary>
        void UpdateStudent(DO.Student student);
        #endregion

        #region Pair
        /// <summary>
        /// Get all the pairs from the database
        /// </summary>
        /// <returns>All the pairs</returns>
        IEnumerable<DO.Pair> GetAllPairs();

        /// <summary>
        /// Add new pair to the database 
        /// </summary>
        /// <param name="pair">the new pair</param>
        int AddPair(DO.Pair pair);

        /// <summary>
        /// Remove pair from the database
        /// </summary>
        /// <param name="id">the id of the pair</param>
        void RemovePair(DO.Pair pair);

        void UpdatePair(DO.Pair pair);

        DO.Pair GetPair(int id);
        #endregion

        #region SpredsheetLastRange
        void UpdateSheetsLastRange(DO.SpredsheetLastRange lastDateOfSheets);
        DO.SpredsheetLastRange GetSheetsLastRang(); 
        #endregion
    }
}
