using System.Collections.Generic;

namespace Christmas.GiftWrappingAnalyser.Entities
{
    public class Description
    {
        public List<string> Tags { get; set; }

        public List<Caption> Captions { get; set; }

        public Description()
        {
            Tags = new List<string>();

            Captions = new List<Caption>();
        }
    }
}