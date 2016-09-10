using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WebRole1
{
    public static class TableStorage
    {
        private const string ListsTable = "Lists";
        private const string TasksTable = "Tasks";

        // Public Table Constants
        public const string PartitionKey = "PartitionKey";
        public const string RowKey = "RowKey";

        public static CloudTable Lists
        {
            get { return GetAzureTable(ListsTable); }
        }

        public static CloudTable Tasks
        {
            get { return GetAzureTable(TasksTable); }
        }

        public static void InitializeAzureTables()
        {
            foreach (
                var table in
                    new[]
                    {
                        ListsTable, TasksTable
                    })
            {
                InitializeAzureTable(table);
            }
        }

        private static void InitializeAzureTable(string tableName)
        {
            var table = GetAzureTable(tableName);

            // Create table if it doesn't exists
            table.CreateIfNotExists();
        }

        private static CloudTable GetAzureTable(string tableName)
        {
            string connectionString;

            // TODO: Fix this, throws exception on some machines during unit tests.
            try
            {
                connectionString = /* RoleEnvironment.IsAvailable */ false
                    ? RoleEnvironment.GetConfigurationSettingValue("ConnectionString")
                    : "UseDevelopmentStorage=true";
            }
            catch (TypeInitializationException)
            {
                connectionString = "UseDevelopmentStorage=true";
            }

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(tableName);

            return table;
        }
    }

    public static class CloudTableExtensions
    {
        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable cloudTable, TableQuery<T> query)
            where T : ITableEntity, new()
        {
            TableQuerySegment<T> currentSegment = null;

            var entities = new List<T>();

            while (currentSegment == null || currentSegment.ContinuationToken != null)
            {
                currentSegment = await cloudTable.ExecuteQuerySegmentedAsync(
                    query,
                    currentSegment != null ? currentSegment.ContinuationToken : null);

                entities.AddRange(currentSegment.Results);
            }

            return entities;
        }
    }
}