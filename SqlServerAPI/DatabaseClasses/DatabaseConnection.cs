using System.Drawing;
using Microsoft.Data.SqlClient;

namespace SqlServerAPI.DatabaseClasses
{
    public class DatabaseConnection
    {
        public DatabaseConnection()
        { }

        public static string ConnectionString
        {
            get { return "Data Source=localhost;Initial Catalog=AdventureWorks2019;User ID=scot;Password=AirWatch1"; }
        }

        public SqlConnection MakeConnection(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (SqlException exeception)
                {
                    Console.WriteLine(exeception.Message);
                }

                return connection;
            }
        }

        public string ExecuteQuery(string query)
        {
            var scot = String.Empty;
            var connectionString = DatabaseConnection.ConnectionString;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            SqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                reader.GetString(2);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    return scot;
                }
            } 
            catch (SqlException ex)
            {
                Console.Write(ex.Message);
                return scot;
            }
        }
    }
}
