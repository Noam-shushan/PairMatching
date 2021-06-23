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

        static Dictionary<string, int> indexHebSheet = new Dictionary<string, int>();
        static Dictionary<string, int> indexEngSheet = new Dictionary<string, int>();

        static GoogleSheetParser()
        {
            setDict();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        public static DateTime UpdateDataInHebrew(DO.LastDateOfSheets lastUpdate)
        {
            GoogleSheetReader sheetReader = new GoogleSheetReader();
            IStudentDescriptor studentDescriptor = new HebrewDescriptor();

            var googleSheetValue = sheetReader.ReadEntries(studentDescriptor.SpreadsheetId,
                studentDescriptor.Range);

            if (googleSheetValue == null)
                throw new Exception("canot parse the sheet file");


            // get the rows that after the last data of update
            var table = from r in googleSheetValue
                        where DateTime.Parse(r[0].ToString()) > lastUpdate.HebrewSheets
                        select r;

            if (!table.Any())
            {
                return lastUpdate.HebrewSheets;
            }

            List<BO.Student> studentsList = new List<BO.Student>();
            foreach (var row in table)
            {
                try
                {
                    int id = bl.AddStudent(new BO.Student
                    {
                        Name = row[indexHebSheet["Name"]].ToString(),
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexHebSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexHebSheet["PrefferdGender"]]),
                        EnglishLevel = studentDescriptor.GetEnglishLevel(row[indexHebSheet["EnglishLevel"]]),
                        DesiredSkillLevel = studentDescriptor.GetSkillLevel(row[indexHebSheet["DesiredSkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[indexHebSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[indexHebSheet["Gender"]]),
                        Country = "Israel",
                        PhoneNumber = row[indexHebSheet["PhoneNumber"]].ToString(),
                        Email = row[indexHebSheet["Email"]].ToString()
                    });
                    for (int i = 2; i < 7; i++)
                    {
                        dal.AddLearningTime(new DO.LearningTime
                        {
                            Id = id,
                            Day = studentDescriptor.GetDay(i),
                            TimeInDay = studentDescriptor.GetTimesInDey(row[i])
                        });
                    }
                }
                catch (Exception)
                {
                    
                }
            }

            return DateTime.Parse(table.Last()[0].ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        public static DateTime UpdateDataInEnglish(DO.LastDateOfSheets lastUpdate)
        {
            GoogleSheetReader sheetReader = new GoogleSheetReader();
            IStudentDescriptor studentDescriptor = new EnglishDiscriptor();

            var googleSheetValue = sheetReader.ReadEntries(studentDescriptor.SpreadsheetId,
                studentDescriptor.Range);

            if (googleSheetValue == null)
                throw new Exception("canot parse the sheet file");

            // get the rows that after the last data of update
            var table = from r in googleSheetValue
                        where DateTime.Parse(r[0].ToString()) > lastUpdate.EnglishSheets
                        select r;

            if (!table.Any())
            {
                return lastUpdate.EnglishSheets;
            }

            List<DO.LearningTime> learningTimesList = new List<DO.LearningTime>();
            List<BO.Student> studentsList = new List<BO.Student>();
            
            foreach (var row in table)
            {
                int id = bl.AddStudent(new BO.Student
                {
                    Name = row[indexEngSheet["Name"]].ToString(),
                    PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexEngSheet["PrefferdTracks"]]),
                    PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexEngSheet["PrefferdGender"]]),
                    DesiredEnglishLevel = studentDescriptor.GetEnglishLevel(row[indexEngSheet["DesiredEnglishLevel"]]),
                    SkillLevel = studentDescriptor.GetSkillLevel(row[indexEngSheet["SkillLevel"]]),
                    LearningStyle = studentDescriptor.GetLearningStyle(row[indexEngSheet["LearningStyle"]]),
                    Gender = studentDescriptor.GetGender(row[indexEngSheet["Gender"]]),
                    Country = row[indexEngSheet["Country"]].ToString(),
                    UtcOffset = studentDescriptor.GetStudentOffset(row[indexEngSheet["UtcOffset"]]),
                    PhoneNumber = row[indexEngSheet["PhoneNumber"]].ToString(),
                    Email = row[indexEngSheet["Email"]].ToString()
                });

                for(int i = 2; i < 7; i++)
                {
                    dal.AddLearningTime(new DO.LearningTime
                    {
                        Id = id,
                        Day = studentDescriptor.GetDay(i),
                        TimeInDay = studentDescriptor.GetTimesInDey(row[i])                      
                    });
                }

            }

            return DateTime.Parse(table.Last()[0].ToString());
        }

        static void setDict()
        {
            indexHebSheet.Add("Name", 1);
            indexHebSheet.Add("PrefferdTracks", 7);
            indexHebSheet.Add("PrefferdGender", 8);
            indexHebSheet.Add("EnglishLevel", 9);
            indexHebSheet.Add("DesiredSkillLevel", 10);
            indexHebSheet.Add("LearningStyle", 11);
            indexHebSheet.Add("Gender", 12);
            indexHebSheet.Add("PhoneNumber", 13);
            indexHebSheet.Add("Email", 14);

            indexEngSheet.Add("Name", 1);
            indexEngSheet.Add("PrefferdTracks", 7);
            indexEngSheet.Add("PrefferdGender", 8);
            indexEngSheet.Add("DesiredEnglishLevel", 9);
            indexEngSheet.Add("SkillLevel", 10);
            indexEngSheet.Add("LearningStyle", 11);
            indexEngSheet.Add("Gender", 12);
            indexEngSheet.Add("Country", 13);
            indexEngSheet.Add("UtcOffset", 13);
            indexEngSheet.Add("PhoneNumber", 14);
            indexEngSheet.Add("Email", 15);
        }
    }
}
