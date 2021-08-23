using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
    public class BadPairException : Exception
    {
        public int PairId { get; set; }

        public int FirstStudentId { get; set; }
        
        public int SecondeStudentId { get; set; }

        public BadPairException(string message, int id) : base(message) => PairId = id; 

        public BadPairException(string message, int firstId, int secondId) : base(message)
        {
            FirstStudentId = firstId;
            SecondeStudentId = secondId;
        }

        public override string ToString() => base.ToString();
    }
}
