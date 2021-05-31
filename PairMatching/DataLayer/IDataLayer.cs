using System;

namespace DataLayer
{
    public interface IDataLayer
    {
        #region Student
        /// <summary>
        /// Add new student to the data source
        /// </summary>
        /// <param name="student">the new student</param>
        void AddStudent(DO.Student student);

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
        #endregion

        #region Pair
        /// <summary>
        /// Add new pair to the data source 
        /// </summary>
        /// <param name="pair">the new pair</param>
        void AddPair(DO.Pair pair);

        /// <summary>
        /// Get pair from the data source
        /// </summary>
        /// <param name="firstStudent">first sudent id</param>
        /// <param name="secondStudent">second student id</param>
        /// <returns>the pair match to the id's</returns>
        DO.Pair GetPair(int firstStudent, int secondStudent);

        /// <summary>
        /// Remove pair from the data source
        /// </summary>
        /// <param name="firstStudent">first sudent id</param>
        /// <param name="secondStudent">second student id</param>
        void RemovePair(int firstStudent, int secondStudent); 
        #endregion

    }
}
