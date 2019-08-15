using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDB
{
    public static class Querying
    {
        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);


            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            //var companies = from company in client.CreateDocumentQuery<Company>(
            //    collectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
            //    select company;

            //var companies = client.CreateDocumentQuery<Company>(collectionUri, new FeedOptions { EnableCrossPartitionQuery = true });

            var companies = client.CreateDocumentQuery<Company>(
                collectionUri, "Select * from root r", DefaultOptions);

            foreach (var company in companies.ToList())
            {
                Console.WriteLine(company.Name);
            }

            //try
            //{
            //    await client.DeleteDocumentCollectionAsync(collectionUri);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);


            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            var query =
                client.CreateDocumentQuery<Company>(
                    collectionUri, DefaultOptions).AsDocumentQuery();

            List<Company> parallelQuery = new List<Company>();
            var options = new FeedOptions
            {
                MaxDegreeOfParallelism = 10,
                MaxBufferedItemCount = 100,
                EnableCrossPartitionQuery = true
            };

            while (query.HasMoreResults)
            {
                foreach (Company company in await query.ExecuteNextAsync())
                {
                    parallelQuery.Add(company);
                }
            }

            foreach (var company in parallelQuery.ToList())
            {
                Console.WriteLine(company.Name);
            }

            Console.ReadLine();

            //try
            //{
            //    await client.DeleteDocumentCollectionAsync(collectionUri);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);


            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            //var query = client.CreateDocumentQuery<Company>(
            //    collectionUri, new SqlQuerySpec()
            //    {
            //        QueryText = "Select * from Companies c where (c.name = @name)",
            //        Parameters = new SqlParameterCollection()
            //        {
            //            new SqlParameter("@name", "Omnidrive")
            //        }
            //    }, DefaultOptions).ToList();

            var companies = client.CreateDocumentQuery<Company>(
                collectionUri, new SqlQuerySpec()
                {
                    QueryText = "Select * from Companies c where (c.name = @name) OR c.category_code= @category_code",
                    Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@name", "Omnidrive"),
                        new SqlParameter("@category_code", "web")
                    }
                }, DefaultOptions).ToList();
            Console.ReadLine();

            //try
            //{
            //    await client.DeleteDocumentCollectionAsync(collectionUri);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);


            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            //var query = client.CreateDocumentQuery<Company>(
            //    collectionUri, new SqlQuerySpec()
            //    {
            //        QueryText = "Select * from Companies c where (c.name = @name)",
            //        Parameters = new SqlParameterCollection()
            //        {
            //            new SqlParameter("@name", "Omnidrive")
            //        }
            //    }, DefaultOptions).ToList();

            var companies = (from c in client.CreateDocumentQuery<Company>(
                collectionUri, DefaultOptions)
                             where c.Name == "Flextor" || c.Category_Code == "web"
                             select new { Id = c.Id, Name = c.Name }).ToList();

            var results = client.CreateDocumentQuery<Company>(
                collectionUri, DefaultOptions).
                Where(c => c.Name == "Flextor" || c.Category_Code == "web")
                .Select(c => new { Name = c.Name, Category = c.Category_Code }).ToList();

            var query = client.CreateDocumentQuery(collectionUri,
                "Select c.name as Name," +
                "c.category_code as Category from companies c where c.name='Flextor' OR c.category_code='web'",
                DefaultOptions).ToList();
            Console.ReadLine();

            //try
            //{
            //    await client.DeleteDocumentCollectionAsync(collectionUri);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);


            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            //var query = client.CreateDocumentQuery<Company>(
            //    collectionUri, new SqlQuerySpec()
            //    {
            //        QueryText = "Select * from Companies c where (c.name = @name)",
            //        Parameters = new SqlParameterCollection()
            //        {
            //            new SqlParameter("@name", "Omnidrive")
            //        }
            //    }, DefaultOptions).ToList();

            var companies = (from c in client.CreateDocumentQuery<Company>(
                collectionUri, DefaultOptions)
                             where c.Name == "Omnidrive" && c.Category_Code != "web"
                             select new { Id = c.Id, Name = c.Name }).ToList();

            var results = client.CreateDocumentQuery<Company>(
                collectionUri, DefaultOptions).
                Where(c => c.Name == "Omnidrive" && c.Category_Code != "web")
                .Select(c => new { Name = c.Name, Category = c.Category_Code }).ToList();

            var query = client.CreateDocumentQuery(collectionUri,
                "Select c.name as Name," +
                "c.category_code as Category from companies c where c.name='Omnidrive' AND c.category_code <> 'web'",
                DefaultOptions).ToList();

            Console.ReadLine();

            //try
            //{
            //    await client.DeleteDocumentCollectionAsync(collectionUri);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);


            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            //var query = client.CreateDocumentQuery<Company>(
            //    collectionUri, new SqlQuerySpec()
            //    {
            //        QueryText = "Select * from Companies c where (c.name = @name)",
            //        Parameters = new SqlParameterCollection()
            //        {
            //            new SqlParameter("@name", "Omnidrive")
            //        }
            //    }, DefaultOptions).ToList();

            var companies = (from c in client.CreateDocumentQuery<Company>(
                collectionUri, DefaultOptions)
                             where c.Name.CompareTo("Digg") < 0
                             select new { Id = c.Id, Name = c.Name }).ToList();

            //var results = client.CreateDocumentQuery<Company>(
            //    collectionUri, DefaultOptions).
            //    Where(c => c.NumberOfEmployees < 100 && c.NumberOfEmployees > 10)
            //    .Select(c => new { Name = c.Name, Category = c.Category_Code }).ToList();

            var query = client.CreateDocumentQuery(collectionUri,
                "Select c.name as Name," +
                "c.category_code as Category from companies c where c.name > 'Digg'",
                DefaultOptions).ToList();

            Console.ReadLine();

            //try
            //{
            //    await client.DeleteDocumentCollectionAsync(collectionUri);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
        }
    }
}
