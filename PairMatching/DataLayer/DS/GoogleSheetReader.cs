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

namespace DS
{
    public class GoogleSheetReader
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        
        string applicationName = "PairMathcing";
        
        public static readonly string SpreadsheetIdHebrew = @"1iNKE8QeDxPqCkOvnmi4Qa7tiDCDjOQ6uDZ6Z_eL4b8Q";

        public static readonly string SheetInHebrew = "shalhevet in hebrew";

        SheetsService service;

        public GoogleSheetReader()
        {
            UserCredential credential;
            using(var stream =
                new FileStream(@"C:\Users\Asuspcc\source\Repos\PairMatching\PairMatching\bin\credentials.json", FileMode.Open, FileAccess.Read))
            {
                //credential = GoogleCredential.FromStream(stream)
                //  .CreateScoped(Scopes);
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;

                service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = applicationName
                });
            }
        }

        public IList<IList<object>> ReadEntries(string sheetName, string spreadsheetId)
        {
            var rang = $"{sheetName}!B2:P";
            var request = service.Spreadsheets.Values.Get(spreadsheetId, rang);

            var response = request.Execute();

            var values = response.Values;

            if(values != null && values.Count > 0)
            {
                return values;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ReadSheet()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(@"credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Pair Matching",
            });

            // Define request parameters.
            string spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            string range = "Class Data!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
        }
    }
}
