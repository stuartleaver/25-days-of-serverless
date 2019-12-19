using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;

namespace Christmas.DurableEntities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty("isfilling")]
        public bool IsFilling { get; set; }

        [JsonProperty("fillstarttime")]
        public DateTime FillStartTime { get; set; }

        public int MaxBallonCount => (int)Math.Round(200 / 0.6);

        [JsonProperty("balloncount")]
        public int BallonCount { get; set; }

        [FunctionName(nameof(Tank))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        => ctx.DispatchAsync<Tank>();

        [FunctionName("Compressor")]
        public static Task Operate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "compressor/{operation}")] HttpRequest req,
            string operation,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation($"Compressor operation - {operation}.");

            var entityId = new EntityId(nameof(Tank), "tank");

            return client.SignalEntityAsync(entityId, operation);
        }

        [FunctionName("QueryTank")]
        public static async Task<IActionResult> Query(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "querytank")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation($"Query tank.");

            var entityId = new EntityId(nameof(Tank), "tank");

            await client.SignalEntityAsync(entityId, "Update");

            var state = await client.ReadEntityStateAsync<JObject>(entityId);

            return new OkObjectResult(state);
        }

        [FunctionName("FillBallonsFromTank")]
        public static Task FillBallonsFromTank(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fillbaloons/{count}")] HttpRequest req,
            int count,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation($"Fill {count} baloons.");

            var entityId = new EntityId(nameof(Tank), "tank");

            return client.SignalEntityAsync(entityId, "FillBallons", count);
        }

        public void Start()
        {
            IsFilling = true;

            FillStartTime = DateTime.UtcNow;
        }

        public void Reset()
        {
            IsFilling = false;

            BallonCount = 0;
        }

        public void Stop()
        {
            var now = DateTime.UtcNow;

            if (IsFilling)
            {
                IsFilling = false;

                var fillDurationMinutes = now.Subtract(FillStartTime);

                UpdateBallonCount(fillDurationMinutes);
            }
        }

        public void Update()
        {
            if (IsFilling)
            {
                var now = DateTime.UtcNow;

                var fillDuration = now.Subtract(FillStartTime);

                UpdateBallonCount(fillDuration);
            }
        }

        public void FillBallons(int count)
        {
            BallonCount = BallonCount - count;
        }

        private void UpdateBallonCount(TimeSpan fillDuration)
        {
            var ballonsFilled = (int)Math.Round((25 * fillDuration.TotalMinutes) / 0.6);

            if(BallonCount + ballonsFilled > MaxBallonCount)
            {
                IsFilling = false;

                BallonCount = MaxBallonCount;
            }
            else
            {
                BallonCount = BallonCount + ballonsFilled;
            }
        }
    }
}