using System;
using DataLayer;
using DO;
using DS;

namespace DalJson
{
    public class DalJson : IDataLayer
    {
        const string studentsPath = @"studentListJson.json";
        const string pairsPath = @"pairListJson.json";
        const string countersPath = @"counters.json";

        public void AddStudent(Student student)
        {
            var studList = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            if(studList.Find(s => s.Id == student.Id) != null)
            {
                throw new Exception($"student {student.Id} is exists" );
            }
            var counters = (Counters)JsonTools.LoadObjFromJsonFile<Counters>(countersPath);
           
            studList.Add(student);


        }

        public void RemoveStudent(int id)
        {
            throw new NotImplementedException();
        }

        public Student GetStudent(int id)
        {
            throw new NotImplementedException();
        }

        public void RemovePair(int firstStudent, int secondStudent)
        {
            throw new NotImplementedException();
        }

        public Pair GetPair(int firstStudent, int secondStudent)
        {
            throw new NotImplementedException();
        }

        public void AddPair(Pair pair)
        {
            throw new NotImplementedException();
        }
    }
}
