using Microsoft.Data.SqlClient;
using SqlServerAPI.DatabaseClasses;

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
        {
            var connectionString = DatabaseConnection.ConnectionString;
            var connection = new DatabaseConnection();
            var connectionType = typeof(DatabaseConnection);
            Assert.AreSame(connection.GetType(), connectionType);
        }

        [TestMethod]
        public void TestSimpleQuery() 
        {
           
            var queryString = "SELECT TOP (1000) [ContactTypeID], [Name], [ModifiedDate] FROM [AdventureWorks2019].[Person].[ContactType]";
            queryString += "ORDER BY [AdventureWorks2019].[Person].[ContactType].Name";
            var databaseHandler = new DatabaseConnection();
            string dbValue = databaseHandler.ExecuteQuery(queryString);
            Assert.AreEqual("Sales Representative", dbValue); 
        }
    }
}