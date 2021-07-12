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
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        readonly string ApplicationName = "PairMathcing";

        SheetsService Service;

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
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public async Task<IList<IList<object>>> ReadEntriesAsync(string spreadsheetId, string range)
        {
            var request = Service.Spreadsheets.Values.Get(spreadsheetId, range);

            //var response = await Task.Run(() => request.Execute());

            var response = await request.ExecuteAsync();

            var values = response.Values;

            if (values != null && values.Count > 0)
            {
                return values;
            }
            return null;
        }

        public IList<IList<object>> ReadEntries(string spreadsheetId, string range)
        {
            var request = Service.Spreadsheets.Values.Get(spreadsheetId, range);

            var response = request.Execute();

            var values = response.Values;

            if (values != null && values.Count > 0)
            {
                return values;
            }
            return null;
        }
    }
}
