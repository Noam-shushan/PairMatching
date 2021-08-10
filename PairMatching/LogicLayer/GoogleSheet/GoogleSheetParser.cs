using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataLayer;


namespace LogicLayer
{
    public class GoogleSheetParser
    {   
        private static readonly IDataLayer dal = DalFactory.GetDal("json");

        private static readonly int ROW_SIZE = 26;

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
            {"Additional information", 19},
            {"PrefferdNumberOfMatchs", 20 },
            {"MoreLanguages", 21 },
            {"Languages", 22 }
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
                {"Who introduced you to this program", 22},
                {"PrefferdNumberOfMatchs", 23 },
                {"MoreLanguages", 24 },
                {"Languages", 25 }
            };
        
        private readonly GoogleSheetReader sheetReader;

        private static readonly int TIME_COLUMN_START = 2;
        private static readonly int TIME_COLUMN_END = 7;

        /// <summary>
        /// Constructor for GoogleSheetParser
        /// Set the GoogleSheetReader that get the key for reading the spredsheets.
        /// Clears data left over from previous reading
        /// <throw></throw>
        /// </summary>
        public GoogleSheetParser()
        {
            try
            {
                sheetReader = new GoogleSheetReader();
                DataSource.ClearLists();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Fix the size of the column for each row in the table to be at the same size 
        /// and return new table with string values
        /// </summary>
        /// <param name="table">original table</param>
        /// <returns>new table with string values</returns>
        private List<List<string>> GetFixdStrTable(IEnumerable<IList<object>> table)
        {
            List<List<string>> tableResult = new List<List<string>>();
            foreach (var row in table)
            {
                // get the row in string values
                var rowStr = (from c in row
                            select c.ToString())
                            .ToList();
                
                if (row.Count < ROW_SIZE)
                {
                    for (int i = 0; i <= ROW_SIZE - row.Count; i++)
                    {
                        // append empty string to the end of the row
                        // just to fix the row size
                        rowStr = rowStr.Append("")
                            .ToList();
                    }
                }
                tableResult.Add(rowStr);
            }
            return tableResult;
        }

        /// <summary>
        /// Read the data from the Google sheet.
        /// Descript the data and create objects from the data.
        /// </summary>
        /// <param name="studentDescriptor">Descriptor from the data</param>
        /// <returns>the last data of update of the spredsheet</returns>
        public async Task<DateTime> ReadAsync(IStudentDescriptor studentDescriptor)
        {
            DateTime result = DateTime.Now;
            await Task.Run(() =>
            {
                var googleSheetValue = sheetReader.ReadEntries(studentDescriptor);

                if (googleSheetValue == null)
                    throw new Exception("canot parse the sheet file");


                // get the rows that after the last data of update
                var table = from r in googleSheetValue
                            where DateTime.Parse(r[0].ToString()) > studentDescriptor.LastUpdate
                            select r;

                if (!table.Any())
                {
                    result = studentDescriptor.LastUpdate;
                    return;
                }

                var tableStr = GetFixdStrTable(table);

                if(studentDescriptor is HebrewDescriptor)
                {
                    result = CreateDataFromHebrewSheet(tableStr, studentDescriptor);
                }
                else if(studentDescriptor is EnglishDiscriptor)
                {
                    result = CreateDataFromEnglishSheet(tableStr, studentDescriptor);
                }
            });
            return result;
        }

        private DateTime CreateDataFromHebrewSheet(List<List<string>> tableStr, IStudentDescriptor studentDescriptor)
        {
            foreach (var row in tableStr)
            {
                try
                {
                    int id = dal.GetNewStudentId();
                    DataSource.StudentsList.Add(new DO.Student
                    {
                        Id = id,
                        Name = row[indexHebSheet["Name"]],
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexHebSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexHebSheet["PrefferdGender"]]),
                        EnglishLevel = studentDescriptor.GetEnglishLevel(row[indexHebSheet["EnglishLevel"]]),
                        DesiredSkillLevel = studentDescriptor.GetSkillLevel(row[indexHebSheet["DesiredSkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[indexHebSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[indexHebSheet["Gender"]]),
                        Country = studentDescriptor.GetCountryName(null),
                        PhoneNumber = row[indexHebSheet["PhoneNumber"]],
                        Email = row[indexHebSheet["Email"]],
                        MoreLanguages = studentDescriptor.GetMoreLanguages(row[indexHebSheet["MoreLanguages"]]),
                        Languages = studentDescriptor.GetLanguages(row[indexHebSheet["Languages"]]),
                        PrefferdNumberOfMatchs = studentDescriptor
                            .GetPrefferdNumberOfMatchs(row[indexHebSheet["PrefferdNumberOfMatchs"]])
                    });
                    AddLearningTime(id, row, studentDescriptor);
                    QandAheb(id, row);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return DateTime.Parse(tableStr.Last()[0]);
        }

        private DateTime CreateDataFromEnglishSheet(List<List<string>> tableStr, IStudentDescriptor studentDescriptor)
        {
            foreach (var row in tableStr)
            {
                try
                {
                    int id = dal.GetNewStudentId();
                    DataSource.StudentsList.Add(new DO.Student
                    {
                        Id = id,
                        Name = row[indexEngSheet["Name"]],
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexEngSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexEngSheet["PrefferdGender"]]),
                        DesiredEnglishLevel = studentDescriptor.GetEnglishLevel(row[indexEngSheet["DesiredEnglishLevel"]]),
                        SkillLevel = studentDescriptor.GetSkillLevel(row[indexEngSheet["SkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[indexEngSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[indexEngSheet["Gender"]]),
                        Country = SpliceText(studentDescriptor.GetCountryName(row[indexEngSheet["Country"]]), 3),
                        UtcOffset = studentDescriptor.GetStudentOffset(row[indexEngSheet["UtcOffset"]]),
                        PhoneNumber = row[indexEngSheet["PhoneNumber"]],
                        Email = row[indexEngSheet["Email"]],
                        MoreLanguages = studentDescriptor.GetMoreLanguages(row[indexEngSheet["MoreLanguages"]]),
                        Languages = studentDescriptor.GetLanguages(row[indexEngSheet["Languages"]]),
                        PrefferdNumberOfMatchs = studentDescriptor
                            .GetPrefferdNumberOfMatchs(row[indexEngSheet["PrefferdNumberOfMatchs"]])
                    });
                    AddLearningTime(id, row, studentDescriptor);
                    QandAeng(id, row);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return DateTime.Parse(tableStr.Last()[0]);
        }

        private static void AddLearningTime(int id, List<string> row, IStudentDescriptor studentDescriptor)
        {
            for (int i = TIME_COLUMN_START; i < TIME_COLUMN_END; i++)
            {
                DataSource.LearningTimesList.Add(new DO.LearningTime
                {
                    Id = id,
                    Day = studentDescriptor.GetDay(i),
                    TimeInDay = studentDescriptor.GetTimesInDey(row[i])
                });
            }
        }

        private static void QandAheb(int id, List<string> row)
        {
            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexHebSheet["Personal information"]].ToString()),
                Question = "Personal information"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexHebSheet["What are your hopes and expectations from this program"]].ToString()),
                Question = "What are your hopes and expectations from this program"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexHebSheet["Personality trates"]].ToString()),
                Question = "Personality trates"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexHebSheet["Who introduced you to this program"]].ToString()),
                Question = "Who introduced you to this program"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexHebSheet["Additional information"]].ToString()),
                Question = "Additional information"
            });
        }

        private static void QandAeng(int id, List<string> row)
        {
            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["Personal information"]].ToString()),
                Question = "Personal information"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["What are your hopes and expectations from this program"]].ToString()),
                Question = "What are your hopes and expectations from this program"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["Personality trates"]].ToString()),
                Question = "Personality trates"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["Who introduced you to this program"]].ToString()),
                Question = "Who introduced you to this program"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["Additional information"]].ToString()),
                Question = "Additional information"
            });

            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["Country and City of residence"]].ToString()),
                Question = "Country and City of residence"
            });


            DataSource.OpenQuestionsList.Add(new DO.OpenQuestion
            {
                Id = id,
                Answer = SpliceText(row[indexEngSheet["Anything else you would like to tell us"]].ToString()),
                Question = "Anything else you would like to tell us"
            });
            
        }

        private static string SpliceText(string text, int n = 8)
        {
            return string.Join(Environment.NewLine, text.Split()
                .Select((word, index) => new { word, index })
                .GroupBy(x => x.index / n)
                .Select(grp => string.Join(" ", grp.Select(x => x.word))));
        }
    }
}
