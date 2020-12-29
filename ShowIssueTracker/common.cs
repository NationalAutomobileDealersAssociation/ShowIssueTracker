using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace ShowIssueTracker
{
    public class common
    {
        private IConfiguration _configuration;
        public common(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string databaseconnectionstring)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(databaseconnectionstring);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
        public static async Task<CloudTable> CreateTableAsync(string tableName, string storageConnectionString)
        {
             //string storageConnectionString = _configuration["databaseconnectionstring"];
           // string storageConnectionString = AppSettings.LoadAppSettings().databaseconnectionstring;

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount =   CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient =  storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            //if (await table.CreateIfNotExistsAsync())
            //{
            //    Console.WriteLine("Created Table named: {0}", tableName);
            //}
            //else
            //{
            //    Console.WriteLine("Table {0} already exists", tableName);
            //}

            Console.WriteLine();
            return table;
        }
    }
}
