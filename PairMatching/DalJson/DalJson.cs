using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DO;

namespace DalJson
{
    public class DalJson : IDataLayer
    {
        #region Singleton
        public static IDataLayer Instance { get; } = new DalJson();

        DalJson() { } 
        #endregion

        #region paths
        const string studentsPath = @"studentListJson.json";
        const string pairsPath = @"pairListJson.json";
        const string countersPath = @"counters.json";
        const string learningTimePath = @"learningTime.json";
        #endregion

        #region Student
        public void AddStudent(Student student)
        {
            // load all students from the json file 
            var studList = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == student.Id);
            // Checks for duplication student
            if (stud != null && !stud.IsDeleted)
            {
                throw new Exception($"student {student.Id} is exists");
            }
            
            var counter = JsonTools.LoadObjFromJsonFile<Counters>(countersPath);
            
            counter.IncStudentCounter();
            student.Id = counter.StudentCounter;
            studList.Add(student);
            
            JsonTools.SaveListToJsonFile(studList, studentsPath);
            JsonTools.SaveObjToJsonFile(counter, countersPath);
        }

        public void RemoveStudent(int id)
        {
            var studList = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == id);
            
            if (stud != null && !stud.IsDeleted)
            {
                studList.Remove(stud);
                stud.IsDeleted = true;
                studList.Add(stud);

                JsonTools.SaveListToJsonFile(studList, studentsPath);
                return;
            }
            throw new Exception($"can not find the sutdent {id}");
        }

        public Student GetStudent(int id)
        {
            var studList = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == id);
            if(stud != null && !stud.IsDeleted)
            {
                return stud;
            }
            throw new Exception($"can not find the sutdent {id}");
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return from s in JsonTools.LoadListFromJsonFile<Student>(studentsPath)
                   where !s.IsDeleted
                   select s;
        }

        public IEnumerable<Student> GetAllStudentsBy(Predicate<Student> predicate)
        {
            return from s in JsonTools.LoadListFromJsonFile<Student>(studentsPath)
                   where !s.IsDeleted && predicate(s)
                   select s;
        } 
        #endregion

        #region Pair
        public void AddPair(Pair pair)
        {
            var pairList = JsonTools.LoadListFromJsonFile<Pair>(pairsPath);
            var pTemp = pairList.FirstOrDefault(p => p.FirstStudent == pair.FirstStudent
                                                 && p.SecondStudent == pair.SecondStudent);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                throw new Exception("the pair is olredy exist");
            }

            pairList.Add(pair);
            JsonTools.SaveListToJsonFile(pairList, pairsPath);
        }

        public Pair GetPair(int firstStudent, int secondStudent)
        {
            var pairList = JsonTools.LoadListFromJsonFile<Pair>(pairsPath);
            var pTemp = pairList.FirstOrDefault(p => p.FirstStudent == firstStudent
                                                 && p.SecondStudent == secondStudent);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                return pTemp;
            }
            throw new Exception("the pair not found");
        }

        public void RemovePair(int firstStudent, int secondStudent)
        {
            var pairList = JsonTools.LoadListFromJsonFile<Pair>(pairsPath);
            var pTemp = pairList.FirstOrDefault(p => p.FirstStudent == firstStudent
                                                 && p.SecondStudent == secondStudent);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                pairList.Remove(pTemp);
                pTemp.IsDeleted = true;
                pairList.Add(pTemp);

                JsonTools.SaveListToJsonFile(pairList, pairsPath);
                return;
            }

            throw new Exception("the pair not found");
        }

        public IEnumerable<Pair> GetAllPairs()
        {
            return from p in JsonTools.LoadListFromJsonFile<Pair>(pairsPath)
                   where !p.IsDeleted
                   select p;
        }

        public IEnumerable<Pair> GetAllPairsBy(Predicate<Pair> predicate)
        {
            return from p in JsonTools.LoadListFromJsonFile<Pair>(pairsPath)
                   where !p.IsDeleted && predicate(p)
                   select p;
        }
        #endregion

        #region LearningTime
        public IEnumerable<LearningTime> GetAllLearningTimes()
        {
            return from l in JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath)
                   select l;
        }

        public IEnumerable<LearningTime> GetAllLearningTimesBy(Predicate<LearningTime> predicate)
        {
            return from l in JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath)
                   where predicate(l)
                   select l;
        }

        public LearningTime GetLearningTime(int id)
        {
            return (from l in JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath)
                    where l.Id == id
                    select l).FirstOrDefault();
        }

        public void AddLearningTime(LearningTime learningTime)
        {
            var learningTimesList = JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath);
            learningTimesList.Add(learningTime);
            JsonTools.SaveListToJsonFile(learningTimesList, learningTimePath);
        } 
        #endregion
    }
}
