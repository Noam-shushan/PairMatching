using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using UtilEntities;

namespace DO
{
    public class Pair
    {
        [BsonId]
        public int Id { get; set; }

        /// <summary>
        /// flag that determine if the pair is deleted from the database 
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

        public DateTime DateOfCreate { get; set; }

        public DateTime DateOfUpdate { get; set; }

        public DateTime DateOfDelete { get; set; }

        public bool IsActive { get; set; } = false;

        /// <summary>
        /// Prefferd tracks of lernning {TANYA, TALMUD, PARASHA ...}
        /// </summary>
        public PrefferdTracks PrefferdTracks { get; set; }

        public List<Note> Notes { get; set; } = new List<Note>();
    }
}
