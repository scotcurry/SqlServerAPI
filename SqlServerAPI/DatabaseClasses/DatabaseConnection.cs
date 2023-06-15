using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using SqlServerAPI.Classes;

using Serilog;
using Serilog.Formatting.Json;

using Datadog.Trace;
using Datadog.Trace.Annotations;

namespace SqlServerAPI.DatabaseClasses
{
    public class DatabaseConnection
    {
        public DatabaseConnection()
        { }

        public static string ConnectionString
        {
            get { return "Data Source=localhost;Initial Catalog=AdventureWorks2019;User ID=scot;Password=AirWatch1;"; }
        }

        public static string OtisConnectionString
        {
            get { return "Data Source=localhost;Initial Catalog=AdventureWorks2019;User ID=Otis;Password=AirWatch1;"; }
        }

        public SqlConnection MakeConnection(string connectionString)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(renderMessage: true), "log.json")
                .CreateLogger();

            Log.Information("Attempting Database Connection");
            Log.Information("Connection String: " + connectionString);
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Log.Information("Success Opening Connection");
                }
                catch (SqlException exeception)
                {
                    Console.WriteLine(exeception.Message);
                    Log.Error(exeception.Message, exeception.StackTrace);
                }
                finally
                {
                    Log.CloseAndFlush();
                }

                return connection;
            }
        }

        [Trace(OperationName = "execute-product-query", ResourceName = "SqlServerAPI.Database.Classes.DatabaseConnection.ExecuteQuery")]
        public string ExecuteQuery(string query)
        {
            using (var scope = Tracer.Instance.StartActive("execute-product-query"))
            {
                var connectionString = ConnectionString;
                var productList = new List<ProductOrders>();
                var jsonString = string.Empty;
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            using SqlCommand cmd = new SqlCommand(query, connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                var productOrder = new ProductOrders
                                {
                                    orderQuantity = reader.GetInt16(0),
                                    orderName = reader.GetString(1),
                                    listPrice = reader.GetDecimal(2)
                                };
                                productList.Add(productOrder);
                            }
                            var productOrderList = new ProductOrderList();
                            productOrderList.productOrders = productList;
                            jsonString = JsonSerializer.Serialize(productOrderList);
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine(ex.Message);
                            jsonString = ex.Message;
                        }
                        connection.Close();
                        return jsonString;
                    }
                }
                catch (SqlException ex)
                {
                    Console.Write(ex.Message);
                    return "Scot";
                }
            }
        }

        [Trace(OperationName = "get-employee-records", ResourceName = "SqlServerAPI.Database.Classes.DatabaseConnection.GetEmployeeRecords")]
        public string GetEmployeeRecords()
        {
            var allEmployeeNames = new List<EmployeeNameRecord>();
            var connectionString = ConnectionString;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var sqlCommand = "SELECT TOP(40) Person.BusinessEntityID, Person.Title, Person.FirstName, Person.LastName FROM Person.Person WHERE (ABS(CAST((BINARY_CHECKSUM(*) * RAND()) as int)) % 100) < 1";
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var employeeRecord = new EmployeeNameRecord
                            {
                                BusinessEntityID = reader.GetInt32(0),
                                FirstName = reader.GetString(2),
                                LastName = reader.GetString(3),
                            };
                            allEmployeeNames.Add(employeeRecord);
                        }
                    }
                }
                var employeeNameList = new EmployeeNameList
                {
                    employeeNameRecords = allEmployeeNames
                };
                try
                {
                    var jsonString = JsonSerializer.Serialize(employeeNameList);
                    return jsonString;
                }
                catch (JsonException ex)
                {
                    Console.Write(ex.Message + "\n" + ex.StackTrace);
                    return "Error";
                }

                

            } catch (SqlException ex)
            {
                Console.Write(ex.Message);
                return "Error";
            }
        }

        [Trace(OperationName = "get-employee-detail", ResourceName = "SqlServerAPI.Database.Classes.DatabaseConnection.GetEmployeeDetail")]
        public string GetEmployeeDetail(int employeeID)
        {
            var sqlQuery = "SELECT [Person].[BusinessEntityID], [Person].[FirstName], [Person].[LastName], [Person].[EmailAddress].[EmailAddress], [Person].[PersonPhone].[PhoneNumber] ";
            sqlQuery += "FROM [AdventureWorks2019].[Person].[Person] ";
            sqlQuery += "INNER JOIN [Person].[EmailAddress] ON [Person].[Person].BusinessEntityID = [Person].[EmailAddress].[BusinessEntityID] ";
            sqlQuery += "INNER JOIN [Person].[PersonPhone] ON [Person].[Person].[BusinessEntityID] = [Person].[PersonPhone].[BusinessEntityID] ";
            sqlQuery += "WHERE [Person].[Person].[BusinessEntityID] =";

            sqlQuery = sqlQuery + employeeID.ToString();

            var connectionString = ConnectionString;
            var employeeDetails = new EmployeeDetail();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            employeeDetails.BusinessEntityID = reader.GetInt32(0);
                            employeeDetails.FirstName = reader.GetString(1);
                            employeeDetails.LastName = reader.GetString(2);
                            employeeDetails.EmailAddress = reader.GetString(3);
                            employeeDetails.PhoneNumber = reader.GetString(4);
                        }
                    }
                }
                var jsonString = JsonSerializer.Serialize(employeeDetails);
                return jsonString;
            } 
            catch (SqlException ex) 
            {
                Console.WriteLine(ex.Message + " " +ex.StackTrace);
                return "Error";
            }
        }

        [Trace(OperationName = "execute-stored_procedure", ResourceName = "SqlServerAPI.Database.Classes.DatabaseConnection.ExecuteStoredProcedure")]
        public string ExecuteStoredProcedure(string spName)
        {

            var allStateRates = new List<StateSalesTax>();
            var connectionString = ConnectionString;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(".\\Logs\\SQLConnection.Log")
                .CreateLogger();

            Log.Information("Attempting Execute Stored Procedure");
            Log.Information("Connection String: " + connectionString);
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(spName, connection))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@CountryRegionCode", "US");
                            SqlDataAdapter dataAdapter = new SqlDataAdapter()
                            {
                                SelectCommand = cmd,
                            };
                            DataSet dataSet = new DataSet();
                            dataAdapter.Fill(dataSet);

                            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                            {
                                var stateInfo = new StateSalesTax
                                {
                                    Name = (string)dataRow["Name"],
                                    TaxRate = (decimal)dataRow["TaxRate"],
                                    StateName = (string)dataRow["StateName"]
                                };
                                allStateRates.Add(stateInfo);
                            }
                        }
                        var completeSalesTaxList = new CompleteSalesTaxList();
                        completeSalesTaxList.stateSalesTaxList = allStateRates;
                        var jsonString = JsonSerializer.Serialize(completeSalesTaxList);
                        return jsonString;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Log.Error(ex.Message);
                        var stateInfo = new StateSalesTax
                        {
                            Name = "Error",
                            TaxRate = Convert.ToDecimal("-1.0"),
                            StateName = (string)ex.Message
                        };
                        allStateRates.Add(stateInfo);
                        return JsonSerializer.Serialize(allStateRates);
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
                var stateInfo = new StateSalesTax
                {
                    Name = "Error",
                    TaxRate = Convert.ToDecimal("-1.0"),
                    StateName = (string)ex.Message
                };
                allStateRates.Add(stateInfo);
                return JsonSerializer.Serialize(allStateRates);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
