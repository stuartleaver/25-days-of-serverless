using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace EzrasPotluck
{
    public static class ListItems
    {
        [FunctionName("ListItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ezraspotluck")] HttpRequest req,
            [Table("EzrasPotluck", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("Ezra's potluck HTTP trigger function to list items.");

            var query = new TableQuery<PotluckItem>();

            TableContinuationToken token = null;

            var items = new List<PotluckItem>();
        
            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, token);

                token = segment.ContinuationToken;

                foreach(PotluckItem item in segment)
                {
                    items.Add(item);
                }
            } while (token != null);

            return (ActionResult)new OkObjectResult(items);
        }
    }
}