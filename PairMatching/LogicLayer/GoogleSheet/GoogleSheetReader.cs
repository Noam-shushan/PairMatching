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

namespace LogicLayer
{
    public class GoogleSheetReader
    {
        private static readonly string[] scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        
        private readonly string applicationName = "PairMathcing";
        
        private readonly SheetsService service;

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

        public IList<IList<object>> ReadEntries(IStudentDescriptor studentDescriptor)
        {
            return ReadEntries(studentDescriptor.SpreadsheetId, studentDescriptor.Range); 
        }
    }
}
