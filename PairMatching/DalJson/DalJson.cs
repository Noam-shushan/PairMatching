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
        const string lastDateOfSheetsPath = @"lastDateOfSheets.json";
        #endregion

        #region Student
        public int AddStudent(Student student)
        {
            // load all students from the json file 
            var studList = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            var stud = studList.Find(s => s.Email == student.Email);
            // Checks for duplication student
            if (stud != null && !stud.IsDeleted)
            {
                throw new Exception($"student {student.Id} is exists");
            }
            
            var counter = JsonTools.LoadObjFromJsonFile<Counters>(countersPath);
            if (counter == null)
            {
                counter = Counters.Instance;
            }
            counter.IncStudentCounter();
            student.Id = counter.StudentCounter;
            studList.Add(student);
            
            JsonTools.SaveListToJsonFile(studList, studentsPath);
            JsonTools.SaveObjToJsonFile(counter, countersPath);
            return student.Id;
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
            var list = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            if(list == null)
            {
                return null;
            }
            return from s in list
                   where !s.IsDeleted
                   select s;
        }

        public IEnumerable<Student> GetAllStudentsBy(Predicate<Student> predicate)
        {
            var list = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            if (list == null)
            {
                return null;
            }
            return from s in list
                   where !s.IsDeleted && predicate(s)
                   select s;
        }

        public void UpdateStudent(DO.Student student)
        {
            var studList = JsonTools.LoadListFromJsonFile<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == student.Id);

            if (stud != null && !stud.IsDeleted)
            {
                studList.Remove(stud);
                studList.Add(student);

                JsonTools.SaveListToJsonFile(studList, studentsPath);
                return;
            }
            throw new Exception($"can not find the sutdent {student}");
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
            var list = JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath);
            if(list == null)
            {
                return null;
            }
            return from l in list 
                   select l;
        }

        public IEnumerable<LearningTime> GetAllLearningTimesBy(Predicate<LearningTime> predicate)
        {
            var list = JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath);
            if (list == null)
            {
                return null;
            }
            return from l in list
                   where predicate(l)
                   select l;
        }

        public LearningTime GetLearningTime(int id)
        {
            var learningTime = (from l in JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath)
                    where l.Id == id
                    select l).FirstOrDefault();
            if(learningTime == default)
            {
                return null;
            }
            return learningTime;
        }

        public void AddLearningTime(LearningTime learningTime)
        {
            var learningTimesList = JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath);
            learningTimesList.Add(learningTime);
            JsonTools.SaveListToJsonFile(learningTimesList, learningTimePath);
        }
        #endregion

        public void UpdateLastDateOfSheets(LastDateOfSheets lastDateOfSheets)
        {
            JsonTools.SaveObjToJsonFile(lastDateOfSheets, lastDateOfSheetsPath);
        }

        public LastDateOfSheets GetLastDateOfSheets()
        {
            return JsonTools.LoadObjFromJsonFile<LastDateOfSheets>(lastDateOfSheetsPath);
        }
    }
}
