using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosDB
{
    class Program
    {
        private static string endpointUrl = "https://cosmos-test-dx.documents.azure.com:443/";

        private static string authorizationKey =
            "m1tZOlNMlXdlXqFuUkZPIJJXvsqrrgGJPnF1odhoQ9lT1GTttkjD70lqcvgUtBedZkzYRimriEh2A3MYDAivZQ==";

        private static DocumentClient client;

        static void Main(string[] args)
        {
            using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
            {
               // client.CreateDatabaseIfNotExistsAsync(new Database {Id = "Supi1"}).Wait();
                
            }
        }
    }
}
