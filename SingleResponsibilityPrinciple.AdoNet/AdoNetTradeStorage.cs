using Microsoft.Data.SqlClient;
using SingleResponsibilityPrinciple.Contracts;

namespace SingleResponsibilityPrinciple.AdoNet
{
    public class AdoNetTradeStorage : ITradeStorage
    {
        private readonly ILogger logger;

        public AdoNetTradeStorage(ILogger logger)
        {
            this.logger = logger;
        }

        public void Persist(IEnumerable<TradeRecord> trades)
        {

            // Template for connection string from database connection file
            //    The @ sign allows for back slashes
            //    Watch for double quotes which must be escaped using "" 
            //    Watch for extra spaces after C: and avoid paths with - hyphens -
            string genericConnectString = @"Data Source=(local);Initial Catalog=TradeDatabase;Integrated Security=True;";
            // The datadirConnectString connection string uses |DataDirectory| 
            //    and assumes the tradedatabase.mdf file is stored in 
            //    SingleResponsibilityPrinciple\bin\Debug 
            string datadirConnectString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\tradedatabase.mdf;Integrated Security=True;Connect Timeout=30;";
            // This users the Azure connection string
            string azureConnectString = @"Server=tcp:cis3285-sql-server.database.windows.net,1433; Initial Catalog = Unit8_TradesDatabase; Persist Security Info=False; User ID=cis3285;Password=Saints4SQL; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 60;";
            string tomConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\tgibbons\Documents\tradedatabase.mdf;Integrated Security=True;Connect Timeout=30";
            // Change the connection string used to match the one you want
            logger.LogInfo("INFO: Connecting to AZURE cloud database... This may timeout the first time");
            using (var connection = new SqlConnection(azureConnectString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var trade in trades)
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "dbo.insert_trade";
                        command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
                        command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                        command.Parameters.AddWithValue("@lots", trade.Lots);
                        command.Parameters.AddWithValue("@price", trade.Price);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                connection.Close();
            }

            logger.LogInfo("{0} trades processed", trades.Count());
        }

        public async Task PersistAsync(IEnumerable<TradeRecord> trades)
        {
            // Connection string (same as before)
            string azureConnectString = @"Server=tcp:cis3285-sql-server.database.windows.net,1433; Initial Catalog = Unit8_TradesDatabase; Persist Security Info=False; User ID=cis3285;Password=Saints4SQL; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 60;";
            logger.LogInfo("INFO: Connecting to AZURE cloud database... This may timeout the first time");

            using (var connection = new SqlConnection(azureConnectString))
            {
                await connection.OpenAsync();  // Open connection asynchronously

                // Explicit cast from DbTransaction to SqlTransaction
                using (var transaction = (SqlTransaction)await connection.BeginTransactionAsync())  // Begin transaction asynchronously
                {
                    foreach (var trade in trades)
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = transaction;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "dbo.insert_trade";
                        command.Parameters.AddWithValue("@sourceCurrency", trade.SourceCurrency);
                        command.Parameters.AddWithValue("@destinationCurrency", trade.DestinationCurrency);
                        command.Parameters.AddWithValue("@lots", trade.Lots);
                        command.Parameters.AddWithValue("@price", trade.Price);

                        await command.ExecuteNonQueryAsync();  // Execute the command asynchronously
                    }

                    await transaction.CommitAsync();  // Commit transaction asynchronously
                }
                await connection.CloseAsync();  // Close connection asynchronously
            }

            logger.LogInfo("{0} trades processed", trades.Count());
        }
    }
}


