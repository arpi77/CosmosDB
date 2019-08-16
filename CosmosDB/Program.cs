using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;

namespace CosmosDB
{
    class Program
    {
        private static string endpointUrl = "";

        private static string authorizationKey =
            "";

        private static DocumentClient client;

        static void Main(string[] args)
        {
            using(client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {
                client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Elso" }).Wait();
                CreatePartitionedCollection().Wait();
            }

            Console.ReadLine();
        }
}
}
