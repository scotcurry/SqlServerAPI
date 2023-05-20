using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.Json;

using SqlServerWeb.DatabaseHelpers;
using System.Text.Json;

namespace SqlServerWeb.Pages
{
    public class SalesTaxByStateViewModel : PageModel
    {
        public string? message { get; set; }
        public List<TaxRateByState>? salesTaxModel { get; set; } = default!;


        public async Task<IActionResult> OnGet()
        {
            ViewData["Title"] = "SalesTaxByState Page";
            message = "Hello Scot";

            var databaseQuery = new DatabaseQueries();
            salesTaxModel = await databaseQuery.GetStateTaxInfo();

            return Page();
        }

        static private List<TaxRateByState>? ParseDatabaseJSON(string complexQueryJSON)
        {
            var stateList = new List<TaxRateByState>();
            stateList = JsonSerializer.Deserialize<List<TaxRateByState>>(complexQueryJSON);

            return stateList;
            
        }
    }

    public class TaxRateByState
    {
        public string? Name { get; set; }
        public decimal TaxRate { get; set; }
        public byte? TaxType { get; set; }
        public string? StateName { get; set; }
    }
}
