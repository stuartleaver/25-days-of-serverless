using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Christmas.ChasseGalerie
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CanoeHealth
    {
        [JsonProperty("health")]
        public CanoeHealthEnum Health { get; set; }

        [JsonProperty("counter")]
        public int Counter;

        [JsonProperty("workerCursesAt")]
        public int WorkerCursesAt;

        private int _counterMax => 10;

        [FunctionName(nameof(CanoeHealth))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<CanoeHealth>();

        public void Reset()
        {
            var random = new Random();

            Counter = 1;

            WorkerCursesAt = random.Next(1, 10);
        }

        public void SetStatus()
        {
            if (Counter == 0 && WorkerCursesAt == 0)
                Reset();

            if (WorkerCursesAt == Counter)
            {
                Health = CanoeHealthEnum.Error;
            }
            else
            {
                Health = CanoeHealthEnum.Ok;
            }

            Counter++;

            if (Counter > _counterMax)
                Reset();
        }
    }
}