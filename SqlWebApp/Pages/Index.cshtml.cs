using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SqlWebApp.HttpHandler;
using Serilog;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.File;

namespace SqlWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var logPath = currentDirectory + "\\logs\\application.log";
            HttpHandlerController httpHandlerController = new HttpHandlerController();
            var returnCode = httpHandlerController.GetStateTaxInfo();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logPath)
                .CreateLogger();

            Log.Information("Running OnGet");
        }
    }
}