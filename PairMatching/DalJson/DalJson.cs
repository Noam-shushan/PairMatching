using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DO;

namespace DalJson
{
    // TODO: add documentation
    public class DalJson : IDataLayer
    {
        #region Singleton
        private static IDataLayer _instance;

        private static readonly object _loke = new object();

        public static IDataLayer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_loke)
                    {
                        if (_instance == null)
                        {
                            _instance = new DalJson();
                        }
                    }
                }
                return _instance;
            }
        }

        private DalJson() 
        {
            SetCounters();
        }
        #endregion

        #region Counters
        private Counters _counters;

        void SetCounters()
        {
            var counter = JsonTools.LoadOne<Counters>(countersPath);
            if (counter == null)
            {
                _counters = Counters.Instance;
            }
            else
            {
                _counters = counter;
            }
        }
        #endregion

        #region paths
        private readonly string studentsPath = @"studentListJson.json";
        private readonly string pairsPath = @"pairListJson.json";
        private readonly string countersPath = @"counters.json";
        private readonly string lastDateOfSheetsPath = @"lastDateOfSheets.json";
        #endregion

        public async Task SaveAllDataFromSpredsheetAsync()
        {
            // load all students from the json file 
            await Task.Run(() =>
            {
                var studentsListFromDB = JsonTools.LoadRecords<Student>(studentsPath);
                studentsListFromDB.AddRange(DataSource.StudentsList);
                JsonTools.InsertRecords(studentsListFromDB, studentsPath);
            });

            await Task.Run(() => JsonTools.InsertOne(_counters, countersPath));
        }

        #region Student
        public int GetNewStudentId()
        {
            _counters.IncStudentCounter();
            return _counters.StudentCounter;
        }

        public int AddStudent(Student student)
        {
            // load all students from the json file 
            var studList = JsonTools.LoadRecords<Student>(studentsPath);
            var stud = studList.Find(s => s.Email == student.Email);
            // Checks for duplication student
            if (stud != null && !stud.IsDeleted)
            {
                throw new Exception($"student {student.Id} is exists");
            }
            _counters.IncStudentCounter();
            student.Id = _counters.StudentCounter;
            studList.Add(student);
            
            JsonTools.InsertRecords(studList, studentsPath);
            JsonTools.InsertOne(_counters, countersPath);
            return student.Id;
        }

        public void RemoveStudent(int id)
        {
            var studList = JsonTools.LoadRecords<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == id);
            
            if (stud != null && !stud.IsDeleted)
            {
                studList.Remove(stud);
                stud.IsDeleted = true;
                studList.Add(stud);

                JsonTools.InsertRecords(studList, studentsPath);
                return;
            }
            throw new Exception($"can not find the sutdent {id}");
        }

        public Student GetStudent(int id)
        {
            var studList = JsonTools.LoadRecords<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == id);
            
            return stud != null && !stud.IsDeleted ? stud :
                throw new Exception($"can not find the sutdent {id}");
        }

        public IEnumerable<Student> GetAllStudents()
        {
            var students = JsonTools.LoadRecords<Student>(studentsPath);
            return from s in students
                   where !s.IsDeleted
                   select s;
        }

        public void UpdateStudent(Student student)
        {
            var studList = JsonTools.LoadRecords<Student>(studentsPath);
            var stud = studList.FirstOrDefault(s => s.Id == student.Id);

            if (stud != null && !stud.IsDeleted)
            {
                studList.Remove(stud);
                studList.Add(student);

                JsonTools.InsertRecords(studList, studentsPath);
                return;
            }
            throw new Exception($"can not find the sutdent {student}");
        }
        #endregion

        #region Pair
        public int AddPair(Pair pair)
        {
            var pairList = JsonTools.LoadRecords<Pair>(pairsPath);
            var pTemp = pairList.Find(p => p.StudentFromIsraelId == pair.StudentFromIsraelId
                                                 && p.StudentFromWorldId == pair.StudentFromWorldId);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                throw new BadPairException("the pair is exist", pTemp.StudentFromIsraelId, pTemp.StudentFromWorldId);
            }
            if (pTemp != null && pTemp.IsDeleted)
            {
                pairList.Remove(pTemp);
            }

            _counters.IncPairCounter();
            pair.Id = _counters.PairCounter;

            pairList.Add(pair);
            JsonTools.InsertRecords(pairList, pairsPath);
            JsonTools.InsertOne(_counters, countersPath);

            return pair.Id;
        }

        public void RemovePair(Pair pair)
        {
            var pairList = JsonTools.LoadRecords<Pair>(pairsPath);
            var pTemp = pairList.Find(p => p.Id == pair.Id);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                pairList.Remove(pTemp);
                pTemp.IsDeleted = true;
                pTemp.DateOfDelete = DateTime.Now;
                pairList.Add(pTemp);

                JsonTools.InsertRecords(pairList, pairsPath);
                return;
            }

            throw new BadPairException("the pair is not found", pTemp != null ? pTemp.StudentFromIsraelId : 0,
                        pTemp != null ? pTemp.StudentFromWorldId : 0);
        }

        public void UpdatePair(Pair pair)
        {
            var pairList = JsonTools.LoadRecords<Pair>(pairsPath);
            var pTemp = pairList.Find(p => p.StudentFromIsraelId == pair.StudentFromIsraelId
                                                 && p.StudentFromWorldId == pair.StudentFromWorldId);
            if (pTemp != null && !pTemp.IsDeleted)
            {
                pairList.Remove(pTemp);
                pTemp.DateOfUpdate = DateTime.Now;
                pairList.Add(pTemp);

                JsonTools.InsertRecords(pairList, pairsPath);
                return;
            }

            throw new BadPairException("the pair is not found", pTemp != null ? pTemp.StudentFromIsraelId : 0,
                        pTemp != null ? pTemp.StudentFromWorldId : 0);
        }

        public IEnumerable<Pair> GetAllPairs()
        {
            var pairs = JsonTools.LoadRecords<Pair>(pairsPath);
            return from p in pairs
                   where !p.IsDeleted
                   select p;
        }

        public Pair GetPair(int id)
        {
            var pairList = JsonTools.LoadRecords<Pair>(pairsPath);
            var result = pairList.Find(p => p.Id == id);
            if(result != null)
            {
                return result;
            }
            throw new BadPairException("can not find the pair", id);
        }
        #endregion

        #region Last update of the data tables
        public void UpdateSheetsLastRange(SpredsheetLastRange lastDateOfSheets)
        {
            JsonTools.InsertOne(lastDateOfSheets, lastDateOfSheetsPath);
        }

        public SpredsheetLastRange GetSheetsLastRang()
        {
            var result = JsonTools.LoadOne<SpredsheetLastRange>(lastDateOfSheetsPath);
            return result ?? new SpredsheetLastRange();
        }
        #endregion
    }
}
