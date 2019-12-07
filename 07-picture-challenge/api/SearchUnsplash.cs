using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PictureChallenge.Api.Services;
using Newtonsoft.Json;

namespace PictureChallenge.Api
{
    public static class SearchUnsplash
    {
        public static ImageSearchService _imageSearchService;

        [FunctionName("SearchUnsplash")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "searchunsplash")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a requestto search Unsplash for {req.Query["query"]}.");

            _imageSearchService = new ImageSearchService(Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_KEY"), Environment.GetEnvironmentVariable("UNSPLASH_SECRET_KEY"));

            var image = await _imageSearchService.GetImageAsync(req.Query["query"]);

            return image != null
                ? (ActionResult)new OkObjectResult(JsonConvert.SerializeObject(image))
                : new BadRequestObjectResult("Please pass a name on the query string");
        }
    }
}
