using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class Pair
    {
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// flag that determine if the pair is deleted from the data source 
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// the first student id 
        /// </summary>
        public int StudentFromIsraelId { get; set; }
        
        /// <summary>
        /// The macher student id for the first student
        /// </summary>
        public int StudentFromWorldId { get; set; }
        
        /// <summary>
        /// the matching degree of the pair 
        /// </summary>
        public MatchingDegrees MatchingDegree { get; set; }

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfUpdate { get; set; }

        public DateTime DateOfDelete { get; set; }

        public string InfoAbout { get; set; } = "";

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks PrefferdTracks { get; set; }
    }
}
