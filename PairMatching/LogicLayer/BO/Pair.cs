using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace BO
{
    public class Pair
    {
        /// <summary>
        /// flag that determine if the pair is deleted from the data source 
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// the first student id 
        /// </summary>
        public SimpleStudent FirstStudent { get; set; }

        /// <summary>
        /// The macher student id for the first student
        /// </summary>
        public SimpleStudent SecondStudent { get; set; }

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public IEnumerable<PrefferdTracks> PrefferdTracks { get; set; }

        public string PrefferdTracksShow
        {
            get
            {
                return string.Join(", ", from p in PrefferdTracks
                                         select Dictionaries.PrefferdTracksDict[p]);
            }
        }

        public bool IsSelected { get; set; }

        public override string ToString()
        {
            return $"חברותא א: {FirstStudent.Name} , חברותא ב: {SecondStudent.Name}";
        }
    }

    public class SimpleStudent
    {
        /// <summary>
        /// the id number of the student
        /// </summary>
        public int Id { get; set; }

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

        public string GenderShow { get => Dictionaries.GendersDict[Gender]; }
    }
}
