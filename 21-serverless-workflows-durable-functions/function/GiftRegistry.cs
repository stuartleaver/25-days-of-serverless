using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Christmas.GiftRegistry
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GiftRegistry
    {
        [FunctionName("Open")]
        public static async Task<HttpResponseMessage> OpenGiftRegistry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "giftregistry/open")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - Open a gift registry");

            var giftRegistryEntityId = new EntityId(nameof(GiftRegistryEntity), Guid.NewGuid().ToString());
            var giftRegistryEntityAggregatorId = new EntityId(nameof(GiftRegistryEntityAggregator), "GiftRegistryAggregate");

            await client.SignalEntityAsync(giftRegistryEntityId, "Open");
            await client.SignalEntityAsync(giftRegistryEntityAggregatorId, "Open");

            await client.SignalEntityAsync(giftRegistryEntityId, DateTime.UtcNow.AddMinutes(5), "Close");
            await client.SignalEntityAsync(giftRegistryEntityAggregatorId, DateTime.UtcNow.AddMinutes(5), "Close");

            // string name = req.Query["name"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(giftRegistryEntityId), Encoding.UTF8, "application/json")
            };

        }

        [FunctionName("Add")]
        public static async Task<IActionResult> AddToGiftRegistry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "giftregistry/add/{id}")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - Add entry to gift registry");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var text = data?.entry;

            var giftRegistryEntityId = new EntityId(nameof(GiftRegistryEntity), id);
            var giftRegistryEntityAggregatorId = new EntityId(nameof(GiftRegistryEntityAggregator), "GiftRegistryAggregate");

            var state = await client.ReadEntityStateAsync<GiftRegistryEntity>(giftRegistryEntityId);

            if (state.EntityExists)
            {
                if (state.EntityState.State != GiftRegistryState.Open)
                    return new BadRequestResult();

                await client.SignalEntityAsync(giftRegistryEntityId, "Add", text);
                await client.SignalEntityAsync(giftRegistryEntityAggregatorId, "Add");

                return new OkObjectResult("Entry added");

            }

            return new NotFoundObjectResult(giftRegistryEntityId);
        }

        [FunctionName("Finish")]
        public static async Task<IActionResult> FinishGiftRegistry(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "giftregistry/finish/{id}")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            string id,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - Finish gift registry");

            var giftRegistryEntityId = new EntityId(nameof(GiftRegistryEntity), id);
            var giftRegistryEntityAggregatorId = new EntityId(nameof(GiftRegistryEntityAggregator), "GiftRegistryAggregate");

            var state = await client.ReadEntityStateAsync<GiftRegistryEntity>(giftRegistryEntityId);

            if (state.EntityExists)
            {
                await client.SignalEntityAsync(giftRegistryEntityId, "Close");
                await client.SignalEntityAsync(giftRegistryEntityAggregatorId, "Close");

                return new OkObjectResult("Gift Registry Closed");

            }

            return new NotFoundObjectResult(giftRegistryEntityId);
        }

        [FunctionName("Stats")]
        public static async Task<IActionResult> GiftRegistryStats(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "giftregistry/stats")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function - Get gift registry stats");

            var giftRegistryEntityAggregatorId = new EntityId(nameof(GiftRegistryEntityAggregator), "GiftRegistryAggregate");

            var state = await client.ReadEntityStateAsync<JObject>(giftRegistryEntityAggregatorId);

            return new OkObjectResult(state);
        }
    }
}