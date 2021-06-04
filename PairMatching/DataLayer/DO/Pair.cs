using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
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
        public int FirstStudent { get; set; }
        
        /// <summary>
        /// The macher student id for the first student
        /// </summary>
        public int SecondStudent { get; set; }
        
        /// <summary>
        /// the matching degree of the pair 
        /// </summary>
        public MatchingDegrees MatchingDegree { get; set; }
    }
}
