using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using CatsReviewApp.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CatsReviewApp.Services
{
    public class ReviewProvider
    {
        private readonly string storageAccountConnectionString;

        private readonly DocumentClient client;

        private readonly string documentDbName;
        private readonly string documentDbColl;
        private readonly string containerName;
        private readonly string queueName;

        public ReviewProvider(IConfiguration configuration)
        {
            storageAccountConnectionString = configuration.GetValue<string>("storageAccountConnectionString");

            documentDbName = configuration.GetValue<string>("documentDbName");
            documentDbColl = configuration.GetValue<string>("documentDbColl");

            containerName = configuration.GetValue<string>("containerName");
            queueName = configuration.GetValue<string>("queueName");

            this.client = new DocumentClient(new Uri(configuration.GetValue<string>("documentDbEndpoint")), configuration.GetValue<string>("documentDbKey"));
        }

        public async Task<IEnumerable<CatReview>> GetReviewsAsync()
        {
            IQueryable<CatReview> catReviewsQuery = this.client
                .CreateDocumentQuery<CatReview>(UriFactory.CreateDocumentCollectionUri(this.documentDbName, this.documentDbColl));

            return await this.QueryAsync(catReviewsQuery);
        }

        public async Task<CatReview> GetReviewAsync(Guid id)
        {
            return (await this.client.ReadDocumentAsync<CatReview>(
                UriFactory.CreateDocumentUri(this.documentDbName, this.documentDbColl, id.ToString()),
                new RequestOptions { PartitionKey = new PartitionKey("Reviews") })).Document;
        }

        public async Task<Guid> CreateReviewAsync(Stream image, string reviewText)
        {
            var recordId = Guid.NewGuid();

            // save image
            var blobContainerClient = new BlobContainerClient(storageAccountConnectionString, containerName);
            await blobContainerClient.CreateIfNotExistsAsync(publicAccessType: Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var blockBlob = blobContainerClient.GetBlobClient(recordId.ToString());
            await blockBlob.UploadAsync(image);

            // save review
            await this.client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(this.documentDbName, this.documentDbColl),
                new CatReview
                {
                    Id = recordId,
                    MediaUrl = blockBlob.Uri.ToString(),
                    ReviewText = reviewText,
                    IsApproved = null,
                    Created = DateTime.UtcNow
                });

            // notify through queue
            var queueClient = new QueueClient(storageAccountConnectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();

            // warning: there is an issue with the Azure.Storage.Queues v12 NuGet package
            // A JSON payload has to be encoded in base64
            // https://github.com/Azure/azure-sdk-for-net/issues/10242
            var payload = new { BlobName = recordId.ToString(), DocumentId = recordId.ToString() };
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            await queueClient.SendMessageAsync(Convert.ToBase64String(plainTextBytes));

            return recordId;
        }

        private async Task<IEnumerable<T>> QueryAsync<T>(IQueryable<T> query)
        {
            var docQuery = query.AsDocumentQuery();
            var batches = new List<IEnumerable<T>>();

            do
            {
                var batch = await docQuery.ExecuteNextAsync<T>();

                batches.Add(batch);
            }
            while (docQuery.HasMoreResults);

            return batches.SelectMany(b => b);
        }
    }
}