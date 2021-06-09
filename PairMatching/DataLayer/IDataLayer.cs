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

        #region LearningTime
        IEnumerable<DO.LearningTime> GetAllLearningTimes();

        IEnumerable<DO.LearningTime> GetAllLearningTimesBy(Predicate<DO.LearningTime> predicate);

        DO.LearningTime GetLearningTime(int id);

        void AddLearningTime(DO.LearningTime learningTime); 
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
