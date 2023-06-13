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
    }

    [Route("api/[controller]")]
    [ApiController]
    public class QueryController: ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            var salesOrderQuery = "SELECT OrderQty, Name, ListPrice FROM Sales.SalesOrderHeader JOIN Sales.SalesOrderDetail ON SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID JOIN Production.Product ON SalesOrderDetail.ProductID=Product.ProductID WHERE Sales.SalesOrderDetail.SalesOrderID=43659";
            var databaseHandler = new DatabaseConnection();
            string returnValue = databaseHandler.ExecuteQuery(salesOrderQuery);
            return returnValue;
            
        }
    }
}
