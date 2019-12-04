using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace EzrasPotluck
{
    public static class AddUpdateItem
    {
        [FunctionName("AddUpdateItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "patch", Route = "ezraspotluck")] HttpRequest req,
            [Table("EzrasPotluck", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Ezra's potluck HTTP trigger function to add or update an item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = data?.name;
            string foodItem = data?.foodItem;

            if(!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(foodItem))
            {
                var operation = TableOperation.InsertOrReplace(new PotluckItem(name, foodItem));

                await table.ExecuteAsync(operation);
            }

            return !string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(foodItem)
                ? (ActionResult)new OkObjectResult($"Hello {name}. Thank you for offering to bring {foodItem}.")
                : new BadRequestObjectResult("Please check the food items you tried to add.");
        }
    }
}