using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WishList.Functions
{
    public static class QueryItems
    {
        [FunctionName("QueryItems")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "wishlist", 
                collectionName: "items", 
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var collectionUri = UriFactory.CreateDocumentCollectionUri("wishlist", "items");

            var query = client.CreateDocumentQuery<WishListItem>(collectionUri).AsEnumerable().ToArray();

            return new OkObjectResult(query);
        }
    }
}
