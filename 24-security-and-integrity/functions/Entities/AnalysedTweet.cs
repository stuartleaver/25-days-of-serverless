using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Christmas.Sweden.Entities
{
    public class AnalysedTweet : TableEntity
    {
        public string OriginalText { get; set; }

        public string TranslatedText { get; set; }

        public string OriginalLanguage { get; set; }

        public string TranslatedLanguage { get; set; }

        public string Username { get; set; }

        public string SentTimestamp { get; set; }

        public double Sentiment { get; set; }

        public AnalysedTweet()
        {
            PartitionKey = Username.Replace(" ", "");

            RowKey = Guid.NewGuid().ToString();
        }
    }
}