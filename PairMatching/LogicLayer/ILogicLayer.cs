using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LogicLayer.Email;

namespace LogicLayer
{
    /// <summary>
    /// Logic layer interface.<br/> 
    /// Performs CRUD operations on the databse.<br/>
    /// Performs all the requirements of the GUI including data management, sending emails,<br/>
    /// recalculating finding suggestions for matching between students and more.<br/>
    /// Reads Google Sheets and turns them into objects and saves in database.
    /// </summary>
    public interface ILogicLayer
    {
        #region Properties
        /// <summary>
        /// A Statistics object of that hold infromation on the data
        /// </summary>
        BO.Statistics Statistics { get; }

        /// <summary>
        /// Filter predicate on the students list
        /// </summary>
        Predicate<BO.Student> StudentListFilter { get; set; }
        
        /// <summary>
        /// List of all students
        /// </summary>
        ObservableCollection<BO.Student> StudentList { get; set; }

        /// <summary>
        /// Filter predicate on the pairs list
        /// </summary>
        Predicate<BO.Pair> PairListFilter { get; set; }

        /// <summary>
        /// List of all pairs
        /// </summary>
        ObservableCollection<BO.Pair> PairList { get; set; }

        /// <summary>
        /// List of new student that thire email addres is not valid
        /// </summary>
        List<BO.SimpleStudent> StudentWithUnvalidEmail { get; set; } 
        #endregion

        #region Students
        /// <summary>
        /// Get all the students by some predicate from the database
        /// </summary>
        /// <param name="predicate">condition on Student</param>
        /// <returns>all students thet is the condition</returns>
        IEnumerable<BO.Student> GetAllStudentsBy(Predicate<BO.Student> predicate);

        /// <summary>
        /// Get Student from the database
        /// </summary>
        /// <param name="id">the student id</param>
        /// <returns>the student match to the id</returns>
        BO.Student GetStudent(int id);

        /// <summary>
        /// Get Student from the database
        /// </summary>
        /// <param name="predicate">predicate on the student propertiecs</param>
        /// <returns>the student that match to the condition</returns>
        BO.Student GetStudent(Predicate<BO.Student> predicate);

        /// <summary>
        /// Remove student from the database
        /// remove all the pairs of this student
        /// </summary>
        /// <param name="id">the student id</param>
        Task RemoveStudentAsync(int id);

        /// <summary>
        /// Add student to the database
        /// </summary>
        /// <param name="student">The new student to add</param>
        void AddStudent(BO.Student student);

        /// <summary>
        /// Update student in the database
        /// </summary>
        /// <param name="student">The student to updata</param>
        void UpdateStudent(BO.Student student);

        /// <summary>
        /// Add note to student
        /// </summary>
        /// <param name="student">The student to add note to</param>
        /// <param name="note">The note to add to the student</param>
        void AddNoteToStudent(BO.Student student, BO.Note note);

        /// <summary>
        /// Remove note from student
        /// </summary>
        /// <param name="student">The student to remove the note from</param>
        /// <param name="note">The note to remove from the student</param>
        void RemoveNoteFromStudent(BO.Student student, BO.Note note);

        /// <summary>
        /// Search students by prefix of thire name 
        /// </summary>
        /// <param name="preifxName">The prefix that cams from the Gui</param>
        void SearchStudents(string preifxName); 
        #endregion

        #region Data reading and updating 
        /// <summary>
        /// Read the new data of student from the google sheets
        /// Seaving all the data to the database.
        /// Sending automatic emails to the new studnts
        /// </summary>
        Task ReadDataFromSpredsheetAsync();

        /// <summary>
        /// Update all the data from the database and find new matching students
        /// </summary>
        Task UpdateAsync(); 
        #endregion

        #region Emails
        /// <summary>
        /// Send automatic email to pair
        /// </summary>
        /// <param name="pair">The pair to send the email to</param>
        /// <param name="emailTypes">The email type [new pair, pair broke, ect...]</param>
        /// <returns></returns>
        Task SendEmailToPairAsync(BO.Pair pair, EmailTypes emailTypes);

        /// <summary>
        /// Send automatic email to studnet
        /// </summary>
        /// <param name="student">The student to send the email to</param>
        /// <param name="emailTypes">The email type [new rejester, status email, ect...]</param>
        /// <returns></returns>     
        Task SendEmailToStudentAsync(BO.Student student, EmailTypes emailTypes);
        
        /// <summary>
        ///  Send open email 
        /// </summary>
        /// <param name="to">Email addres to send</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="body">The body of the email</param>
        /// <param name="fileAttachment">File name to attach to the email</param>
        /// <returns></returns>
        Task SendOpenEmailAsync(string to, string subject, string body, string fileAttachment = "");
        #endregion

        #region Pair
        /// <summary>
        /// Making the match between the tow students
        /// </summary>
        /// <param name="fromIsrael">student from israel</param>
        /// <param name="fromWorld">student from the world</param>
        Task<int> MatchAsync(BO.Student fromIsrael, BO.Student fromWorld);

        /// <summary>
        /// Get all pair from the database
        /// </summary>
        /// <returns></returns>
        IEnumerable<BO.Pair> GetAllPairs();

        /// <summary>
        /// Remove pair from the database.
        /// after the remove recalculating finding suggestions for matching
        /// </summary>
        /// <param name="pair">The pair to renove</param>
        /// <returns></returns>
        Task RemovePairAsync(BO.Pair pair);

        /// <summary>
        /// Update pair in the database
        /// </summary>
        /// <param name="pair">The pair to update</param>
        void UpdatePair(BO.Pair pair);

        /// <summary>
        /// Get pair from the pair list by predicte
        /// </summary>
        /// <param name="predicate">the predicate</param>
        /// <returns></returns>
        BO.Pair GetPair(Predicate<BO.Pair> predicate);

        /// <summary>
        /// Get pair from the pair list by id
        /// </summary>
        /// <param name="pairId">The pair id</param>
        /// <returns></returns>
        BO.Pair GetPair(int pairId);

        /// <summary>
        /// Add note to pair
        /// </summary>
        /// <param name="pair">The pair to add note to</param>
        /// <param name="note">The note to add to the pair</param>
        void AddNoteToPair(BO.Pair pair, BO.Note note);

        /// <summary>
        /// Remove note from pair
        /// </summary>
        /// <param name="pair">The pair to remove note from</param>
        /// <param name="note">The note to remove from the pair</param>
        void RemoveNoteFromPair(BO.Pair pair, BO.Note note);

        /// <summary>
        /// Active pair from standby status to active pair.
        /// recalculating finding suggestions for matching
        /// </summary>
        /// <param name="pair">The pair to activete</param>
        /// <returns></returns>
        Task ActivatePairAsync(BO.Pair pair);

        /// <summary>
        /// Filter the pair list by track of stading
        /// </summary>
        /// <param name="track">the track stading</param>
        void FilterPairsByTrack(string track); 
        #endregion
    }
}
