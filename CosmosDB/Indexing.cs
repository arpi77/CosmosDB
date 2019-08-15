using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosDB
{
    public static class Indexing
    {
        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });
            var collection = await client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = collectionId });

            await NoIndexAtDocumentLevel(database.Resource, collection.Resource);

        }

        private static async Task NoIndexAtDocumentLevel(
            Database database, DocumentCollection collection)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);

            var created = await client.CreateDocumentAsync(
                collectionUri,
                new { id = "123", name = "Chander Dhall", company = "Cazton" },
                new RequestOptions
                {
                    IndexingDirective = IndexingDirective.Exclude
                });

            var docExists = client.CreateDocumentQuery(
                collectionUri,
                "Select * from root r where r.company='Cazton'")
                .AsEnumerable().Any();

            Document document = await client.ReadDocumentAsync(
                created.Resource.SelfLink);

            await client.DeleteDocumentCollectionAsync(collectionUri);
        }

        private static async Task RunDemo()
        {
            //var database = await client.CreateDatabaseIfNotExistsAsync(
            //    new Database { Id = databaseId });
            //var collection = await client.CreateDocumentCollectionAsync(
            //    UriFactory.CreateDatabaseUri(databaseId),
            //    new DocumentCollection { Id = collectionId });

            await ManualIndexingAtCollectionLevel();

        }

        private static async Task ManualIndexingAtCollectionLevel()
        {
            var collectionUrl = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);

            var collectionSpecifications = new DocumentCollection
            {
                Id = collectionId,
            };
            collectionSpecifications.IndexingPolicy.Automatic = false;

            var collection = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                collectionSpecifications);

            Document created = await client.CreateDocumentAsync(
                collection.Resource.SelfLink,
                new { id = "200", name = "Chander Dhall", company = "Apple" });

            bool docExists = client.CreateDocumentQuery(
                collection.Resource.SelfLink,
                "Select * from root r where r.company='Apple'").AsEnumerable().Any();

            Document document = await client.ReadDocumentAsync(created.SelfLink);

            Document manuallyIndexedDocument = await client.CreateDocumentAsync(
                collection.Resource.SelfLink,
                new { id = "100", name = "Chander Dhall", company = "Microsoft" },
                new RequestOptions
                {
                    IndexingDirective = IndexingDirective.Include
                });

            docExists = client.CreateDocumentQuery(
                collection.Resource.SelfLink,
                "Select * from root r where r.company='Microsoft'").AsEnumerable().Any();

            await client.DeleteDocumentCollectionAsync(collectionUrl);
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);
            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            dynamic document = new
            {
                id = "900",
                name = "Chander",
                company = new { name = "Cazton", address = new { city = "Austin", state = "Texas" } },
                address = new
                {
                    mailingAddress = new { city = "Seattle", state = "Washington" },
                    shippingAddress = new { city = "Los Angeles", state = "California" }
                },
                notes = new { awards = "Microsoft MVP", title = "CEO" }
            };

            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            collectionDefinition.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath { Path = "/*" });
            collectionDefinition.IndexingPolicy.ExcludedPaths.Add(
             new ExcludedPath { Path = "/company/*" });
            //collectionDefinition.IndexingPolicy.ExcludedPaths.Add(
            // new ExcludedPath { Path = "/\"company\"/*" });
            collectionDefinition.IndexingPolicy.ExcludedPaths.Add(
             new ExcludedPath { Path = "/address/mailingAddress/*" });

            var collection = await client.CreateDocumentCollectionIfNotExistsAsync(
             UriFactory.CreateDatabaseUri(databaseId),
             collectionDefinition);

            Document created = await client.CreateDocumentAsync(
                collection.Resource.SelfLink, document);
            var result = client.CreateDocumentQuery(
                collection.Resource.SelfLink,
                "Select * from root r where r.name='Chander'").
                AsEnumerable().Any();

            //try
            //{
            //    result = client.CreateDocumentQuery(
            //    collection.Resource.SelfLink,
            //    "Select * from root r where r.company.name='Cazton'").
            //    AsEnumerable().Any();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            //try
            //{
            //    result = client.CreateDocumentQuery(
            //    collection.Resource.SelfLink,
            //    "Select * from root r where r.address.mailingAddress.city='Austin'").
            //    AsEnumerable().Any();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            try
            {
                result = client.CreateDocumentQuery(
                collection.Resource.SelfLink,
                "Select * from root r where r.notes.title='CEO'").
                AsEnumerable().Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);
            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            collectionDefinition.IndexingPolicy.IncludedPaths.Add(
                new IncludedPath { Path = "/*" });
            collectionDefinition.IndexingPolicy.ExcludedPaths.Add(
             new ExcludedPath { Path = "/age/*" });
            collectionDefinition.IndexingPolicy.ExcludedPaths.Add(
             new ExcludedPath { Path = "/salary/*" });

            var collection = await client.CreateDocumentCollectionIfNotExistsAsync(
             UriFactory.CreateDatabaseUri(databaseId),
             collectionDefinition);

            var alison = await client.CreateDocumentAsync(
                collection.Resource.SelfLink,
                new { id = "1", employee_name = "Alison", age = 25, salary = 200000 });

            var ben = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { id = "2", employee_name = "Ben", age = 30, salary = 100000 });

            var krishna = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { id = "3", employee_name = "Krishna", age = 35, salary = 150000 });

            var sana = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { id = "4", employee_name = "sana", age = 40, salary = 300000 });

            var kevin = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { id = "5", employee_name = "Kevin", age = 45, salary = 250000 });

            var jacob = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { id = "6", employee_name = "Jacob", age = 50, salary = 50000 });

            //try
            //{
            //    var result = client.CreateDocumentQuery(
            //        collection.Resource.SelfLink,
            //        "Select * from root r where r.age > 25 ").
            //        AsEnumerable().Any();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            try
            {
                var results = client.CreateDocumentQuery(
                    collection.Resource.SelfLink,
                    "Select * from root r where r.age > 25 ",
                    new FeedOptions { EnableScanInQuery = true }).
                    AsEnumerable().ToList();

                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);
            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            IndexingPolicy indexingPolicy = new IndexingPolicy();
            //new RangeIndex(DataType.String)
            //{
            //    Precision = -1
            //});

            indexingPolicy.IncludedPaths.Add(new IncludedPath
            {
                Path = "/*"
            });
            indexingPolicy.IncludedPaths.Add(new IncludedPath
            {
                Path = "/state/?",
                Indexes = new Collection<Index>()
                {
                    new RangeIndex(DataType.String)
                    {
                        Precision=-1
                    }
                }
            });

            collectionDefinition.IndexingPolicy = indexingPolicy;

            var collection = await client.CreateDocumentCollectionIfNotExistsAsync(
             UriFactory.CreateDatabaseUri(databaseId),
             collectionDefinition);

            var alison = await client.CreateDocumentAsync(
                collection.Resource.SelfLink,
                new
                {
                    id = "1",
                    employee_name = "Alison",
                    age = 25,
                    salary = 200000,
                    city = "Irvine",
                    state = "CA"
                });

            var ben = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "Dallas", state = "TX", id = "2", employee_name = "Ben", age = 30, salary = 100000 });

            var krishna = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "San Jose", state = "CA", id = "3", employee_name = "Krishna", age = 35, salary = 150000 });

            var sana = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "LA", state = "CA", id = "4", employee_name = "sana", age = 40, salary = 300000 });

            var kevin = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "Austin", state = "TX", id = "5", employee_name = "Kevin", age = 45, salary = 250000 });

            var jacob = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "LA", state = "CA", id = "6", employee_name = "Jacob", age = 50, salary = 50000 });

            try
            {
                var results = client.CreateDocumentQuery(
                     collection.Resource.SelfLink,
                     "Select * from root r where r.state >= 'D'");

                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task RunDemo()
        {
            var database = await client.CreateDatabaseIfNotExistsAsync(
                new Database { Id = databaseId });

            var collectionUri = UriFactory.CreateDocumentCollectionUri(
                databaseId, collectionId);
            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var collectionDefinition = new DocumentCollection
            {
                Id = collectionId
            };

            IndexingPolicy indexingPolicy = new IndexingPolicy();
            //new RangeIndex(DataType.String)
            //{
            //    Precision = -1
            //});

            indexingPolicy.IncludedPaths.Add(new IncludedPath
            {
                Path = "/*"
            });
            indexingPolicy.IncludedPaths.Add(new IncludedPath
            {
                Path = "/state/?",
                Indexes = new Collection<Index>()
                {
                    new RangeIndex(DataType.String)
                    {
                        Precision=-1
                    }
                }
            });

            collectionDefinition.IndexingPolicy = indexingPolicy;

            var collection = await client.CreateDocumentCollectionIfNotExistsAsync(
             UriFactory.CreateDatabaseUri(databaseId),
             collectionDefinition);

            collection.Resource.IndexingPolicy.IndexingMode = IndexingMode.Lazy;
            collection.Resource.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { })
            {
                Precision = -1
            )};


            var alison = await client.CreateDocumentAsync(
                collection.Resource.SelfLink,
                new
                {
                    id = "1",
                    employee_name = "Alison",
                    age = 25,
                    salary = 200000,
                    city = "Irvine",
                    state = "CA"
                });

            var ben = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "Dallas", state = "TX", id = "2", employee_name = "Ben", age = 30, salary = 100000 });

            var krishna = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "San Jose", state = "CA", id = "3", employee_name = "Krishna", age = 35, salary = 150000 });

            var sana = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "LA", state = "CA", id = "4", employee_name = "sana", age = 40, salary = 300000 });

            var kevin = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "Austin", state = "TX", id = "5", employee_name = "Kevin", age = 45, salary = 250000 });

            var jacob = await client.CreateDocumentAsync(
               collection.Resource.SelfLink,
               new { city = "LA", state = "CA", id = "6", employee_name = "Jacob", age = 50, salary = 50000 });

            try
            {
                var results = client.CreateDocumentQuery(
                     collection.Resource.SelfLink,
                     "Select * from root r where r.state >= 'D'");

                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                await client.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
