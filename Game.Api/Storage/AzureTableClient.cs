using System;
using Azure.Data.Tables;

namespace Game.Api.Storage
{
    /// <summary>
    /// Lightweight wrapper for Azure TableServiceClient usage.
    /// Expects connection string in AZURE_TABLES_CONNECTION_STRING or provided explicitly.
    /// </summary>
    public class AzureTableClient
    {
        private readonly TableServiceClient _serviceClient;

        public AzureTableClient(string connectionString = null)
        {
            connectionString ??= Environment.GetEnvironmentVariable("AZURE_TABLES_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Azure Table Storage connection string not provided. Set AZURE_TABLES_CONNECTION_STRING.");

            _serviceClient = new TableServiceClient(connectionString);
        }

        public TableClient GetTableClient(string tableName)
        {
            var client = _serviceClient.GetTableClient(tableName);
            client.CreateIfNotExists();
            return client;
        }
    }
}
