using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace BO
{
    public class Student
    {
        /// <summary>
        /// the id number of the student
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// is this student as deleted from the data source
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// the name of the student
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the country of the student
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// the email of the student
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// the phone number of the student
        /// </summary>
        public int PhoneNumber { get; set; }

        /// <summary>
        /// the gender of the student
        /// </summary>
        public Genders Gender { get; set; }

        /// <summary>
        /// Desired learning time and day
        /// </summary>
        public LearningTime DesiredLearningTime { get; set; }

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks PrefferdTracks { get; set; }

        /// <summary>
        /// the prefferd gender to lern with
        /// </summary>
        public Genders PrefferdGender { get; set; }

        /// <summary>
        ///  Desired level of english from the other pair
        /// </summary>
        public EnglishLevels DesiredEnglishLevel { get; set; }

        /// <summary>
        ///  level of english
        /// </summary>
        public EnglishLevels EnglishLevel { get; set; }

        /// <summary>
        ///  Desired level of skiil from the other pair
        /// </summary>
        public SkillLevels DesiredSkillLevel { get; set; }

        /// <summary>
        /// level of skiil 
        /// </summary>
        public SkillLevels SkillLevel { get; set; }

        /// <summary>
        /// learning style 
        /// </summary>
        public LearningStyles LearningStyle { get; set; }

        public IEnumerable<Student> FirstMatchingStudents { get; set; }

        public IEnumerable<Student> SecondeMatchingStudents { get; set; }

    }
}

