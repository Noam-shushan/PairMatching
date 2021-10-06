using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;


namespace LogicLayer
{
    /// <summary>
    /// Google Sheet parser that descript the values in the spreadsheet table.<br/>
    /// Save the descripting objects to the database.
    /// </summary>
    public class GoogleSheetParser
    {   
        private readonly IDataLayer dal = DalFactory.GetDal();

        private const int ROW_SIZE = 26;
        private const int TIME_COLUMN_START = 2;
        private const int TIME_COLUMN_END = 7;

        /// <summary>
        /// indexer of the values in the hebrow spreadsheet
        /// </summary>
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

        /// <summary>
        /// indexer of the values in the english spreadsheet
        /// </summary>
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
        
        // Reader for the spreadsheets
        private readonly GoogleSheetReader sheetReader;

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
                        DateOfRegistered = DateTime.Parse(row[0]),
                        Id = id,
                        Name = row[indexHebSheet["Name"]],
                        DesiredLearningTime = GetLearningTime(row, studentDescriptor),
                        OpenQuestions = GetQandAheb(row),
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
                    }) ;
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
                        DateOfRegistered = DateTime.Parse(row[0]),
                        Id = id,
                        Name = row[indexEngSheet["Name"]],
                        DesiredLearningTime = GetLearningTime(row, studentDescriptor),
                        OpenQuestions = GetQandAeng(row),
                        PrefferdTracks = studentDescriptor.GetPrefferdTracks(row[indexEngSheet["PrefferdTracks"]]),
                        PrefferdGender = studentDescriptor.GetPrefferdGender(row[indexEngSheet["PrefferdGender"]]),
                        DesiredEnglishLevel = studentDescriptor.GetEnglishLevel(row[indexEngSheet["DesiredEnglishLevel"]]),
                        SkillLevel = studentDescriptor.GetSkillLevel(row[indexEngSheet["SkillLevel"]]),
                        LearningStyle = studentDescriptor.GetLearningStyle(row[indexEngSheet["LearningStyle"]]),
                        Gender = studentDescriptor.GetGender(row[indexEngSheet["Gender"]]),
                        Country = studentDescriptor.GetCountryName(row[indexEngSheet["Country"]]).SpliceText(3),
                        UtcOffset = studentDescriptor.GetStudentOffset(row[indexEngSheet["UtcOffset"]]),
                        PhoneNumber = row[indexEngSheet["PhoneNumber"]],
                        Email = row[indexEngSheet["Email"]],
                        MoreLanguages = studentDescriptor.GetMoreLanguages(row[indexEngSheet["MoreLanguages"]]),
                        Languages = studentDescriptor.GetLanguages(row[indexEngSheet["Languages"]]),
                        PrefferdNumberOfMatchs = studentDescriptor
                            .GetPrefferdNumberOfMatchs(row[indexEngSheet["PrefferdNumberOfMatchs"]])

                    });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return DateTime.Parse(tableStr.Last()[0]);
        }

        private static IEnumerable<DO.LearningTime> GetLearningTime(List<string> row, IStudentDescriptor studentDescriptor)
        {
            var result = new List<DO.LearningTime>();
            for (int i = TIME_COLUMN_START; i < TIME_COLUMN_END; i++)
            {
                result.Add(new DO.LearningTime
                {
                    Day = studentDescriptor.GetDay(i),
                    TimeInDay = studentDescriptor.GetTimesInDey(row[i])
                });
            }
            return result;
        }

        private static IEnumerable<DO.OpenQuestion> GetQandAheb(List<string> row)
        {
            var result = new List<DO.OpenQuestion>();
            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexHebSheet["Personal information"]],
                Question = "Personal information"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexHebSheet["What are your hopes and expectations from this program"]],
                Question = "What are your hopes and expectations from this program"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexHebSheet["Personality trates"]],
                Question = "Personality trates"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexHebSheet["Who introduced you to this program"]],
                Question = "Who introduced you to this program"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexHebSheet["Additional information"]],
                Question = "Additional information"
            });

            return result;
        }

        private static IEnumerable<DO.OpenQuestion> GetQandAeng(List<string> row)
        {
            var result = new List<DO.OpenQuestion>();
            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["Personal information"]],
                Question = "Personal information"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["What are your hopes and expectations from this program"]],
                Question = "What are your hopes and expectations from this program"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["Personality trates"]],
                Question = "Personality trates"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["Who introduced you to this program"]],
                Question = "Who introduced you to this program"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["Additional information"]],
                Question = "Additional information"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["Country and City of residence"]],
                Question = "Country and City of residence"
            });

            result.Add(new DO.OpenQuestion
            {
                Answer = row[indexEngSheet["Anything else you would like to tell us"]],
                Question = "Anything else you would like to tell us"
            });
            return result;
        }
    }
}
