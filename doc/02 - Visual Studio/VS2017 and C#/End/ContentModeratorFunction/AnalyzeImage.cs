using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentModeratorFunction
{
    public class AnalyzeImage
    {
        private readonly string ContentModeratorApiUri = $"https://{Environment.GetEnvironmentVariable("AssetsLocation")}.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0/ProcessText/Screen?language=eng";
        private readonly string ComputerVisionApiRoot = $"https://{Environment.GetEnvironmentVariable("AssetsLocation")}.api.cognitive.microsoft.com/vision/v1.0";
        private readonly string SearchTag = "cat";

        private readonly HttpClient httpClient;

        public AnalyzeImage(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("ReviewImageAndText")]
        public async Task Run(
            [QueueTrigger("review-queue")] ReviewRequestItem queueInput,
            [Blob("input-images/{BlobName}", FileAccess.Read)] Stream image,
            [CosmosDB("customerReviewData", "reviews", Id = "{DocumentId}", PartitionKey = "Reviews", ConnectionStringSetting = "customerReviewDataDocDB")] dynamic inputDocument)
        {
            bool passesText = await PassesTextModeratorAsync(inputDocument);

            (bool containsCat, string caption) = await PassesImageModerationAsync(image);

            inputDocument.IsApproved = containsCat && passesText;
            inputDocument.Caption = caption;
        }

        public class ReviewRequestItem
        {
            public string DocumentId { get; set; }
            public string BlobName { get; set; }
        }

        public async Task<bool> PassesTextModeratorAsync(dynamic document)
        {
            if (document.ReviewText == null)
            {
                return true;
            }

            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("ContentModerationApiKey"));

            var stringContent = new StringContent(document.ReviewText);
            var response = await httpClient.PostAsync(ContentModeratorApiUri, stringContent);

            response.EnsureSuccessStatusCode();

            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync());
            JToken token = data["Terms"];

            //If we have Terms in result it failed the moderation (Terms will have the bad terms)
            return !token.HasValues;
        }

        public async Task<(bool, string)> PassesImageModerationAsync(Stream image)
        {
            var client = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("MicrosoftVisionApiKey")),
                new DelegatingHandler[] { });

            var result = await client.AnalyzeImageInStreamAsync(image, new[] { VisualFeatureTypes.Description });

            bool containsCat = result.Description.Tags.Take(5).Contains(SearchTag);
            string message = result.Description?.Captions.FirstOrDefault()?.Text;
            return (containsCat, message);
        }
    }
}
