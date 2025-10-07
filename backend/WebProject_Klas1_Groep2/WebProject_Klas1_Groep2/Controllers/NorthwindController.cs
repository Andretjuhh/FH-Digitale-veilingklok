using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // Required for SQL connection
using System.Collections.Generic;
using System.Data; // Required for CommandType and SqlDbType (best practice)

namespace WebProject_Klas1_Groep2.Controllers
{
    // Sets the URL route for this controller (e.g., /northwind)
    [ApiController]
    [Route("[controller]")]
    public class NorthwindController : ControllerBase
    {
        // Define the connection string. Using 'master' because your tables landed there.
        const string connectionString =
            "Data Source=(localdb)\\MSSQLLocalDB;" +
            "Initial Catalog=master;";

        // --------------------------------------------------------------------------------
        // 1. Connection Test (You already created this)
        // --------------------------------------------------------------------------------

        [HttpGet]
        [Route("test")]
        public string TestConnection()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return "SUCCESS: Connected to Northwind database via master catalog!";
                }
                catch (Exception ex)
                {
                    return $"FAILURE: Connection failed. Error: {ex.Message}";
                }
            }
        }

        // --------------------------------------------------------------------------------
        // 2. SQL SELECT Command Execution (The new step)
        // --------------------------------------------------------------------------------

        [HttpGet]
        [Route("products/{minPrice}")]
        public List<string> GetProductsByPrice(decimal minPrice)
        {
            // 1. Define Query String and Data Structure
            const string queryString =
                "SELECT ProductID, UnitPrice, ProductName " +
                "FROM dbo.Products " +
                "WHERE UnitPrice > @pricePoint " +
                "ORDER BY UnitPrice DESC;";

            List<string> productList = new List<string>();

            // 1. Make a connection (using the connectionString)
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // 2. Create an SQL command object
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    // 3. Fill in the parameter
                    command.Parameters.AddWithValue("@pricePoint", minPrice);

                    try
                    {
                        // 4. Open the connection
                        connection.Open();

                        // 5. Execute the command and get the SqlDataReader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Loop through the result table
                            while (reader.Read())
                            {
                                string productName = reader.GetString(2);
                                decimal unitPrice = reader.GetDecimal(1);
                                productList.Add($"Product: {productName}, Price: {unitPrice:C}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        productList.Add($"ERROR: Failed to retrieve data. {ex.Message}");
                    }
                }
            }
            return productList;
        }

        [HttpPut]
        [Route("products/update-price/{productID}/{newPrice}")]
        public string UpdateProductPrice(int productID, decimal newPrice)
        {
            // The query will update the UnitPrice for a specific ProductID
            const string updateQuery =
                "UPDATE dbo.Products " +
                "SET UnitPrice = @price " +
                "WHERE ProductID = @id;";

            // We still use the same connection string (assuming tables are in 'master')
            const string connectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;" +
                "Initial Catalog=master;";

            Int32 rows = 0; // The variable to store the number of changed rows

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    // Set parameters (crucial for safety, just like with SELECT)
                    command.Parameters.AddWithValue("@price", newPrice);
                    command.Parameters.AddWithValue("@id", productID);

                    try
                    {
                        connection.Open();

                        // ExecuteNonQuery() is used because UPDATE returns no result table
                        rows = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return $"FAILURE: Update failed. Error: {ex.Message}";
                    }
                }
            }
            // Return the number of changed rows
            return $"SUCCESS: Updated {rows} row(s) for ProductID {productID}.";
        }

        [HttpPut]
        [Route("transaction-test")]
        public string RunTransactionTest()
        {
            // Use the same connection string (assuming tables are in 'master')
            const string connectionString =
                "Data Source=(localdb)\\MSSQLLocalDB;" +
                "Initial Catalog=master;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Open the connection first

                // 1. Begin the transaction
                SqlTransaction sqlTran = connection.BeginTransaction();

                // 2. Create the command and link it to the transaction
                // This is necessary for all commands that are part of the transaction
                SqlCommand command = connection.CreateCommand();
                command.Transaction = sqlTran;

                try
                {
                    // Set the first command (e.g., reduce stock for ProductID 1)
                    command.CommandText =
                        "UPDATE dbo.Products SET UnitsInStock = UnitsInStock - 1 WHERE ProductID = 1;";
                    command.ExecuteNonQuery(); // Execute the first query

                    // Simulate a second, critical action (e.g., logging or another update)
                    command.CommandText =
                        "UPDATE dbo.Products SET UnitsOnOrder = UnitsOnOrder + 1 WHERE ProductID = 1;";
                    command.ExecuteNonQuery(); // Execute the second query

                    // 3. If all SQL queries succeed: COMMIT all changes
                    sqlTran.Commit();
                    return "SUCCESS: Transaction committed. All changes saved.";
                }
                catch (Exception ex)
                {
                    // 4. If an error occurs: ROLLBACK all changes
                    // Even the queries that succeeded (e.g., the first UPDATE) are rolled back.
                    sqlTran.Rollback();
                    return $"FAILURE: Transaction rolled back. Error: {ex.Message}";
                }
            }
        }
    }
}
