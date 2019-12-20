using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Christmas.GiftWrappingAnalyser.Entities;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.NotificationHubs;

namespace Christmas.GiftWrappingAnalyser
{
    public static class GiftWrappingAnalyser
    {
        [FunctionName("GiftWrappingAnalyser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function");

            string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
            string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");

            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            var imageDetails = await AnalyzeImageUrl(client, req.Body);

            var giftDeliveryResult = new GiftDeliveryResult(
                imageDetails.Description.Tags.Contains("box"),
                imageDetails.Description.Tags.Contains("gift wrapping"),
                imageDetails.Description.Tags.Contains("ribbon"),
                imageDetails.Description.Tags.Contains("present")
            );

            if (giftDeliveryResult.IsDelivered)
            {
                NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(
                    Environment.GetEnvironmentVariable("NOTIFICATION_HUB_CONNECTION_STRING"),
                    Environment.GetEnvironmentVariable("NOTIFICATION_HUB_NAME"));

                string payload = "{\"data\": {\"message\":\"A new gift has been delivered.\" }}";

                await hub.SendFcmNativeNotificationAsync(payload);
            }

            return new OkObjectResult("Request completed");
        }

        private static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };

            return client;
        }

        public static async Task<ImageDetails> AnalyzeImageUrl(ComputerVisionClient client, Stream body)
        {
            var imageDetails = new ImageDetails();
            var results = new ImageAnalysis();

            // Creating a list that defines the features to be extracted from the image. 
            List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags
            };

            using (var reader = new StreamReader(body))
            {
                var base64 = await reader.ReadToEndAsync();
                var bytes = Convert.FromBase64String(base64);

                using (var stream = new MemoryStream(bytes))
                {
                    results = await client.AnalyzeImageInStreamAsync(stream, features);
                }
            }

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