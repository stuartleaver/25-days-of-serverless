using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Christmas.BestDeals
{
    public class BestDeal : TableEntity
    {
        public string TweetId { get; set; }

        public string Username { get; set; }

        [JsonProperty("tweetText")]
        public string TweetText { get; set; }

        [JsonProperty("url")]
        public string Url => $"https://twitter.com/{Username}/status/{TweetId}";
    }
}