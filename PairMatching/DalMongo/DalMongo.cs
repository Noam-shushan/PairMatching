using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        //private static IDataLayer _instance;

        //private static readonly object _loke = new object();

        public static IDataLayer Instance { get; } = new DalMongo();
        //{
        //    get
        //    {
        //        if(_instance == null)
        //        {
        //            lock (_loke)
        //            {
        //                if(_instance == null)
        //                {
        //                    _instance = new DalMongo();
        //                }
        //            }
        //        }
        //        return _instance;
        //    } 
        //}

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

        private Student CopySafe(Student fromDB, Student fromSheet)
        {
            foreach (PropertyInfo propTo in fromDB.GetType().GetProperties())
            {
                if(propTo.Name == "MatchTo" || propTo.Name == "Name" || propTo.Name == "Email"
                    || propTo.Name == "Notes" || propTo.Name == "MatchingHistories"
                    || propTo.Name == "IsSimpleStudent" || propTo.Name == "Id")
                {
                    continue;
                }
                PropertyInfo propFrom = typeof(Student).GetProperty(propTo.Name);
                if (propFrom == null)
                {
                    continue;
                }

                var value = propFrom.GetValue(fromSheet, null);
                if (value != null)
                {
                    propTo.SetValue(fromDB, value);
                }
            }
            return fromDB;
        }

        private Student Clone(Student original)
        {
            Student copyToObject = new Student();

            foreach (PropertyInfo propertyInfo in typeof(Student).GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(copyToObject, propertyInfo.GetValue(original, null), null);
                }
            }

            return copyToObject;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            try
            {
                var students = db.LoadeRecords<Student>(studentsTable);
                //TODO fix some broken Data
                //var dops = from s in students
                //           group s by s.Name;

                //var l = from s1 in students
                //        where !s1.IsSimpleStudent
                //        from s2 in DataSource.StudentsList
                //        where s1.Email == s2.Email && s1.Name == s1.Name
                //        select CopySafe(Clone(s1), Clone(s2));

                //foreach(var s in l)
                //{
                //    db.UpsertRecord(studentsTable, s.Id, s);
                //}


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
