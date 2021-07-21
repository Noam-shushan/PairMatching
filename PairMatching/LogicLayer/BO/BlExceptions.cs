using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class BadPairException : Exception
    {
        public string FirstStudentName { get; set; }

        public string SecondeStudentName { get; set; }

        public BadPairException(string message, string firsName, string secondName) : base(message)
        {
            FirstStudentName = firsName;
            SecondeStudentName = secondName;
        }

        public override string ToString() => base.ToString() + $": {FirstStudentName}, {SecondeStudentName}";
    }
}
