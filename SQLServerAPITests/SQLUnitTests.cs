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

            Assert.AreSame(connectionString, "Provider=MSOLEDBSQL19;Server=(local);Database=AdventureWorks;Use Encryption for Data=Optional;");
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
            var databaseHandler = new DatabaseConnection();
            string scot = databaseHandler.ExecuteQuery(queryString);
            Assert.AreEqual(queryString, scot); 
        }
    }
}