using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.Json;
using System.Text.Json;

using Serilog;

using SqlServerWeb.DatabaseHelpers;
using Serilog.Formatting.Json;

namespace SqlServerWeb.Pages
{
    public class SalesTaxByStateViewModel : PageModel
    {
        public string? message { get; set; }
        public List<TaxRateByState>? salesTaxModel { get; set; } = default!;


        public async Task<IActionResult> OnGet()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(renderMessage: true), "log.json")
                .CreateLogger();

            Log.Information("Loading Page");
            ViewData["Title"] = "SalesTaxByState Page";
            message = "Hello Scot";

            try
            {
                var databaseQuery = new DatabaseQueries();
                salesTaxModel = await databaseQuery.GetStateTaxInfo();
                Log.Information("Tax Model Returned : " + salesTaxModel.Count.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return Page();
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
