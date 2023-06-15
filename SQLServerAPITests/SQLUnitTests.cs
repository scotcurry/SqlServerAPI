using System.Text.Json;
using SqlServerAPI.DatabaseClasses;
using SqlServerAPI.Classes;
using System.Xml.Linq;

namespace SQLServerAPITests
{
    [TestClass]
    public class SQLUnitTests
    {
        [TestMethod]
        public void TestGetConnectionString()
        {
            var connectionString = DatabaseConnection.ConnectionString;

            Assert.AreSame(connectionString, "Data Source=localhost;Initial Catalog=AdventureWorks2019;User ID=scot;Password=AirWatch1;");
        }

        [TestMethod]
        public void TestConnectToDatabase()
        {;
            var connection = new DatabaseConnection();
            var connectionType = typeof(DatabaseConnection);
            Assert.AreSame(connection.GetType(), connectionType);
        }

        [TestMethod]
        public void TestSimpleQuery()
        {
           
            var queryString = "SELECT OrderQty, Name, ListPrice FROM Sales.SalesOrderHeader JOIN Sales.SalesOrderDetail ON SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID " +
                "JOIN Production.Product ON SalesOrderDetail.ProductID = Product.ProductID WHERE Sales.SalesOrderDetail.SalesOrderID = 43659";

            var databaseHandler = new DatabaseConnection();
            string dbValue = databaseHandler.ExecuteQuery(queryString);
            var deserializedJSON = JsonSerializer.Deserialize<ProductOrderList>(dbValue);
            var totalEmployees = deserializedJSON?.productOrders?.Count;
            Assert.AreNotEqual(0, totalEmployees); 
        }

        [TestMethod]
        public void TestStoredProcedure() 
        {
            var spName = "Sales.spTaxRateByState";
            var databaseHandler = new DatabaseConnection();
            string returnValue = databaseHandler.ExecuteStoredProcedure(spName);
            var deserializedJSON = JsonSerializer.Deserialize<CompleteSalesTaxList>(returnValue);
            var totalStates = deserializedJSON?.stateSalesTaxList?.Count;
            Assert.AreNotEqual(0, totalStates);
        }

        [TestMethod]
        public void TextEmployeeQuery()
        {
            var databaseHandler = new DatabaseConnection();
            string returnValue = databaseHandler.GetEmployeeRecords();
            var deserializedJSON = JsonSerializer.Deserialize<EmployeeNameList>(returnValue);
            var totalEmployees = deserializedJSON?.employeeNameRecords?.Count;
            Assert.AreNotEqual(0, totalEmployees);
        }

        [TestMethod]
        public void TestEmployeeDetail()
        {
            var databaseHandler = new DatabaseConnection();
            string returnValue = databaseHandler.GetEmployeeDetail(10);
            var deserializedJSON = JsonSerializer.Deserialize<EmployeeDetail>(returnValue);
            Assert.AreEqual("Raheem", deserializedJSON?.LastName);
        }
    }
}