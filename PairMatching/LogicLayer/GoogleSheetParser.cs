using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace LogicLayer
{
    public class GoogleSheetParser
    {   
        private static readonly DataLayer.IDataLayer dal = DataLayer.DalFactory.GetDal("json");
        
        private static readonly Dictionary<string, int> indexHebSheet = new Dictionary<string, int>()
        {
            {"Name", 1 },
            {"PrefferdTracks", 7},
            {"PrefferdGender", 8},
            {"EnglishLevel", 9},
            {"DesiredSkillLevel", 10},
            {"LearningStyle", 11},
            {"Gender", 12},
            {"PhoneNumber", 13},
            {"Email", 14},
            {"Personal information", 15},
            {"What are your hopes and expectations from this program", 16},
            {"Personality trates", 17},
            {"Who introduced you to this program", 18},
            {"Additional information", 19}
        };

        private static readonly Dictionary<string, int> indexEngSheet = 
            new Dictionary<string, int>()
            {
                {"Name", 1 },
                {"PrefferdTracks", 7},
                {"PrefferdGender", 8},
                {"DesiredEnglishLevel", 9},
                {"SkillLevel", 10},
                {"LearningStyle", 11},
                {"Gender", 12},
                {"Country", 13},
                {"UtcOffset", 13},
                {"PhoneNumber", 14},
                {"Email", 15},
                {"Country and City of residence", 16},
                {"Personal information", 17},
                {"Personality trates", 18},
                {"Additional information", 19},
                {"What are your hopes and expectations from this program", 20},
                {"Anything else you would like to tell us", 21},
                {"Who introduced you to this program", 22}
            };
        
        private readonly GoogleSheetReader sheetReader;
        
        private static readonly int TIME_COLUMN_START = 2;      
        private static readonly int TIME_COLUMN_END = 7;

        public GoogleSheetParser()
        {
            try
            {
                sheetReader = new GoogleSheetReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        public DateTime UpdateDataInHebrew(DO.LastDateOfSheets lastUpdate)
        {

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

            foreach (var row in table)
            {
                try
                {
                    int id = dal.AddStudent(new DO.Student
                    {
                        Name = row[indexHebSheet["Name"]].ToString(),
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexHebSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexHebSheet["PrefferdGender"]]),
                        EnglishLevel = studentDescriptor.GetEnglishLevel(row[indexHebSheet["EnglishLevel"]]),
                        DesiredSkillLevel = studentDescriptor.GetSkillLevel(row[indexHebSheet["DesiredSkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[indexHebSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[indexHebSheet["Gender"]]),
                        Country = studentDescriptor.GetCountryName(null),
                        PhoneNumber = row[indexHebSheet["PhoneNumber"]].ToString(),
                        Email = row[indexHebSheet["Email"]].ToString()
                    });
                    addLearningTime(id, row, studentDescriptor);
                    QandAheb(id, row);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return DateTime.Parse(table.Last()[0].ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastUpdate"></param>
        /// <returns></returns>
        public DateTime UpdateDataInEnglish(DO.LastDateOfSheets lastUpdate)
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
            
            foreach (IList<object> row in table)
            {
                try
                {
                    int id = dal.AddStudent(new DO.Student
                    {
                        Name = row[indexEngSheet["Name"]].ToString(),
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexEngSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexEngSheet["PrefferdGender"]]),
                        DesiredEnglishLevel = studentDescriptor.GetEnglishLevel(row[indexEngSheet["DesiredEnglishLevel"]]),
                        SkillLevel = studentDescriptor.GetSkillLevel(row[indexEngSheet["SkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[indexEngSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[indexEngSheet["Gender"]]),
                        Country = studentDescriptor.GetCountryName(row[indexEngSheet["Country"]]),
                        UtcOffset = studentDescriptor.GetStudentOffset(row[indexEngSheet["UtcOffset"]]),
                        PhoneNumber = row[indexEngSheet["PhoneNumber"]].ToString(),
                        Email = row[indexEngSheet["Email"]].ToString()
                    });
                    addLearningTime(id, row, studentDescriptor);
                    QandAeng(id, row);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }

            return DateTime.Parse(table.Last()[0].ToString());
        }

        private static void addLearningTime(int id, IList<object> row, IStudentDescriptor studentDescriptor)
        {
            for (int i = TIME_COLUMN_START; i < TIME_COLUMN_END; i++)
            {
                dal.AddLearningTime(new DO.LearningTime
                {
                    Id = id,
                    Day = studentDescriptor.GetDay(i),
                    TimeInDay = studentDescriptor.GetTimesInDey(row[i])
                });
            }
        }

        private static void QandAheb(int id, IList<object> row)
        {
            try
            {
                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexHebSheet["Personal information"]].ToString()),
                    Question = "Personal information"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexHebSheet["What are your hopes and expectations from this program"]].ToString()),
                    Question = "What are your hopes and expectations from this program"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexHebSheet["Personality trates"]].ToString()),
                    Question = "Personality trates"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexHebSheet["Who introduced you to this program"]].ToString()),
                    Question = "Who introduced you to this program"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexHebSheet["Additional information"]].ToString()),
                    Question = "Additional information"
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                // some rows dont have the same rage
            }
        }

        private static void QandAeng(int id, IList<object> row)
        {
            try
            {
                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["Personal information"]].ToString()),
                    Question = "Personal information"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["What are your hopes and expectations from this program"]].ToString()),
                    Question = "What are your hopes and expectations from this program"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["Personality trates"]].ToString()),
                    Question = "Personality trates"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["Who introduced you to this program"]].ToString()),
                    Question = "Who introduced you to this program"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["Additional information"]].ToString()),
                    Question = "Additional information"
                });

                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["Country and City of residence"]].ToString()),
                    Question = "Country and City of residence"
                });


                dal.AddOpenQuestions(new DO.OpenQuestion
                {
                    Id = id,
                    Answer = SpliceText(row[indexEngSheet["Anything else you would like to tell us"]].ToString()),
                    Question = "Anything else you would like to tell us"
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                // some rows dont have the same rage
            }
        }

        private static string SpliceText(string text)
        {
            return string.Join(Environment.NewLine, text.Split()
                .Select((word, index) => new { word, index })
                .GroupBy(x => x.index / 6)
                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
        }
    }
}
