using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDB
{
    public static class Create
    {
        private static DocumentClient client;
        static async Task<DocumentCollection> CreatePartitionedCollection()
        {
            DocumentCollection collection = new DocumentCollection();
            collection.Id = "Dexter";
            collection.PartitionKey.Paths.Add(@"/partner_id");

            DocumentCollection coll = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri("Elso"), collection,
                new RequestOptions { OfferThroughput = 10000 });

            return coll;

        }

        ///originals
        static async Task<DocumentCollection> CreatePartitionedCollection(
            string databaseId, string collectionId, string partitionKey)
        {
            DocumentCollection collection = new DocumentCollection();
            collection.Id = collectionId;
            collection.PartitionKey.Paths.Add(partitionKey);

            DocumentCollection companyCollection = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                collection,
                new RequestOptions { OfferThroughput = 10000 });

            return companyCollection;
        }

        static async Task<DocumentCollection> CreatePartitionedCollectionWithCustomIndexing(
            string databaseId, string collectionId, string partitionKey)
        {
            DocumentCollection collection = new DocumentCollection();
            collection.Id = collectionId;
            collection.PartitionKey.Paths.Add(partitionKey);
            collection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
            DocumentCollection companyCollection = await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                collection,
                new RequestOptions
                {
                    OfferThroughput = 10000,
                    ConsistencyLevel = ConsistencyLevel.Session
                });

            return companyCollection;
        }

        static async Task ChangeCollectionPerformance(string databaseId, string collectionId)
        {
            DocumentCollection collection = await client.ReadDocumentCollectionAsync(
                UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            Offer offer = client.CreateOfferQuery().Where(o => o.ResourceLink == collection.SelfLink)
                .AsEnumerable().Single();

            Console.WriteLine("Current offer is {0} and Collection Name is {1}", offer, collection.Id);

            Offer replaced = await client.ReplaceOfferAsync(new OfferV2(offer, 12000));

            offer = client.CreateOfferQuery().Where(o => o.ResourceLink == collection.SelfLink).AsEnumerable().Single();
            OfferV2 offerV2 = (OfferV2)offer;
            Console.WriteLine(offerV2.Content.OfferThroughput);
        }

        static void AddTypedDocumentsFromFile(string databaseId, string collectionId)
        {
            using (StreamReader file = new StreamReader("companies.json"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    JObject json = JObject.Parse(line);
                    Company company = json.ToObject<Company>();
                    company.Id = (string)json.SelectToken("_id.$oid");
                    CreateTypedDocument(databaseId, collectionId, company).Wait();
                }
            }
        }

        private static async Task<Document>
            CreateTypedDocument(string databaseId, string collectionId,
            Company company)
        {
            var collection = await client.ReadDocumentCollectionAsync(
                UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));

            Document document = await client.CreateDocumentAsync(
                collection.Resource.SelfLink, company);
            return document;
        }

        static async void AddJsonFromFile(string databaseId, string collectionId,
            string filePath)
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(line);
                    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        CreateCollectionFromJson(databaseId, collectionId, stream).Wait();
                    }
                }
            }
        }

        static async Task<Document> CreateCollectionFromJson(string databaseId,
            string collectionId, Stream stream)
        {
            var collection =
                await client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));

            Document document = await client.CreateDocumentAsync(
                collection.Resource.SelfLink, Resource.LoadFrom<Document>(stream));
            return document;
        }

    }
}
