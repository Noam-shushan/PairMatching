using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS
{
    public static class DataSource
    {
        public static List<Student> StudentsList = new List<Student>();
        public static List<Pair> PairsList;
        public static List<LearningTime> LearningTimesList = new List<LearningTime>();

        static DataSource()
        {
            InitializationHebrew();
        }

        static void InitializationHebrew()
        {
            GoogleSheetReader sheetReader = new GoogleSheetReader();
            var googleSheetValue = sheetReader.ReadEntries(GoogleSheetReader.SheetInHebrew,
                GoogleSheetReader.SpreadsheetIdHebrew);
            IStudentDescriptor sd = new HebrewDescriptor();
            

            if (googleSheetValue == null)
                throw new Exception("canot parse the sheet file");

            foreach(var row in googleSheetValue)
            {
                StudentsList.Add(new Student
                {
                    Name = row[0].ToString(),
                    PrefferdTracks = sd.GetPrefferdTracks(row[6]),
                    PrefferdGender = sd.GetPrefferdGender(row[7]),
                    EnglishLevel = sd.GetEnglishLevel(row[8]),
                    DesiredSkillLevel = sd.GetSkillLevel(row[9]),
                    LearningStyle = sd.GetLearningStyle(row[10]),
                    Gender = sd.GetGender(row[11]),
                    Country = row[12].ToString(),
                    PhoneNumber =  row[13].ToString(),
                    Email = row[14].ToString()                    
                }) ;
            }
        }
    }
}
