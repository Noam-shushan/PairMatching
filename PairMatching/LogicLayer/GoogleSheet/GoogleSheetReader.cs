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
    public class GoogleSheetReader
    {
        // The scope i need is only to read
        private static readonly string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        
        private const string applicationName = "PairMathcing";
        
        private readonly SheetsService service;

        /// <summary>
        /// Constructor for the Reader.<br/>
        /// Create credential from service acount.
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
        /// <returns>Table of object values</returns>
        public IList<IList<object>> ReadEntries(string spreadsheetId, string range)
        {
            try
            {
                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

                var response = request.Execute();

                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    return values;
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
        /// <returns>Table of object values</returns>
        public IList<IList<object>> ReadEntries(IStudentDescriptor studentDescriptor)
        {
            return ReadEntries(studentDescriptor.SpreadsheetId, studentDescriptor.Range); 
        }
    }
}
