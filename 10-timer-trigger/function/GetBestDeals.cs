using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Net;

namespace Christmas.BestDeals
{
    public static class GetBestDeals
    {
        [FunctionName("GetBestDeals")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("bestdeals", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = new TableQuery<BestDeal>();

            TableContinuationToken token = null;

            var bestdeals = new List<BestDeal>();

            do
            {
                var deals = await table.ExecuteQuerySegmentedAsync(query, token);

                token = deals.ContinuationToken;

                foreach (var deal in deals.Results)
                {
                    bestdeals.Add(deal);
                }
            } while (token != null);

            var json = JsonConvert.SerializeObject(bestdeals, Formatting.Indented);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
