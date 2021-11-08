using System.Collections.Generic;
using System.Linq;
using DO;
using UtilEntities;

namespace BO
{
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

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public IEnumerable<PrefferdTracks> PrefferdTracks { get; set; } = new List<PrefferdTracks>(); 

        public bool IsFromIsrael { get => Country == "Israel"; }

        public string PrefferdTracksShow
        {
            get
            {
                
                return IsFromIsrael ? string.Join(",", from p in PrefferdTracks
                                          select Dictionaries.PrefferdTracksDict[p])
                    : string.Join(",", from p in PrefferdTracks
                                       select p.ToString()).ToLower();
            }
        }
    }
}
