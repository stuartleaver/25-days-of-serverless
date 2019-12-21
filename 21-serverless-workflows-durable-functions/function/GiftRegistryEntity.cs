using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace Christmas.GiftRegistry
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GiftRegistryEntity
    {
        [JsonProperty("state")]
        public GiftRegistryState State { get; set; }

        [JsonProperty("entries")]
        public List<string> Entries { get; set; }

        [FunctionName(nameof(GiftRegistryEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<GiftRegistryEntity>();

        public void Open()
        {
            State = GiftRegistryState.Open;

            Entries = new List<string>();
        }

        public void Add(string entry)
        {
            Entries.Add(entry);
        }

        public void Close()
        {
            if(State != GiftRegistryState.Closed)
                State = GiftRegistryState.Closed;
        }
    }
}