using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.AspNetCore.Http;

namespace EzrasPotluck
{
    public static class DeleteItem
    {
        [FunctionName("DeleteItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "ezraspotluck")] HttpRequest req,
            [Table("EzrasPotluck", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Ezra's potluck HTTP trigger function to delete an item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string name = data?.name;
            string foodItem = data?.foodItem;

            if(!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(foodItem))
            {
                var operation = TableOperation.Delete(new PotluckItem(name, foodItem) { ETag = "*" });

                await table.ExecuteAsync(operation);
            }

            return (ActionResult)new NoContentResult();
        }
    }
}