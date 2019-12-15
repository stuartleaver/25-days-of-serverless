using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Christmas.ComputerVision.Entities;

namespace Christmas.ComputerVision
{
    public static class ComputerVision
    {
        [FunctionName("ComputerVision")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string subscriptionKey = Environment.GetEnvironmentVariable("COMPUTER_VISION_SUBSCRIPTION_KEY");
            string endpoint = Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT");

            string url = req.Query["url"];

            ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);

            var imageDetails = await AnalyzeImageUrl(client, url);

            return url != null
                ? (ActionResult)new OkObjectResult(imageDetails)
                : new BadRequestObjectResult("Please pass a url of an image on the query string");
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