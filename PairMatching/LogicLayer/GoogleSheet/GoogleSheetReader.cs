using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace LogicLayer.GoogleSheet
{
    /// <summary>
    /// Read Google Sheet file using Google Sheet Api
    /// </summary>
    internal class GoogleSheetReader
    {
        // The scope i need is only to read
        private static readonly string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        
        private const string applicationName = "PairMathcing";
        
        private readonly SheetsService service;

        private const int ROW_SIZE = 26;

        /// <summary>
        /// Constructor for the Reader.<br/>
        /// Create credential from service account.
        /// </summary>
        public GoogleSheetReader()
        {
            GoogleCredential credential;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pairmatching.json");
            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    credential = GoogleCredential
                        .FromStream(stream)
                        .CreateScoped(scopes);
                }
                
                service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName,
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Read the spreadsheet from google sheet by spreadsheet id  and range of rows and colunms.
        /// </summary>
        /// <param name="spreadsheetId">The spreadsheet id</param>
        /// <param name="range">The spreadsheet range of rows and colunms</param>
        /// <returns>Table of string values</returns>
        public List<List<string>> ReadEntries(string spreadsheetId, string range)
        {
            try
            {
                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

                var response = request.Execute();

                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    return GetFixdStrTable(values);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("can not read the google sheets\n" + ex.Message);
            }
        }

        /// <summary>
        /// Read the spreadsheet from google sheet by a descriptor
        /// </summary>
        /// <param name="studentDescriptor">A student descriptor</param>
        /// <returns>Table of string values</returns>
        public List<List<string>> ReadEntries(IStudentDescriptor studentDescriptor)
        {
            return ReadEntries(studentDescriptor.SpreadsheetId, studentDescriptor.Range); 
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
    }
}
