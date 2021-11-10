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
        private readonly Counters _counters = Counters.Instance;

        private SpredsheetLastRange _lastRangeOfSheets = new SpredsheetLastRange();

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
                student.Id = GetNewStudentId();
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
        #endregion

        #region Last update of the data tables
        public void UpdateSheetsLastRange(SpredsheetLastRange lastDateOfSheets)
        {
            try
            {
                _lastRangeOfSheets = lastDateOfSheets;
                db
                    .UpsertRecord(countersAndLastDataOfSpredsheetTable, lastDateOfSheets.Id, lastDateOfSheets);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public SpredsheetLastRange GetSheetsLastRang()
        {
            try
            {
                var lastRange = db
                    .LoadeRecordById<SpredsheetLastRange>(countersAndLastDataOfSpredsheetTable, _lastRangeOfSheets.Id);
                if(lastRange is null)
                {
                    return _lastRangeOfSheets;
                }
                _lastRangeOfSheets = lastRange;
                return lastRange;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
