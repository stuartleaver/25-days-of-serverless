using Newtonsoft.Json;

namespace WishList.Functions
{
    public class WishListItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("wish")]
        public string Wish { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}

