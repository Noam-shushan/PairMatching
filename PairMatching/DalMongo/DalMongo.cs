using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using DO;

namespace DalMongo
{
    // TODO: add documentation
    public class DalMongo : IDataLayer
    {
        private static readonly MongoCrud db = new MongoCrud("Shalhevet");

        #region Singleton
        public static IDataLayer Instance { get; } = new DalMongo();

        private DalMongo()
        {
            var countersTemp = db.LoadeRecordById<Counters>(countersAndLastDataOfSpredsheetTable, _counters.Id);
            if(countersTemp != null)
            {
                _counters = countersTemp;
            }
        }
        #endregion

        #region paths
        private readonly string studentsTable = "Students";
        private readonly string pairsTable = "Pairs";
        private readonly string countersAndLastDataOfSpredsheetTable = "CountersAndLastDataOfSpredsheet";
        #endregion

        #region Counters
        private Counters _counters = Counters.Instance;

        private LastDataOfSpredsheet _lastDateOfSheets = new LastDataOfSpredsheet();

        #endregion

        public async Task SaveAllDataFromSpredsheetAsync()
        {
            await db.InsertManyAsync(studentsTable, DataSource.StudentsList);
            db.UpsertRecord(countersAndLastDataOfSpredsheetTable, _counters.Id, _counters);
        }

        #region Student
        public int GetNewStudentId()
        {
            _counters.IncStudentCounter();
            return _counters.StudentCounter;
        }

        public int AddStudent(Student student)
        {
            try
            {
                _counters.IncStudentCounter();
                student.Id = _counters.StudentCounter;
                db.UpsertRecord(countersAndLastDataOfSpredsheetTable, _counters.Id, _counters);
                db.InsertRecord(studentsTable, student);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return student.Id;
        }

        public void RemoveStudent(int id)
        {
            Student student;
            try
            {
                student = db.LoadeRecordById<Student>(studentsTable, id);
                if (student != null && !student.IsDeleted)
                {
                    student.IsDeleted = true;
                    db.UpsertRecord(studentsTable, id, student);
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            throw new BadStudentException($"can not find studet number {id} or this student is deletet", id);
        }

        public Student GetStudent(int id)
        {
            Student student;
            try
            {
                student = db.LoadeRecordById<Student>(studentsTable, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (student != null && !student.IsDeleted)
            {
                return student;
            }
            throw new BadStudentException($"can not find studet number {id} or this student is deletet", id);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            try
            {
                var students = db.LoadeRecords<Student>(studentsTable);
                return from s in students
                       where !s.IsDeleted
                       select s;
            }
            catch (Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Student> GetAllStudentsBy(Predicate<Student> predicate)
        {
            try
            {
                var students = db.LoadeRecords<Student>(studentsTable);
                return from s in students
                       where predicate(s) && !s.IsDeleted
                       select s;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateStudent(Student student)
        {
            try
            {
                db.UpsertRecord(studentsTable, student.Id, student);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Pair
        public int AddPair(Pair pair)
        {
            try
            {
                _counters.IncPairCounter();
                pair.Id = _counters.PairCounter;
                
                db.UpsertRecord(countersAndLastDataOfSpredsheetTable, _counters.Id, _counters);
                db.InsertRecord(pairsTable, pair);
                
                return pair.Id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemovePair(Pair pair)
        {
            try
            {
                pair.IsDeleted = true;
                pair.DateOfDelete = DateTime.Now;
                db.UpsertRecord(pairsTable, pair.Id, pair);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdatePair(Pair pair)
        {
            try
            {
                pair.DateOfUpdate = DateTime.Now;
                db.UpsertRecord(pairsTable, pair.Id, pair);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Pair> GetAllPairs()
        {
            try
            {
                var pairList = db.LoadeRecords<Pair>(pairsTable);
                return from p in pairList
                       where !p.IsDeleted
                       select p;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Pair> GetAllPairsBy(Predicate<Pair> predicate)
        {
            try
            {
                var pairList = db.LoadeRecords<Pair>(pairsTable);
                return from p in pairList
                       where predicate(p) && !p.IsDeleted
                       select p;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Pair GetPair(int id)
        {
            Pair pair;
            try
            {
                pair = db.LoadeRecordById<Pair>(pairsTable, id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if(pair != null && !pair.IsDeleted)
            {
                return pair;
            }
            if(pair == null)
            {
                throw new BadPairException("can not find the pair", 0, 0);
            }
            throw new BadPairException("can not find the pair of is removed", 
                pair.StudentFromIsraelId, pair.StudentFromWorldId);

        }

        public Pair GetPair(int studFromIsraelId, int studFromWorldId)
        {
            Pair pair;
            try
            {
                pair = GetAllPairsBy(p => p.StudentFromIsraelId == studFromIsraelId
                    && p.StudentFromWorldId == studFromWorldId)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); 
            }
            if (pair != null)
            {
                return pair;
            }
            throw new BadPairException("can not find the pair", studFromWorldId, studFromIsraelId);
        }
        #endregion

        #region Last update of the data tables
        public void UpdateLastDateOfSheets(LastDataOfSpredsheet lastDateOfSheets)
        {
            try
            {
                _lastDateOfSheets = lastDateOfSheets;
                db
                    .UpsertRecord(countersAndLastDataOfSpredsheetTable, lastDateOfSheets.Id, lastDateOfSheets);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public LastDataOfSpredsheet GetLastDateOfSheets()
        {
            try
            {
                var lastDate = db
                    .LoadeRecordById<LastDataOfSpredsheet>(countersAndLastDataOfSpredsheetTable, _lastDateOfSheets.Id);
                if(lastDate is null)
                {
                    return _lastDateOfSheets;
                }
                lastDate.EnglishSheets = lastDate.EnglishSheets.ToLocalTime();
                lastDate.HebrewSheets = lastDate.HebrewSheets.ToLocalTime();
                _lastDateOfSheets = lastDate;
                return lastDate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
