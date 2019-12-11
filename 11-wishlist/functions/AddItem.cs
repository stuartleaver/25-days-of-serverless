using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;

namespace WishList.Functions
{
    public static class AddItem
    {
        [FunctionName("AddItem")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "wishlist", 
                collectionName: "items", 
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<dynamic> document,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WishListItem>(requestBody);

            await document.AddAsync(data);
        }
    }
}
