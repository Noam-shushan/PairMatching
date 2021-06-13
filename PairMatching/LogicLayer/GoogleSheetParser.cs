using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LogicLayer
{
    public class GoogleSheetParser
    {
        static IBL bl = BlFactory.GetBL();
        static DataLayer.IDataLayer dal = DataLayer.DalFactory.GetDal("json");

        public static List<DO.Pair> PairsList;
        public static List<DO.LearningTime> LearningTimesList = new List<DO.LearningTime>();

        public static DateTime UpdateData(IStudentDescriptor studentDescriptor, DateTime lastUpdate)
        {
            GoogleSheetReader sheetReader = new GoogleSheetReader();

            var googleSheetValue = sheetReader.ReadEntries(studentDescriptor.SpreadsheetId,
                studentDescriptor.Range);

            if (googleSheetValue == null)
                throw new Exception("canot parse the sheet file");

            var table = from r in googleSheetValue
                        where DateTime.Parse(r[0].ToString()) > lastUpdate
                        select r;           

            List<BO.Student> studentsList = new List<BO.Student>();
            foreach (var row in table)
            {
                studentsList.Add(new BO.Student
                {
                    Name = row[1].ToString(),
                    PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[7]),
                    PrefferdGender = studentDescriptor.GetPrefferdGender(row[8]),
                    EnglishLevel = studentDescriptor.GetEnglishLevel(row[9]),
                    DesiredSkillLevel = studentDescriptor.GetSkillLevel(row[10]),
                    LearningStyle = studentDescriptor.GetLearningStyle(row[11]),
                    Gender = studentDescriptor.GetGender(row[12]),
                    Country = row[13].ToString(),
                    PhoneNumber = row[14].ToString(),
                    Email = row[15].ToString()
                });
            }

            foreach(var s in studentsList)
            {
                bl.AddStudent(s);
            }

            return DateTime.Parse(table.Last()[0].ToString());
        }

/*        static void InitializationHebrew()
        {
            GoogleSheetReader sheetReader = new GoogleSheetReader();

            IStudentDescriptor sd = new HebrewDescriptor();
            var googleSheetValue = sheetReader.ReadEntries(HebrewDescriptor.SpreadsheetIdHebrew,
                HebrewDescriptor.Range);

            if (googleSheetValue == null)
                throw new Exception("canot parse the sheet file");

            foreach (var row in googleSheetValue)
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
                    PhoneNumber = row[14].ToString(),
                    Email = row[15].ToString()
                });
            }
        }*/
    }
}
