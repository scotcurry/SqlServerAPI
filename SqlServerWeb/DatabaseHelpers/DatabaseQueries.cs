using SqlServerWeb.Pages;
using System.Text.Json;

namespace SqlServerWeb.DatabaseHelpers
{
    public class DatabaseQueries
    {
        public async Task<string> GetChuckJoke()
        {
            HttpClient client = new HttpClient();
            var returnJSON = String.Empty;
            var chuckJokeSite = "https://api.chucknorris.io/jokes/random";
            var response = await client.GetAsync(chuckJokeSite);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    returnJSON = await response.Content.ReadAsStringAsync();
                }
            }

            return returnJSON;
        }

        public async Task<List<TaxRateByState>> GetStateTaxInfo()
        {
            HttpClient client = new HttpClient();
            var complexQueryJson = String.Empty;
            var databaseApiSite = "https://win2019server.curryware.org/api/Database";
            var response = await client.GetAsync(databaseApiSite);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    complexQueryJson = await response.Content.ReadAsStringAsync();
                }
            }

            var statesToReturn = ParseDatabaseJSON(complexQueryJson);
            return statesToReturn!;
        }

        private List<TaxRateByState>? ParseDatabaseJSON(string complexQueryJSON)
        {
            var stateList = new List<TaxRateByState>();
            stateList = JsonSerializer.Deserialize<List<TaxRateByState>>(complexQueryJSON);

            if (stateList == null)
            {
                var stateTaxError = new TaxRateByState();
                stateTaxError.Name = "Error";
            }
            else
            {
                return stateList;
            }
            return stateList;
        }
    }
}
