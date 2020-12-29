using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
namespace ShowIssueTracker.Models
{
    public class TableManager
    {
        // private property  
        private CloudTable table;

        // Constructor   
        public TableManager(string _CloudTableName)
        {
            if (string.IsNullOrEmpty(_CloudTableName))
            {
                throw new ArgumentNullException("Table", "Table Name can't be empty");
            }
            try
            {
                string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=womenworkforcedb;AccountKey=zOT6k0n4m0fSYIgKkNySlKpWzvrTarDfolBttUZwYffLGv3sHyRmwbg6tYBf1UuWGlKK9j0nHO05qZSdMVkZxQ==;TableEndpoint=https://womenworkforcedb.table.cosmos.azure.com:443/;";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                table = tableClient.GetTableReference(_CloudTableName);
                //table.CreateIfNotExists();
            }
            catch (StorageException StorageExceptionObj)
            {
                throw StorageExceptionObj;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //public static async Task<CloudTable> CreateTableAsync(string tableName)
        //{
        //    string storageConnectionString = AppSettings.LoadAppSettings().StorageConnectionString;

        //    // Retrieve storage account information from connection string.
        //    CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

        //    // Create a table client for interacting with the table service
        //    CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

        //    Console.WriteLine("Create a Table for the demo");

        //    // Create a table client for interacting with the table service 
        //    CloudTable table = tableClient.GetTableReference(tableName);
        //    if (await table.CreateIfNotExistsAsync())
        //    {
        //        Console.WriteLine("Created Table named: {0}", tableName);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Table {0} already exists", tableName);
        //    }

        //    Console.WriteLine();
        //    return table;
        //}
    }
}