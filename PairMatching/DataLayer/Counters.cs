using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Counters
    {
        [BsonId]
        public int Id { get; } = 1;

        static int _studentCounter = 0;

        static int _pairCounter = 0;

        public static Counters Instance { get; } = new Counters();

        Counters() 
        {
            
        }

        public int StudentCounter
        {
            get => _studentCounter;
            set => _studentCounter = value;
        }

        public int PairCounter 
        { 
            get => _pairCounter; 
            set => _pairCounter = value; 
        }

        public void IncStudentCounter()
        {
            ++_studentCounter;
        }

        public void IncPairCounter()
        {
            ++_pairCounter;
        }
    }
}
