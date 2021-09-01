using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LogicLayer
{
    public interface IBL
    {
        Predicate<BO.Student> StudentListFilter { get; set; }

        BO.Statistics Statistics { get; }

        /// <summary>
        /// the students list which wiil use in the Gui layer
        /// </summary>
        ObservableCollection<BO.Student> StudentList { get; set; }

        Predicate<BO.Pair> PairListFilter { get; set; }

        ObservableCollection<BO.Pair> PairList { get; set; }

        List<BO.SimpleStudent> StudentWithUnvalidEmail { get; set; }

        /// <summary>
        /// Get all the students by some predicate from the data source
        /// </summary>
        /// <param name="predicate">condition on Student</param>
        /// <returns>all students thet is the condition</returns>
        IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate);

        /// <summary>
        /// Get Student from the data source
        /// </summary>
        /// <param name="id">the student id</param>
        /// <returns>the student match to the id</returns>
        BO.Student GetStudent(int id);

        BO.Student GetStudent(Predicate<BO.Student> predicate);

        /// <summary>
        /// Remove student from the data source
        /// </summary>
        /// <param name="id">the student id</param>
        Task RemoveStudentAsync(int id);

        void AddStudent(BO.Student student);

        void UpdateStudent(BO.Student student);

        /// <summary>
        /// Update all the data from the google sheets
        /// </summary>
        Task ReadDataFromSpredsheetAsync();

        /// <summary>
        /// making the match between the tow students
        /// </summary>
        /// <param name="fromIsrael">student from israel</param>
        /// <param name="fromWorld">student from the world</param>
        Task<int> MatchAsync(BO.Student fromIsrael, BO.Student fromWorld);

        /// <summary>
        /// Update the data from the data base and calulate the matching students
        /// </summary>
        Task UpdateAsync();

        IEnumerable<BO.Pair> GetAllPairs();

        Task RemovePairAsync(BO.Pair pair);

        void SearchStudents(string preifxName);

        Task SendEmailToPairAsync(BO.Pair pair, EmailTypes emailTypes);

        Task SendEmailToStudentAsync(BO.Student student, EmailTypes emailTypes);

        Task SendOpenEmailAsync(string to, string subject, string body, string fileAttachment = "");

        void UpdatePair(BO.Pair pair);

        Task ActivatePairAsync(BO.Pair pair);

        void FilterPairsByTrack(string track);
    }
}
