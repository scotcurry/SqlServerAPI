using Microsoft.AspNetCore.Mvc;
using SqlServerAPI.DatabaseClasses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        // GET: api/<DatabaseScaffold>
        [HttpGet]
        public string Get()
        {
            var spName = "Sales.spTaxRateByState";
            var databaseHandler = new DatabaseConnection();
            string returnValue = databaseHandler.ExecuteStoredProcedure(spName);
            Console.Write(returnValue);
            return returnValue;
        }

        // GET api/<DatabaseScaffold>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DatabaseScaffold>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DatabaseScaffold>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DatabaseScaffold>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
