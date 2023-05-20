using Microsoft.AspNetCore.Mvc;

namespace SqlWebApp.HttpHandler
{
    public class HttpHandlerController : Controller
    {
        public IActionResult Index()
        {
            var returnCode = GetStateTaxInfo();
            ViewData["Title"] = "Hello Scot";
            ViewData["Scot"] = "Otis";
            return View();
        }

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

        public async Task<string> GetStateTaxInfo()
        {
            HttpClient client = new HttpClient();
            var returnJSON = String.Empty;
            var databaseApiSite = "https://win2019server.curryware.org/api/Database";
            var response = await client.GetAsync(databaseApiSite);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    returnJSON = await response.Content.ReadAsStringAsync();
                }
            }

            return returnJSON;
        }
    }
}
