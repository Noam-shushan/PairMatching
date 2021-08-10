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
            UserCredential credential;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client_secret.json");
            try
            {
                using (var stream =
            new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
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
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            var response = request.Execute();
            
            var values = response.Values;

            if (values != null && values.Count > 0)
            {
                return values;
            }
            return null;
        }

        public IList<IList<object>> ReadEntries(IStudentDescriptor studentDescriptor)
        {
            return ReadEntries(studentDescriptor.SpreadsheetId, studentDescriptor.Range); 
        }
    }
}
