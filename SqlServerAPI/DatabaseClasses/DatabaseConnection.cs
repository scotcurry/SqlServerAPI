using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using SqlServerAPI.Classes;

using Serilog;
using Serilog.Formatting.Json;

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

        [Trace(OperationName = "execute.query", ResourceName = "SqlServerAPI.Database.Classes.DatabaseConnection.ExecuteQuery")]
        public string ExecuteQuery(string query)
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

        [Trace(OperationName = "execute.stored_procedure", ResourceName = "SqlServerAPI.Database.Classes.DatabaseConnection.ExecuteStoredProcedure")]
        public string ExecuteStoredProcedure(string spName)
        {

            var allStateRates = new List<StateSalesTaxList>();
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
                                var stateInfo = new StateSalesTaxList
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
                        var stateInfo = new StateSalesTaxList
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
                var stateInfo = new StateSalesTaxList
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
