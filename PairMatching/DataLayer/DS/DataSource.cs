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
            
            IStudentDescriptor sd = new HebrewDescriptor();
            var googleSheetValue = sheetReader.ReadEntries(HebrewDescriptor.SpreadsheetIdHebrew, 
                HebrewDescriptor.Range);

            if (googleSheetValue == null)
                throw new Exception("canot parse the sheet file");

            foreach(var row in googleSheetValue)
            {
                StudentsList.Add(new Student
                {
                    Name = row[1].ToString(),
                    PrefferdTracks = sd.GetPrefferdTracks(row[7]),
                    PrefferdGender = sd.GetPrefferdGender(row[8]),
                    EnglishLevel = sd.GetEnglishLevel(row[9]),
                    DesiredSkillLevel = sd.GetSkillLevel(row[10]),
                    LearningStyle = sd.GetLearningStyle(row[11]),
                    Gender = sd.GetGender(row[12]),
                    Country = row[13].ToString(),
                    PhoneNumber =  row[14].ToString(),
                    Email = row[15].ToString()                    
                }) ;
            }
        }
    }
}
