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
        private Counters _counters;

        public Counters GetCounters()
        {
            return _counters;
        }

        void SetCounters()
        {
            var counter = JsonTools.LoadObjFromJsonFile<Counters>(countersPath);
            if (counter == null)
            {
                _counters = Counters.Instance;
            }
            else
            {
                _counters = counter;
            }
        }

        #region Singleton
        public static IDataLayer Instance { get; } = new DalJson();

        private DalJson() 
        {
            SetCounters();
        }
        #endregion

        #region paths
        private readonly string studentsPath = @"studentListJson.json";
        private readonly string pairsPath = @"pairListJson.json";
        private readonly string countersPath = @"counters.json";
        private readonly string learningTimePath = @"learningTime.json";
        private readonly string lastDateOfSheetsPath = @"lastDateOfSheets.json";
        private readonly string openQuestionsPath = @"openQuestions.json";
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
            
            _counters.IncStudentCounter();
            student.Id = _counters.StudentCounter;
            studList.Add(student);
            
            JsonTools.SaveListToJsonFile(studList, studentsPath);
            JsonTools.SaveObjToJsonFile(_counters, countersPath);
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

        public void UpdateStudent(Student student)
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
            var pTemp = pairList.Find(p => p.FirstStudent == pair.FirstStudent
                                                 && p.SecondStudent == pair.SecondStudent);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                throw new BadPairException("the pair is exist", pTemp.FirstStudent, pTemp.SecondStudent);
            }
            if (pTemp != null && pTemp.IsDeleted)
            {
                pairList.Remove(pTemp);
            }

            pairList.Add(pair);
            JsonTools.SaveListToJsonFile(pairList, pairsPath);
        }

        public Pair GetPair(int firstStudent, int secondStudent)
        {
            var pairList = JsonTools.LoadListFromJsonFile<Pair>(pairsPath);
            var pTemp = pairList.Find(p => p.FirstStudent == firstStudent
                                                 && p.SecondStudent == secondStudent);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                return pTemp;
            }
            throw new BadPairException("the pair is not found", pTemp != null ? pTemp.FirstStudent : 0, 
                pTemp != null ? pTemp.SecondStudent : 0);
        }

        public void RemovePair(int firstStudent, int secondStudent)
        {
            var pairList = JsonTools.LoadListFromJsonFile<Pair>(pairsPath);
            var pTemp = pairList.Find(p => p.FirstStudent == firstStudent
                                                 && p.SecondStudent == secondStudent);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                pairList.Remove(pTemp);
                pTemp.IsDeleted = true;
                pairList.Add(pTemp);

                JsonTools.SaveListToJsonFile(pairList, pairsPath);
                return;
            }

            throw new BadPairException("the pair is not found", pTemp != null ? pTemp.FirstStudent : 0,
                        pTemp != null ? pTemp.SecondStudent : 0);
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
            return JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath);
        }

        public IEnumerable<LearningTime> GetAllLearningTimesBy(Predicate<LearningTime> predicate)
        {
            return from l in JsonTools.LoadListFromJsonFile<LearningTime>(learningTimePath)
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

        #region OpenQuestions
        public IEnumerable<OpenQuestion> GetAllOpenQuestions()
        {
            return JsonTools.LoadListFromJsonFile<OpenQuestion>(openQuestionsPath);
        }

        public IEnumerable<OpenQuestion> GetAllOpenQuestionsBy(Predicate<OpenQuestion> predicate)
        {
            return from o in JsonTools.LoadListFromJsonFile<OpenQuestion>(openQuestionsPath)
                   where predicate(o)
                   select o;
        }

        public void AddOpenQuestions(OpenQuestion openQuestion)
        {
            var list = JsonTools.LoadListFromJsonFile<OpenQuestion>(openQuestionsPath);
            list.Add(openQuestion);
            JsonTools.SaveListToJsonFile(list, openQuestionsPath);
        } 
        #endregion

        #region Last update of the data tables
        public void UpdateLastDateOfSheets(LastDateOfSheets lastDateOfSheets)
        {
            JsonTools.SaveObjToJsonFile(lastDateOfSheets, lastDateOfSheetsPath);
        }

        public LastDateOfSheets GetLastDateOfSheets()
        {
            return JsonTools.LoadObjFromJsonFile<LastDateOfSheets>(lastDateOfSheetsPath);
        }
        #endregion
    }
}
