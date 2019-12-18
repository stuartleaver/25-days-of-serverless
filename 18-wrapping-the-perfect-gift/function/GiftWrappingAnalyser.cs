using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Christmas.GiftWrappingAnalyser.Entities;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Christmas.GiftWrappingAnalyser
{
    public static class GiftWrappingAnalyser
    {
        [FunctionName("GiftWrappingAnalyser")]
        public static async Task Run(
            [BlobTrigger("gift-wrapping/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob,
            string name,
            Uri uri,
            [Table("PerfectGiftWrappingResults", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Uri:{uri}\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
            string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");

            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            var imageDetails = await AnalyzeImageUrl(client, uri.ToString());

            var giftWrappingResult = new GiftWrappingResult(
                name, 
                uri, 
                imageDetails.Description.Tags.Contains("box"), 
                imageDetails.Description.Tags.Contains("gift wrapping"), 
                imageDetails.Description.Tags.Contains("ribbon"), 
                imageDetails.Description.Tags.Contains("present")
            );

            var tableOperation = TableOperation.Insert(giftWrappingResult);

            try
            {
                await table.ExecuteAsync(tableOperation);
            }
            catch (StorageException ex)
            {
                log.LogError($"An error occured while trying to store the analysed letter - {ex.InnerException.ToString()}");
            }
        }

        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };

            return client;
        }

        public static async Task<ImageDetails> AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
        {
            var imageDetails = new ImageDetails();

            imageDetails.ImageUrl = imageUrl;

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags
            };

            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, features);

            foreach (var caption in results.Description.Captions)
            {
                imageDetails.Description.Captions.Add(
                    new Caption
                    {
                        Text = caption.Text,
                        Confidence = caption.Confidence
                    }
                );
            }

            foreach (var tag in results.Tags)
            {
                imageDetails.Description.Tags.Add(
                    tag.Name
                );
            }

            return imageDetails;
        }
    }
}
