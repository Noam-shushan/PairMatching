using System;
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

    public class BadStudentException : Exception
    {
        public int StudnetId { get; set; }

        public BadStudentException(string message, int id) : base(message) => StudnetId = id;


        public override string ToString() => base.ToString();
    }
}
