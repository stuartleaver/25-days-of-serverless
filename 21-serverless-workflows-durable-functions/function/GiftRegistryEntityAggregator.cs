using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Christmas.GiftRegistry
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GiftRegistryEntityAggregator
    {
        [JsonProperty("openGiftRegistrys")]
        public int OpenGiftRegistrys { get; set; }

        [JsonProperty("closedGiftRegistrys")]
        public int ClosedGiftRegistrys { get; set; }

        [JsonProperty("totalGiftRegistryEntrys")]
        public int TotalGiftRegistryEntrys { get; set; }

        [FunctionName(nameof(GiftRegistryEntityAggregator))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<GiftRegistryEntityAggregator>();

        public void Open()
        {
            OpenGiftRegistrys++;
        }

        public void Add(string entry)
        {
            TotalGiftRegistryEntrys++;
        }

        public void Close()
        {
            ClosedGiftRegistrys++;

            OpenGiftRegistrys--;
        }
    }
}