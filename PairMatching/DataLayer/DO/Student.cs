using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class Student
    {
        /// <summary>
        /// the id number of the student
        /// </summary>
        [BsonId]
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
        public string PhoneNumber { get; set; }

        /// <summary>
        /// the gender of the student
        /// </summary>
        public Genders Gender { get; set; }

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public IEnumerable<PrefferdTracks> PrefferdTracks { get; set; }

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

        /// <summary>
        /// the utc offset of the student
        /// </summary>
        public TimeSpan UtcOffset { get; set; }

        /// <summary>
        /// the id of the student that match to this.
        /// </summary>
        public int MatchTo { get; set; }

        /// <summary>
        /// Desired learning time and day
        /// </summary>
        public IEnumerable<LearningTime> DesiredLearningTime { get; set; }

        public IEnumerable<OpenQuestion> OpenQuestions { get; set; }

        public int PrefferdNumberOfMatchs { get; set; }

        public string InfoAbout { get; set; }

        public MoreLanguages MoreLanguages { get; set; }

        public IEnumerable<string> Languages { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}\n" +
                $"Country: {Country}\n" +
                $"Gender: {Gender}";
        }
    }
}
